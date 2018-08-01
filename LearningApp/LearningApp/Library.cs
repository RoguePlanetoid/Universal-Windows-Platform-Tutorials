using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace LearningApp
{
    public class Library
    {
        private InkPresenter _presenter;
        private MNISTModel _model = new MNISTModel();
        private MNISTModelInput _input = new MNISTModelInput();
        private MNISTModelOutput _output = new MNISTModelOutput();

        private async Task<VideoFrame> Render(InkCanvas inkCanvas)
        {
            RenderTargetBitmap target = new RenderTargetBitmap();
            await target.RenderAsync(inkCanvas, 28, 28);
            IBuffer buffer = await target.GetPixelsAsync();
            SoftwareBitmap bitmap = SoftwareBitmap.CreateCopyFromBuffer(buffer,
            BitmapPixelFormat.Bgra8, target.PixelWidth, target.PixelHeight,
            BitmapAlphaMode.Ignore);
            buffer = null;
            target = null;
            return VideoFrame.CreateWithSoftwareBitmap(bitmap);
        }

        public async void Init(InkCanvas inkCanvas)
        {
            _presenter = inkCanvas.InkPresenter;
            _presenter.InputDeviceTypes =
            CoreInputDeviceTypes.Pen |
            CoreInputDeviceTypes.Mouse |
            CoreInputDeviceTypes.Touch;
            _presenter.UpdateDefaultDrawingAttributes(new InkDrawingAttributes()
            {
                Color = Colors.White,
                Size = new Size(22, 22),
                IgnorePressure = true,
                IgnoreTilt = true,
            });
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(
            new Uri($"ms-appx:///Assets/mnist.onnx"));
            _model = await MNISTModel.CreateMNISTModel(file);
        }

        public async void Recognise(InkCanvas inkCanvas, TextBlock display)
        {
            _input.Input3 = await Render(inkCanvas);
            _output = await _model.EvaluateAsync(_input);
            int result = _output.Plus214_Output_0.IndexOf(_output.Plus214_Output_0.Max());
            display.Text = result.ToString();
        }

        public void Clear(ref TextBlock display)
        {
            display.Text = string.Empty;
            _presenter.StrokeContainer.Clear();
        }
    }
}