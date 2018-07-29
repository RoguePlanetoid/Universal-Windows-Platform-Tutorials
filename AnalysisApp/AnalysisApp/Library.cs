using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Xaml.Controls;

public class Library
{
    private InkPresenter _presenter;
    private InkAnalyzer _analyser;

    private void Presenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
    {
        _analyser.AddDataForStrokes(args.Strokes);
    }

    private void Presenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
    {
        foreach (InkStroke stroke in args.Strokes)
        {
            _analyser.RemoveDataForStroke(stroke.Id);
        }
    }

    public void Init(ref InkCanvas inkCanvas)
    {
        _presenter = inkCanvas.InkPresenter;
        _presenter.StrokesCollected += Presenter_StrokesCollected;
        _presenter.StrokesErased += Presenter_StrokesErased;
        _presenter.InputDeviceTypes =
        CoreInputDeviceTypes.Pen |
        CoreInputDeviceTypes.Mouse |
        CoreInputDeviceTypes.Touch;
        _analyser = new InkAnalyzer();
    }

    public async void Analyse(TextBlock display)
    {
        IReadOnlyList<InkStroke> strokes = _presenter.StrokeContainer.GetStrokes();
        foreach (InkStroke stroke in strokes)
        {
            _analyser.SetStrokeDataKind(stroke.Id, InkAnalysisStrokeKind.Writing);
        }
        InkAnalysisResult result = await _analyser.AnalyzeAsync();
        if (result.Status == InkAnalysisStatus.Updated)
        {
            display.Text = _analyser.AnalysisRoot.RecognizedText;
        }
    }

    public void Clear(ref TextBlock display)
    {
        display.Text = string.Empty;
        _presenter.StrokeContainer.Clear();
        _analyser.ClearDataForAllStrokes();
    }
}