using Microsoft.Win32;
using NAudio.Gui;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Toolbox.Audio;

namespace Toolbox.Tests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EneSoundPlayer simplePlayer;

        public MainWindow()
        {
            InitializeComponent();

            simplePlayer = new();

            MusicPlayerSelectionButton.Click += MusicPlayerSelectionButton_Click;
            VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            simplePlayer.ChangeAudioVolume((float)VolumeSlider.Value);
            TrackVolume.Text = $"Volume (%{VolumeSlider.Value * 100})";
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
                simplePlayer.Play(dialog.FileName);

                if (simplePlayer.FileName == string.Empty)
                {
                    TrackTitle.Text = "File has no name or is invalid!";
                }
                else
                {
                    TrackTitle.Text = $"{simplePlayer.FileName}";
                }
            }
        }
    }
}