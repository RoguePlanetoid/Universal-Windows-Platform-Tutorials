using System;
using System.Numerics;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

public class Library
{
    private const int scroll_step = 2;
    private const int mouse_wheel = 120;
    private const double change_increment = 0.5;

    private MediaPlaybackSphericalVideoProjection _projection;
    private MediaPlayerElement _element = null;
    private MediaPlayer _player;
    private Grid _grid = null;
    private bool _press = false;
    private double _delta = 1.8f;
    private double _centerX = 0;
    private double _centerY = 0;

    private Quaternion GetPitchRoll(double heading, double pitch, double roll)
    {
        double ToRadians(double degree)
        {
            return degree * Math.PI / 180.0;
        }
        Quaternion result;
        double headingPart = ToRadians(heading) * change_increment;
        double sin1 = Math.Sin(headingPart);
        double cos1 = Math.Cos(headingPart);
        double pitchPart = ToRadians(-pitch) * change_increment;
        double sin2 = Math.Sin(pitchPart);
        double cos2 = Math.Cos(pitchPart);
        double rollPart = ToRadians(roll) * change_increment;
        double sin3 = Math.Sin(rollPart);
        double cos3 = Math.Cos(rollPart);
        result.W = (float)(cos1 * cos2 * cos3 - sin1 * sin2 * sin3);
        result.X = (float)(cos1 * cos2 * sin3 + sin1 * sin2 * cos3);
        result.Y = (float)(sin1 * cos2 * cos3 + cos1 * sin2 * sin3);
        result.Z = (float)(cos1 * sin2 * cos3 - sin1 * cos2 * sin3);
        return result;
    }

    private bool IsOpened()
    {
        if (_player != null && _projection != null &&
        _player.PlaybackSession.PlaybackState != MediaPlaybackState.Opening &&
        _player.PlaybackSession.PlaybackState != MediaPlaybackState.None) return true;
        return false;
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (e.OriginalSource != _grid && _press)
        {
            double changeX = e.GetCurrentPoint(_element).Position.X - _centerX;
            double changeY = _centerY - e.GetCurrentPoint(_element).Position.Y;
            _projection.ViewOrientation = GetPitchRoll(changeX, changeY, 0);
        }
        e.Handled = true;
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        _press = false;
        e.Handled = true;
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (e.OriginalSource != _grid && IsOpened()) _press = true;
        e.Handled = true;
    }

    private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        if (e.OriginalSource != _grid)
        {
            double value = _projection.HorizontalFieldOfViewInDegrees +
                (scroll_step * e.GetCurrentPoint(_element).Properties.MouseWheelDelta / mouse_wheel);
            if (value > 0 && value <= 180)
            {
                _projection.HorizontalFieldOfViewInDegrees = value;
            }
        }
        e.Handled = true;
    }

    private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
    {
        _delta = (_player.PlaybackSession.PlaybackState ==
        MediaPlaybackState.Playing) ? 1.8f : 0.25f;
    }

    private void Player_MediaOpened(MediaPlayer sender, object args)
    {
        _projection = _player.PlaybackSession.SphericalVideoProjection;
        SphericalVideoFrameFormat videoFormat = _projection.FrameFormat;
        if (videoFormat != SphericalVideoFrameFormat.Equirectangular)
        {
            _projection.FrameFormat = SphericalVideoFrameFormat.Equirectangular;
        }
        _projection.IsEnabled = true;
        _projection.HorizontalFieldOfViewInDegrees = 120;
        _player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
    }

    public void Layout(ref MediaPlayerElement display)
    {
        if (_element == null) _element = display;
        _centerX = _element.ActualWidth / 2;
        _centerY = _element.ActualHeight / 2;
        if (_grid == null)
        {
            FrameworkElement _root = (FrameworkElement)VisualTreeHelper.GetChild(_element.TransportControls, 0);
            if (_root != null)
            {
                _grid = (Grid)_root.FindName("ControlPanelGrid");
                _grid.PointerPressed += OnPointerPressed;
                _grid.PointerReleased += OnPointerReleased;
                _grid.PointerWheelChanged += OnPointerWheelChanged;
                _element.PointerPressed += OnPointerPressed;
                _element.PointerReleased += OnPointerReleased;
                _element.PointerMoved += OnPointerMoved;
                _element.PointerWheelChanged += OnPointerWheelChanged;
            }
        }
    }

    public async void Open(MediaPlayerElement display)
    {
        try
        {
            FileOpenPicker picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".mp4");
            StorageFile open = await picker.PickSingleFileAsync();
            if (open != null)
            {
                if (_element == null) _element = display;
                _player = new MediaPlayer
                {
                    Source = MediaSource.CreateFromStorageFile(open)
                };
                _player.MediaOpened += Player_MediaOpened;
                _element.SetMediaPlayer(_player);
            }
        }
        finally
        {
            // Ignore Exceptions
        }
    }
}