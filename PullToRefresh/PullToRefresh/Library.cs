using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace PullToRefresh
{
    public class PullToRefreshData
    {
        public Brush Fill { get; set; }
        public string Date { get; set; }
    }

    public class Library
    {
        private readonly List<Color> _colours = typeof(Colors)
        .GetRuntimeProperties()
        .Select(c => (Color)c.GetValue(null)).ToList();

        private ObservableCollection<PullToRefreshData> _list
        = new ObservableCollection<PullToRefreshData>();

        private PullToRefreshData GetNext()
        {
            return new PullToRefreshData()
            {
                Fill = new SolidColorBrush(_colours[DateTime.Now.Second]),
                Date = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
            };
        }

        private async Task FetchAsync(int count)
        {
            for (int i = 0; i < count; i++)
            {
                await Task.Delay(1000);
                _list.Insert(0, GetNext());
            }
        }

        private async void RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            using (var deferral = args.GetDeferral())
            {
                await FetchAsync(4);
            }
        }

        public async void Init(RefreshContainer container, ListView display)
        {
            display.ItemsSource = _list;
            container.RefreshRequested += RefreshRequested;
            await FetchAsync(2);
        }

        public void Refresh(ref RefreshContainer container)
        {
            container.RequestRefresh();
        }
    }
}