using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

public class Library
{
    private readonly Dictionary<string, double> _notes = new Dictionary<string, double>()
    {
        { "C", 261.6 }, { "C#", 277.2 }, { "D", 293.7 },
        { "D#", 311.1 } , { "E", 329.6 }, { "F", 349.2 },
        { "F#", 370.0 }, { "G", 392.0 }, { "G#", 415.3 },
        { "A", 440.0 }, { "A#", 466.2 }, { "B", 493.9 }
    };

    private void Play(double note)
    {
        MediaElement playback = new MediaElement();
        IRandomAccessStream stream = new InMemoryRandomAccessStream();
        BinaryWriter writer = new BinaryWriter(stream.AsStream());
        int formatChunkSize = 16;
        int headerSize = 8;
        short formatType = 1;
        short tracks = 1;
        int samplesPerSecond = 44100;
        short bitsPerSample = 16;
        short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
        int bytesPerSecond = samplesPerSecond * frameSize;
        int waveSize = 4;
        int data = 0x61746164;
        int samples = 88200 * 4;
        int dataChunkSize = samples * frameSize;
        int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
        double frequency = note * 1.5;
        writer.Write(0x46464952); // RIFF
        writer.Write(fileSize);
        writer.Write(0x45564157); // WAVE
        writer.Write(0x20746D66); // Format
        writer.Write(formatChunkSize);
        writer.Write(formatType);
        writer.Write(tracks);
        writer.Write(samplesPerSecond);
        writer.Write(bytesPerSecond);
        writer.Write(frameSize);
        writer.Write(bitsPerSample);
        writer.Write(data);
        writer.Write(dataChunkSize);
        for (int i = 0; i < samples / 4; i++)
        {
            double t = (double)i / (double)samplesPerSecond;
            short s = (short)(10000 * (Math.Sin(t * frequency * 2.0 * Math.PI)));
            writer.Write(s);
        }
        stream.Seek(0);
        playback.SetSource(stream, "audio/wav");
        playback.Play();
    }

    private void Add(Grid grid, int column)
    {
        Button button = new Button()
        {
            Height = 80,
            Width = 20,
            FontSize = 10,
            Padding = new Thickness(0),
            Content = _notes.Keys.ElementAt(column),
            Margin = new Thickness(5)
        };
        button.Click += (object sender, RoutedEventArgs e) =>
        {
            button = (Button)sender;
            int note = Grid.GetColumn(button);
            Play(_notes[_notes.Keys.ElementAt(note)]);
        };
        button.SetValue(Grid.ColumnProperty, column);
        grid.Children.Add(button);
    }

    private void Layout(ref Grid Grid)
    {
        Grid.Children.Clear();
        Grid.ColumnDefinitions.Clear();
        Grid.RowDefinitions.Clear();
        // Setup Grid
        for (int Column = 0; (Column < _notes.Count); Column++)
        {
            Grid.ColumnDefinitions.Add(new ColumnDefinition());
            Add(Grid, Column);
        }
    }

    public void New(ref Grid grid)
    {
        Layout(ref grid);
    }
}