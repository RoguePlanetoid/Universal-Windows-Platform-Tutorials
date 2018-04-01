using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace ExpandControl
{
    public sealed class ExpandPanel : Control
    {
        public ExpandPanel()
        {
            this.DefaultStyleKey = typeof(ExpandPanel);
        }

        private bool _useTransitions = true;
        private VisualState _collapsedState;
        private Windows.UI.Xaml.Controls.Primitives.ToggleButton _toggleExpander;
        private FrameworkElement _contentElement;

        public static readonly DependencyProperty HeaderContentProperty =
        DependencyProperty.Register("HeaderContent", typeof(object),
        typeof(ExpandPanel), null);

        public static readonly DependencyProperty MainContentProperty =
        DependencyProperty.Register("MainContent", typeof(object),
        typeof(ExpandPanel), null);

        public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register("IsExpanded", typeof(bool),
        typeof(ExpandPanel), new PropertyMetadata(true));

        public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
        typeof(ExpandPanel), null);

        public object HeaderContent
        {
            get { return GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        public object MainContent
        {
            get { return GetValue(MainContentProperty); }
            set { SetValue(MainContentProperty, value); }
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        private void ChangeVisualState(bool useTransitions)
        {
            if (IsExpanded)
            {
                if (_contentElement != null)
                {
                    _contentElement.Visibility = Visibility.Visible;
                }
                VisualStateManager.GoToState(this, "Expanded", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Collapsed", useTransitions);
                _collapsedState = (VisualState)GetTemplateChild("Collapsed");
                if (_collapsedState == null)
                {
                    if (_contentElement != null)
                    {
                        _contentElement.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _toggleExpander = (Windows.UI.Xaml.Controls.Primitives.ToggleButton)
                GetTemplateChild("ExpandCollapseButton");
            if (_toggleExpander != null)
            {
                _toggleExpander.Click += (object sender, RoutedEventArgs e) =>
                {
                    IsExpanded = !IsExpanded;
                    _toggleExpander.IsChecked = IsExpanded;
                    ChangeVisualState(_useTransitions);
                };
            }
            _contentElement = (FrameworkElement)GetTemplateChild("Content");
            if (_contentElement != null)
            {
                _collapsedState = (VisualState)GetTemplateChild("Collapsed");
                if ((_collapsedState != null) && (_collapsedState.Storyboard != null))
                {
                    _collapsedState.Storyboard.Completed += (object sender, object e) =>
                    {
                        _contentElement.Visibility = Visibility.Collapsed;
                    };
                }
            }
            ChangeVisualState(false);
        }
    }
}
