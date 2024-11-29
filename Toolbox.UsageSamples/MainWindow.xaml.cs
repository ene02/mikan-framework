using System.Windows;
using Microsoft.Win32;
using Toolbox.IO;

namespace Toolbox.Tests;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly Toolbox.Audio.SoundPlayer _soundPlayer = new Toolbox.Audio.SoundPlayer();
    private bool _timeSliderPressed = false;
    private string _filepath;

    public MainWindow()
    {
        InitializeComponent();
        MusicPlayerSelectionButton.Click += MusicPlayerSelectionButton_Click;
        VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
        PositionSlider.PreviewMouseLeftButtonDown += PositionSlider_PreviewMouseLeftButtonDown;
        PositionSlider.PreviewMouseLeftButtonUp += PositionSlider_PreviewMouseLeftButtonUp;
        PositionSlider.ValueChanged += PositionSlider_ValueChanged;
        PanningSlider.ValueChanged += PanningSlider_ValueChanged;
        SpeedSlider.ValueChanged += SpeedSlider_ValueChanged;
        PitchSlider.ValueChanged += PitchSlider_ValueChanged;

        _soundPlayer.PlaybackEnded += (s, e) =>
        {
            Dispatcher.Invoke(() =>
            {
                TrackTime.Text = "(0:00 - 0:00)";
            });
        };
    }

    private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _soundPlayer.SetSpeed((float)e.NewValue);
        VelocidadTitle.Text = $"Velocidad ({Math.Round(e.NewValue, 1)})";
    }

    private void PitchSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _soundPlayer.SetPitch((float)e.NewValue);
        PitchTitle.Text = $"Agudez ({Math.Round(e.NewValue, 1)})";
    }

    private void PanningSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _soundPlayer.SetPanning((float)e.NewValue);
        PanningTitle.Text = $"Lado ({Math.Round(e.NewValue, 1)})";
    }

    private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_soundPlayer.IsPlaying) // If the song already ended (disposed)...
        {
            _soundPlayer.Play(_filepath); // restart song before applying position changes.
        }

        if (_timeSliderPressed)
        {
            _soundPlayer.SetPosition((int)e.NewValue * 1000);
        }

        TrackTime.Text = $"({ConvertToMinAndSec(e.NewValue)} - {ConvertToMinAndSec(_soundPlayer.AudioLenght)})";
    }

    private void PositionSlider_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _timeSliderPressed = false;
        TrackTime.FontWeight = FontWeights.Normal;
    }

    private void PositionSlider_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _timeSliderPressed = true;
        TrackTime.FontWeight = FontWeights.Bold;
    }

    private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _soundPlayer.SetVolume(e.NewValue);
        TrackVolume.Text = $"Volumen (%{Math.Round(e.NewValue * 100, 0)})";
    }

    private void MusicPlayerSelectionButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new()
        {
            CheckFileExists = true,
            ShowHiddenItems = true,
            Filter = "mp3|*.mp3|All files|*.*"
        };

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            _filepath = dialog.FileName;

            _soundPlayer.Play(_filepath);
            TrackTitle.Text = FilenameParser.Get(_filepath);
            TrackTime.Text = $"(0:00 - {ConvertToMinAndSec(_soundPlayer.AudioLenght)})";
            PositionSlider.Maximum = _soundPlayer.AudioLenght;
            MonitorTrackPosition();
        }
    }

    private async Task MonitorTrackPosition()
    {
        await Task.Run(async () =>
        {
            while (_soundPlayer.IsPlaying)
            {
                Dispatcher.Invoke(() =>
                {
                    if (!_timeSliderPressed)
                    {
                        TrackTime.Text = $"({ConvertToMinAndSec(_soundPlayer.GetPosition())} - {ConvertToMinAndSec(_soundPlayer.AudioLenght)})";
                        PositionSlider.Value = _soundPlayer.GetPosition();
                    }
                });

                await Task.Delay(100);
            }
        });
    }

    private string ConvertToMinAndSec(double seconds)
    {
        int min = (int)seconds / 60;
        int sec = (int)seconds % 60;

        if (sec <= 9)
        {
            return $"{min}:0{sec}";
        }

        return $"{min}:{sec}";
    }
}