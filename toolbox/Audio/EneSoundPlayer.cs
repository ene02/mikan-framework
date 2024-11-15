using NAudio.Wave;

namespace Toolbox.Audio
{
    /// <summary>
    /// A simple WasapiOut music player.
    /// </summary>
    public class EneSoundPlayer : IDisposable
    {
        /// <summary>
        /// Event for volume changes.
        /// </summary>
        public EventHandler<VolumeChangedEvent>? VolumeChanged;

        /// <summary>
        /// Event for playback ending.
        /// </summary>
        public EventHandler? PlaybackStopped;

        private WaveOutEvent _mainOutput;
        private AudioFileReader _mainAudio;
        private float _audioVolume = 1;
        private bool _disposed = false;
        private string _fileName = string.Empty;

        /// <summary>
        /// Current playback state.
        /// </summary>
        public PlaybackState PlaybackState
        {
            get { return _mainOutput.PlaybackState; }
        }

        /// <summary>
        /// Current volume of this player.
        /// </summary>
        public float CurrentVolume
        {
            get { return _audioVolume; }
        }

        /// <summary>
        /// The file name of the last played file.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
        }

        public EneSoundPlayer()
        {
            _mainOutput = new();
            _mainOutput.PlaybackStopped += _mainOutput_PlaybackStopped;
        }

        private void _mainOutput_PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            PlaybackStopped?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Releases all resources used by MusicPlayer.
        /// </summary>
        public void Dispose()
        {
            // Pretty sure NAudio takes care of a lot of stuff, but i added a check anyway just in case.
            if (!_disposed) return;

            _mainAudio?.Dispose();
            _mainAudio?.Close();
            _mainOutput?.Dispose();
            _disposed = false;
            _fileName = string.Empty;

        _disposed = true;
        }

        /// <summary>
        /// Play a song on the main output.
        /// </summary>
        /// <param name="audioFilePath"></param>
        public void Play(string audioFilePath)
        {
            _disposed = false; 

            if (string.IsNullOrEmpty(audioFilePath) || !File.Exists(audioFilePath))
                throw new ArgumentException("Invalid file path.");

            if (audioFilePath.Contains(".mp3")) // Not optimal but gucci for now.
            {
                _fileName = FilenameParser.Get(audioFilePath);
            }
            else
            {
                _fileName = string.Empty;
            }

            // Stop current instances and dispose them if they exist.
            if (_mainOutput != null && _mainOutput.PlaybackState == PlaybackState.Playing)
            {
                _mainOutput.Stop();
                _mainAudio.Dispose();
                _mainAudio.Close();
            }

            LoadAudioToOutput(audioFilePath);
        }

        public void Stop()
        {
            _mainOutput.Stop();
        }

        // Loads the song in async, to prevent main thread blocks in case the file selected is large.
        private async Task LoadAudioToOutput(string audioFilePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Set the instance for new file playback.
                    _mainAudio = new(audioFilePath);

                    ChangeAudioVolume(_audioVolume);

                    _mainOutput.Init(_mainAudio);
                    _mainOutput.Play();
                }
                catch
                {
                    ChangeAudioVolume(_audioVolume); // Save volume anyways.
                }
            });
        }

        /// <summary>
        /// Change the volume of the main output.
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="output"></param>
        public void ChangeAudioVolume(float volume)
        {
            if (_disposed) return;

            if (_mainAudio != null) _mainAudio.Volume = volume;

            _audioVolume = volume;

            VolumeChanged?.Invoke(this, new VolumeChangedEvent(volume));
        }
    }
}
