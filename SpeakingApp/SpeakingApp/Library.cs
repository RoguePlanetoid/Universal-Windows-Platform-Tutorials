using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class Library
{
    private const string extension_txt = ".txt";
    private const string extension_wav = ".wav";

    private SpeechSynthesizer synth = new SpeechSynthesizer();

    private async Task<string> OpenAsync()
    {
        try
        {
            FileOpenPicker picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            picker.FileTypeFilter.Add(extension_txt);
            StorageFile open = await picker.PickSingleFileAsync();
            if (open != null)
            {
                return await FileIO.ReadTextAsync(open);
            }
        }
        finally
        {
        }
        return null;
    }

    private async void SaveAsync(string contents)
    {
        try
        {
            FileSavePicker picker = new FileSavePicker()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                DefaultFileExtension = extension_txt,
                SuggestedFileName = "Document"
            };
            picker.FileTypeChoices.Add("Text File", new List<string>() { extension_txt });
            picker.FileTypeChoices.Add("Wave File", new List<string>() { extension_wav });
            StorageFile save = await picker.PickSaveFileAsync();
            if (save != null)
            {
                if (save.FileType == extension_txt)
                {
                    await FileIO.WriteTextAsync(save, contents);
                }
                else if (save.FileType == extension_wav)
                {
                    using (SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(contents))
                    {
                        using (DataReader reader = new DataReader(stream))
                        {
                            await reader.LoadAsync((uint)stream.Size);
                            IBuffer buffer = reader.ReadBuffer((uint)stream.Size);
                            await FileIO.WriteBufferAsync(save, buffer);
                        }
                    }
                }
            }
        }
        finally
        {
        }
    }

    private async void Speak(string text, MediaElement media)
    {
        try
        {
            if (media.CurrentState == MediaElementState.Playing)
            {
                media.Stop();
            }
            else
            {
                SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(text);
                media.AutoPlay = true;
                media.SetSource(stream, stream.ContentType);
                media.Play();
            }
        }
        finally
        {
        }
    }

    public Dictionary<string, string> Voices()
    {
        Dictionary<string, string> results = new Dictionary<string, string>();
        foreach (VoiceInformation voice in SpeechSynthesizer.AllVoices.OrderBy(o => o.DisplayName))
        {
            results.Add(voice.Id, voice.DisplayName);
        }
        return results;
    }

    public void Voice(string id)
    {
        synth.Voice = SpeechSynthesizer.AllVoices.First(f => f.Id == id);
    }

    public void New(ref TextBox text, ref MediaElement media)
    {
        media.Source = null;
        text.Text = string.Empty;
    }

    public async void Open(TextBox text)
    {
        string content = await OpenAsync();
        if (content != null)
        {
            text.Text = content;
        }
    }

    public void Save(ref TextBox text)
    {
        SaveAsync(text.Text);
    }

    public void Play(ref TextBox text, ref MediaElement media)
    {
        Speak(text.Text, media);
    }
}