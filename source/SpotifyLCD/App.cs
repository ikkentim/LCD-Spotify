using System;
using System.Drawing;
using LogiFrame;
using LogiFrame.Components;
using SpotifyAPI;
using SpotifyLCD.Properties;
using Size = LogiFrame.Size;

namespace SpotifyLCD
{
    class App
    {
        private readonly Spotify _spotify;
        private readonly Frame _frame;
        private readonly Picture _state;
        private readonly Marquee _track;
        private readonly Marquee _artist;
        private readonly Marquee _album;
        private readonly ProgressBar _playTime;

        public App()
        {
            _frame = new Frame("Spotify", true, true, false);

            _spotify=new Spotify();

            _state = new Picture
            {
                Image = Resources.Stop,
                AutoSize = true,
                Location = new Location(2, Frame.LCDSize.Height - Resources.Stop.Height - 2)
            };

            _track = new Marquee
            {
                Font = new Font("Arial", 7.5f),
                Location = new Location(0, 0),
                Size = new Size(Frame.LCDSize.Width, 11),
                UseCache = true,
                EndSteps = 5,
                MarqueeStyle = MarqueeStyle.Visibility
            };

            _artist = new Marquee
            {
                Font = new Font("Arial", 7.5f),
                Location = new Location(0, 11),
                Size = new Size(Frame.LCDSize.Width, 11),
                UseCache = true,
                EndSteps = 5,
                MarqueeStyle = MarqueeStyle.Visibility
            };

            _album = new Marquee
            {
                Font = new Font("Arial", 7.5f, FontStyle.Bold),
                Location = new Location(0, 22),
                Size = new Size(Frame.LCDSize.Width, 11),
                Interval = 500,
                StepSize = 4,
                EndSteps = 12,
                UseCache = true,
                MarqueeStyle = MarqueeStyle.Visibility,
                Run = true,
                SyncedMarquees = { _artist, _track }
            };

            _playTime = new ProgressBar
            {
                Horizontal = true,
                MinimumValue = 0,
                MaximumValue = 60,
                Value = 50,
                Location = new Location(Resources.Stop.Width + + 4, Frame.LCDSize.Height - 8 - 2),
                Size = new Size(Frame.LCDSize.Width - Resources.Stop.Width - 4 - 2, 8),
                ProgressBarStyle = ProgressBarStyle.Border
                
            };

            _frame.Components.Add(_state);
            _frame.Components.Add(_track);
            _frame.Components.Add(_artist);
            _frame.Components.Add(_album);
            _frame.Components.Add(_playTime);

            _spotify.AutoUpdate(600);

            _spotify.AvailabilityChanged += _spotify_AvailabilityChanged;
            _spotify.PlayStateChanged +=_spotify_PlayStateChanged;
            _spotify.PlayingTimeChanged += _spotify_PlayingTimeChanged;
            _spotify.TrackChanged += _spotify_TrackChanged;

            UpdateVisibility();

            if (!_spotify.IsAvailable) return;

            UpdateTrack();
            UpdatePlayTime();
            UpdateStatePicture();
        }

        public void WaitForClose()
        {
            _frame.WaitForClose();
        }

        void _spotify_TrackChanged(object sender, EventArgs e)
        {
            UpdateTrack();
        }

        void _spotify_PlayingTimeChanged(object sender, EventArgs e)
        {
            UpdatePlayTime();
        }

        void _spotify_PlayStateChanged(object sender, EventArgs e)
        {
            UpdateStatePicture();
        }

        void _spotify_AvailabilityChanged(object sender, EventArgs e)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            _frame.UpdatePriority = _spotify.IsAvailable ? UpdatePriority.Normal : UpdatePriority.IdleNoShow;
        }

        private void UpdateTrack()
        {
            var track = _spotify.CurrentTrack;

            _track.Text = track == null ? string.Empty : track.Name;
            _artist.Text = track == null ? string.Empty : track.Artist.Name;
            _album.Text = track == null ? string.Empty : track.Album.Name;

        }

        private void UpdatePlayTime()
        {
            var track = _spotify.CurrentTrack;

            _playTime.Value = (float)_spotify.PlayingTime;
            _playTime.MaximumValue = track == null ? 1 : (float)track.Length/1000;
        }

        private void UpdateStatePicture()
        {
            _state.Image = _spotify.IsPlayEnabled
                ? _spotify.IsPlaying ? Resources.Play : Resources.Pause
                : Resources.Stop;
        }
    }
}