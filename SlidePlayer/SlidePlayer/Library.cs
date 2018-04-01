using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

public class Library
{
    public delegate void PlayingEvent(BitmapImage image, int index);
    public event PlayingEvent Playing;
    public delegate void StoppedEvent();
    public event StoppedEvent Stopped;

    private List<BitmapImage> _list = new List<BitmapImage>();
    private int _index = 0;
    private bool _paused = false;

    public bool IsPlaying { get; set; }
    public int Speed { get; set; }
    public int Position { get; set; }

    public void Go(ref Image display, string value, KeyRoutedEventArgs args)
    {
        if (args.Key == Windows.System.VirtualKey.Enter)
        {
            try
            {
                display.Source = new BitmapImage(new Uri(value));
            }
            catch
            {

            }
        }
    }

    public double Add(string value)
    {
        _list.Add(new BitmapImage(new Uri(value)));
        return _list.Count - 1;
    }

    public double Remove(int index)
    {
        if (index >= 0 && index < _list.Count)
        {
            _list.RemoveAt(index);
        }
        return _list.Count - 1;
    }

    public async void Play()
    {
        if (_list.Any() && (!_paused || !IsPlaying))
        {
            IsPlaying = true;
            _paused = false;
            while (IsPlaying)
            {
                if (_list.Count > 0)
                {
                    if (_index < _list.Count)
                    {

                        Playing(_list[_index], _index);
                        _index += 1;
                    }
                    else
                    {
                        this.Stop();
                    }
                }
                await Task.Delay(Speed);
            }
        }
    }

    public void Pause()
    {
        if (_list.Any() && IsPlaying)
        {
            _paused = true;
            IsPlaying = false;
        }
    }

    public void Stop()
    {
        if (_list.Any())
        {
            _index = 0;
            _paused = false;
            IsPlaying = false;
            Stopped();
        }
    }
}