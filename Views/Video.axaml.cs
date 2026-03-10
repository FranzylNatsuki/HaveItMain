using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using LibVLCSharp.Avalonia;

namespace HaveItMain.Views;

public partial class Video : Window
{
    private MediaPlayer mediaPlayer;
    private LibVLC libVLC;

    private string? videoPath;
    
    private string? LoadVideoPath()
    {
        try
        {
            var configPath = Path.Combine(
                AppContext.BaseDirectory,
                "video_path.txt");

            // 🔹 If config file doesn't exist, create it
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath,
                    "PUT_VIDEO_PATH_HERE");

                Console.WriteLine("Config file created.");
                return null;
            }

            var path = File.ReadAllText(configPath).Trim();
            return path;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading config: {ex.Message}");
            return null;
        }
    }

    public Video()
    {
        InitializeComponent();
        this.Opened += OnOpened;
        this.Closing += OnClosing;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Setup();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void Setup()
    {
        Core.Initialize();

        libVLC = new LibVLC("--avcodec-hw=none");
        mediaPlayer = new MediaPlayer(libVLC);

        var videoView = this.FindControl<VideoView>("VideoView");
        var playButton = this.FindControl<Button>("PlayButton");
        var pauseButton = this.FindControl<Button>("PauseButton");
        var stopButton = this.FindControl<Button>("StopButton");
        var replayButton = this.FindControl<Button>("ReplayButton");
        var volumeSlider = this.FindControl<Slider>("VolumeSlider");

        videoView.MediaPlayer = mediaPlayer;

        // 🔹 Load path from txt file
        videoPath = LoadVideoPath();

        if (string.IsNullOrWhiteSpace(videoPath) || !File.Exists(videoPath))
        {
            Console.WriteLine("Video file not found!");
            no_video();
            return;
        }

        // Load media
        var media = new Media(libVLC, videoPath, FromType.FromPath);
        mediaPlayer.Media = media;

        // Volume slider
        volumeSlider.Value = 100;
        volumeSlider.PropertyChanged += (s, e) =>
        {
            if (e.Property == Slider.ValueProperty)
                mediaPlayer.Volume = (int)volumeSlider.Value;
        };

        replayButton.Click += (s, e) =>
        {
            PlayVideo();
            playButton.IsVisible = false;
            pauseButton.IsVisible = true;
        };

        playButton.Click += (s, e) =>
        {
            if (!mediaPlayer.IsPlaying)
                mediaPlayer.Play();

            playButton.IsVisible = false;
            pauseButton.IsVisible = true;
        };

        pauseButton.Click += (s, e) =>
        {
            if (mediaPlayer.IsPlaying)
                mediaPlayer.Pause();

            pauseButton.IsVisible = false;
            playButton.IsVisible = true;
        };

        stopButton.Click += (s, e) =>
        {
            mediaPlayer.Stop();
            pauseButton.IsVisible = false;
            playButton.IsVisible = true;
        };

        mediaPlayer.EndReached += (s, e) =>
        {
            // Important: Don't call mediaPlayer.Stop() inside EndReached! 
            // It's already stopping.
    
            Dispatcher.UIThread.Post(() =>
            {
                // Reset the UI buttons
                pauseButton.IsVisible = false;
                playButton.IsVisible = true;
        
                // Seek back to the start so the user can just hit 'Play' again
                mediaPlayer.Position = 0; 
            });
        };
    }

    private async void no_video()
    {
        var dialog = new SimpleMessageDialog("Video file not found!");
        await dialog.ShowDialog(this); // 'this' = parent Window
    }

    private string? lastVideoPath;

    private void PlayVideo()
    {
        if (string.IsNullOrWhiteSpace(videoPath) || !File.Exists(videoPath))
        {
            no_video();
            return;
        }

        // 1. Fully Stop first
        mediaPlayer.Stop();

        // 2. Clear the current media safely
        var oldMedia = mediaPlayer.Media;
        mediaPlayer.Media = null;
        oldMedia?.Dispose();

        // 3. Create NEW media instance
        var media = new Media(libVLC, videoPath, FromType.FromPath);
    
        // 4. Play the new instance
        mediaPlayer.Play(media);
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (mediaPlayer != null)
        {
            // Stop playback
            if (mediaPlayer.IsPlaying)
                mediaPlayer.Stop();

            // Detach from VideoView to avoid handle crash
            var videoView = this.FindControl<VideoView>("VideoView");
            if (videoView != null)
                videoView.MediaPlayer = null;

            // Dispose Media object
            mediaPlayer.Media?.Dispose();

            // Dispose MediaPlayer and LibVLC
            mediaPlayer.Dispose();
            libVLC.Dispose();
        }
    }
}
