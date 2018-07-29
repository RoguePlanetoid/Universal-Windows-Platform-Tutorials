using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.System;
using Windows.Media.SpeechRecognition;
using System.Text;
using Windows.UI.Core;
using Windows.Globalization;
using Windows.UI.Xaml.Media;

public class Library
{
    private const string app_title = "Dictates App";
    private const string extension_txt = ".txt";
    private const string label_dictate = "Dictate";
    private const string label_stop = "Stop";
    private const uint privacy_statement_declined = 0x80045509;

    private IAsyncOperation<IUICommand> _dialogCommand;
    private SpeechRecognizer _recogniser = new SpeechRecognizer();
    private StringBuilder _builder = new StringBuilder();
    private CoreDispatcher _dispatcher;
    private bool _listening;

    public delegate void ResultHandler(string value);
    public event ResultHandler Result;

    public delegate void CompletedHandler();
    public event CompletedHandler Completed;

    private async Task<bool> ShowDialogAsync(string content, string title = app_title)
    {
        try
        {
            if (_dialogCommand != null)
            {
                _dialogCommand.Cancel();
                _dialogCommand = null;
            }
            _dialogCommand = new MessageDialog(content, title).ShowAsync();
            await _dialogCommand;
            return true;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    private async void ShowPrivacy()
    {
        await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-speechtyping"));
    }

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
            if (open != null) return await FileIO.ReadTextAsync(open);
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
            StorageFile save = await picker.PickSaveFileAsync();
            if (save != null) await FileIO.WriteTextAsync(save, contents);
        }
        finally
        {
        }
    }

    private async void Recogniser_Completed(
        SpeechContinuousRecognitionSession sender,
        SpeechContinuousRecognitionCompletedEventArgs args)
    {
        if (args.Status != SpeechRecognitionResultStatus.Success)
        {
            if (args.Status == SpeechRecognitionResultStatus.TimeoutExceeded)
            {
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Result?.Invoke(_builder.ToString());
                    Completed?.Invoke();
                    _listening = false;
                });
            }
            else
            {
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Completed?.Invoke();
                    _listening = false;
                });
            }
        }
    }

    private async void Recogniser_ResultGenerated(
        SpeechContinuousRecognitionSession sender,
        SpeechContinuousRecognitionResultGeneratedEventArgs args)
    {
        if (args.Result.Confidence == SpeechRecognitionConfidence.Medium ||
            args.Result.Confidence == SpeechRecognitionConfidence.High)
        {
            _builder.Append($"{args.Result.Text} ");
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Result?.Invoke(_builder.ToString());
            });
        }
    }

    private async void SpeechRecognizer_HypothesisGenerated(
        SpeechRecognizer sender,
        SpeechRecognitionHypothesisGeneratedEventArgs args)
    {
        string hypothesis = args.Hypothesis.Text;
        string content = $"{_builder.ToString()} {hypothesis} ...";
        await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
            Result?.Invoke(content);
        });
    }

    private async void Setup(Language language)
    {
        if (_recogniser != null)
        {
            _recogniser.ContinuousRecognitionSession.Completed -= Recogniser_Completed;
            _recogniser.ContinuousRecognitionSession.ResultGenerated -= Recogniser_ResultGenerated;
            _recogniser.HypothesisGenerated -= SpeechRecognizer_HypothesisGenerated;
            _recogniser.Dispose();
            _recogniser = null;
        }
        _recogniser = new SpeechRecognizer(language);
        SpeechRecognitionTopicConstraint constraint = new SpeechRecognitionTopicConstraint(
        SpeechRecognitionScenario.Dictation, "dictation");
        _recogniser.Constraints.Add(constraint);
        SpeechRecognitionCompilationResult result = await _recogniser.CompileConstraintsAsync();
        if (result.Status != SpeechRecognitionResultStatus.Success)
        {
            await ShowDialogAsync($"Grammar Compilation Failed: {result.Status.ToString()}");
        }
        _recogniser.ContinuousRecognitionSession.Completed += Recogniser_Completed;
        _recogniser.ContinuousRecognitionSession.ResultGenerated += Recogniser_ResultGenerated;
        _recogniser.HypothesisGenerated += SpeechRecognizer_HypothesisGenerated;
    }

    public Dictionary<Language, string> Languages()
    {
        Dictionary<Language, string> results = new Dictionary<Language, string>();
        foreach (Language language in SpeechRecognizer.SupportedTopicLanguages)
        {
            results.Add(language, language.DisplayName);
        }
        return results;
    }

    public async void Language(object value)
    {
        if (_recogniser != null)
        {
            Language language = (Language)value;
            if (_recogniser.CurrentLanguage != language)
            {
                try
                {
                    Setup(language);
                }
                catch (Exception exception)
                {
                    await ShowDialogAsync(exception.Message);
                }
            }
        }
    }

    private void Content_TextChanged(object sender, TextChangedEventArgs e)
    {
        var grid = (Grid)VisualTreeHelper.GetChild((TextBox)sender, 0);
        for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
        {
            object obj = VisualTreeHelper.GetChild(grid, i);
            if (!(obj is ScrollViewer)) continue;
            ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
            break;
        }
    }

    public void Init(AppBarButton microphone, ComboBox languages, TextBox content)
    {
        _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        Completed += () =>
        {
            microphone.Label = label_dictate;
            languages.IsEnabled = true;
        };
        Result += (string value) =>
        {
            content.Text = value;
        };
        content.TextChanged += Content_TextChanged;
    }

    public async void Dictate(AppBarButton dictate, ComboBox languages, TextBox content)
    {
        dictate.IsEnabled = false;
        if (_listening == false)
        {
            if (_recogniser.State == SpeechRecognizerState.Idle)
            {
                dictate.Label = label_stop;
                languages.IsEnabled = false;
                try
                {
                    _listening = true;
                    await _recogniser.ContinuousRecognitionSession.StartAsync();
                }
                catch (Exception ex)
                {
                    if ((uint)ex.HResult == privacy_statement_declined)
                    {
                        ShowPrivacy();
                    }
                    else
                    {
                        await ShowDialogAsync(ex.Message);
                    }
                    _listening = false;
                    dictate.Label = label_dictate;
                    languages.IsEnabled = true;
                }
            }
        }
        else
        {
            _listening = false;
            dictate.Label = label_dictate;
            languages.IsEnabled = true;
            if (_recogniser.State != SpeechRecognizerState.Idle)
            {
                try
                {
                    await _recogniser.ContinuousRecognitionSession.StopAsync();
                    content.Text = _builder.ToString();
                }
                catch (Exception ex)
                {
                    await ShowDialogAsync(ex.Message);
                }
            }
        }
        dictate.IsEnabled = true;
    }

    public async void New(TextBox text)
    {
        if (await ShowDialogAsync("Create New Document?"))
        {
            _builder.Clear();
            text.Text = string.Empty;
        }
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
}