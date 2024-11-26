using System.Windows;
using Microsoft.Win32;

namespace Toolbox.Tests;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly Toolbox.Audio.SoundPlayer _soundPlayer = new Toolbox.Audio.SoundPlayer();
    
    public MainWindow()
    {
        InitializeComponent();
        MusicPlayerSelectionButton.Click += MusicPlayerSelectionButton_Click;
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
            _soundPlayer.Play(dialog.FileName);
        }
    }
}