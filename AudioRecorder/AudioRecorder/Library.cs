using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

public class Library
{
    private const string file_name = "audio.m4a";

    private string _filename;
    private MediaCapture _capture;
    private InMemoryRandomAccessStream _buffer;

    public bool Recording;

    private async Task<bool> Init()
    {
        if (_buffer != null)
        {
            _buffer.Dispose();
        }
        _buffer = new InMemoryRandomAccessStream();
        if (_capture != null)
        {
            _capture.Dispose();
        }
        try
        {
            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Audio
            };
            _capture = new MediaCapture();
            await _capture.InitializeAsync(settings);
            _capture.RecordLimitationExceeded += (MediaCapture sender) =>
            {
                Stop();
                throw new Exception("Exceeded Record Limitation");
            };
            _capture.Failed += (MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs) =>
            {
                Recording = false;
                throw new Exception(string.Format("Code: {0}. {1}", errorEventArgs.Code, errorEventArgs.Message));
            };
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null && ex.InnerException.GetType() == typeof(UnauthorizedAccessException))
            {
                throw ex.InnerException;
            }
            throw;
        }
        return true;
    }

    public async void Record()
    {
        await Init();
        await _capture.StartRecordToStreamAsync(MediaEncodingProfile.CreateM4a(AudioEncodingQuality.Auto), _buffer);
        if (Recording) throw new InvalidOperationException("Cannot execute two recordings at the same time");
        Recording = true;
    }

    public async void Stop()
    {
        await _capture.StopRecordAsync();
        Recording = false;
    }

    public async Task Play(CoreDispatcher dispatcher)
    {
        MediaElement playback = new MediaElement();
        IRandomAccessStream audio = _buffer.CloneStream();
        if (audio == null) throw new ArgumentNullException("buffer");
        StorageFolder storageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        if (!string.IsNullOrEmpty(_filename))
        {
            StorageFile original = await storageFolder.GetFileAsync(_filename);
            await original.DeleteAsync();
        }
        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        {
            StorageFile storageFile = await storageFolder.CreateFileAsync(file_name, CreationCollisionOption.GenerateUniqueName);
            _filename = storageFile.Name;
            using (IRandomAccessStream fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await RandomAccessStream.CopyAndCloseAsync(audio.GetInputStreamAt(0), fileStream.GetOutputStreamAt(0));
                await audio.FlushAsync();
                audio.Dispose();
            }
            IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read);
            playback.SetSource(stream, storageFile.FileType);
            playback.Play();
        });
    }
}