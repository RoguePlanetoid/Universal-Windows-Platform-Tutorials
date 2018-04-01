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
    private const string file_name = "video.mp4";

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
                StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo
            };
            _capture = new MediaCapture();
            await _capture.InitializeAsync();
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

    public async void Record(CaptureElement preview)
    {
        await Init();
        preview.Source = _capture;
        await _capture.StartPreviewAsync();
        await _capture.StartRecordToStreamAsync(MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto), _buffer);
        if (Recording) throw new InvalidOperationException("Cannot execute two recordings at the same time");
        Recording = true;
    }

    public async void Stop()
    {
        await _capture.StopRecordAsync();
        Recording = false;
    }

    public async Task Play(CoreDispatcher dispatcher, MediaElement playback)
    {
        IRandomAccessStream video = _buffer.CloneStream();
        if (video == null) throw new ArgumentNullException("buffer");
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
                await RandomAccessStream.CopyAndCloseAsync(video.GetInputStreamAt(0), fileStream.GetOutputStreamAt(0));
                await video.FlushAsync();
                video.Dispose();
            }
            IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read);
            playback.SetSource(stream, storageFile.FileType);
            playback.Play();
        });
    }
}