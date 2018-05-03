using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace JsonfileApp
{
    public class Music
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _album;
        private string _artist;
        private string _genre;

        public Music() { Id = Guid.NewGuid().ToString(); }

        public string Id { get; set; }
        public string Album { get { return _album; } set { _album = value; NotifyPropertyChanged(); } }
        public string Artist { get { return _artist; } set { _artist = value; NotifyPropertyChanged(); } }
        public string Genre { get { return _genre; } set { _genre = value; NotifyPropertyChanged(); } }
    }

    public class Library
    {
        private const string file_name = "file.json";

        private StorageFile _file;

        public static ObservableCollection<Music> Collection { get; private set; } = new ObservableCollection<Music>();

        private async void Read()
        {
            try
            {
                _file = await ApplicationData.Current.LocalFolder.GetFileAsync(file_name);
                using (Stream stream = await _file.OpenStreamForReadAsync())
                {
                    Collection = (ObservableCollection<Music>)
                        new DataContractJsonSerializer(typeof(ObservableCollection<Music>))
                        .ReadObject(stream);
                }
            }
            catch
            {
            }
        }

        private async void Write()
        {
            try
            {
                _file = await ApplicationData.Current.LocalFolder.CreateFileAsync(file_name,
                    CreationCollisionOption.ReplaceExisting);
                using (Stream stream = await _file.OpenStreamForWriteAsync())
                {
                    new DataContractJsonSerializer(typeof(ObservableCollection<Music>))
                        .WriteObject(stream, Collection);
                }
            }
            catch
            {
            }
        }

        public Library()
        {
            Read();
        }

        public void Add(FlipView display)
        {

            Collection.Insert(0, new Music());
            display.SelectedIndex = 0;
        }

        public void Save()
        {
            Write();
        }

        public void Remove(FlipView display)
        {
            if (display.SelectedItem != null)
            {
                Collection.Remove(Collection.Where(w => w.Id ==
                ((Music)display.SelectedValue).Id).Single());
                Write();
            }
        }

        public async void Delete(FlipView display)
        {
            try
            {
                Collection = new ObservableCollection<Music>();
                display.ItemsSource = Collection;
                _file = await ApplicationData.Current.LocalFolder.GetFileAsync(file_name);
                await _file.DeleteAsync();
            }
            catch
            {

            }
        }
    }
}