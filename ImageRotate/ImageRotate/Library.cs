using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

public class Library
{
    private bool _rotating = false;
    private Storyboard _rotation = new Storyboard();

    public void Rotate(string axis, ref Image target)
    {
        if (_rotating)
        {
            _rotation.Stop();
            _rotating = false;
        }
        else
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0.0,
                To = 360.0,
                BeginTime = TimeSpan.FromSeconds(1),
                RepeatBehavior = RepeatBehavior.Forever
            };
            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, "(UIElement.Projection).(PlaneProjection.Rotation" + axis + ")");
            _rotation.Children.Clear();
            _rotation.Children.Add(animation);
            _rotation.Begin();
            _rotating = true;
        }
    }
}