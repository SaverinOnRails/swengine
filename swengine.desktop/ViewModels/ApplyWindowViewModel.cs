using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Data;
using AvaloniaMpv;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using swengine.desktop.Helpers;
using swengine.desktop.Models;
using swengine.desktop.Services;

namespace swengine.desktop.ViewModels;

public partial class ApplyWindowViewModel : ViewModelBase {
    //get search results from previous window
    private WallpaperResponse _wallpaperResponse;

    //MotionBgs service
    public IBgsProvider BgsProvider { get; set; }

    public string Backend { get; set; }
    public MpvPlayer Player => Singleton.Player;
    //Resolution user selected. Defaults to 4k.
    [ObservableProperty]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ApplyWindowViewModel))]
    private GifQuality selectedResolution = GifQuality.q2160p;

    //duration selected by user
    [ObservableProperty] private int selectedDuration = 5;
    //FPS user selected for GIF
    [ObservableProperty] private string selectedFps = "60";

    //whether to use the best settings for a particular wallpaper

    [ObservableProperty]
    private bool bestSettings = true;

    [ObservableProperty] private ApplicationStatusWrapper applicationStatusWrapper = new();

    //Initialize Native Libvlc client for playing the wallpaper preview

    public ApplyWindowViewModel() {
    }

    //The wallpaper object that will be gotten from the Bg service after it has obtained information about the wallpaper.
    [ObservableProperty] private Wallpaper wallpaper;
    public WallpaperResponse WallpaperResponse {
        get { return _wallpaperResponse; }
        set {
            SetProperty(ref _wallpaperResponse, value);
            ObjectCreated();
        }
    }

    //Called when the WallpaperResponse object is set while the window is opening
    public async void ObjectCreated() {
        try {
            Wallpaper = await BgsProvider.InfoAsync(WallpaperResponse.Src, Title: WallpaperResponse.Title);
            // using var media = new Media(_libVlc, new Uri(Wallpaper.Preview));
            // MediaPlayer.Play(media);
            // MediaPlayer.Volume = 0;
            try {
                Player.MpvCommand(new string[] { "set", "mute", "yes" });
                Player.MpvCommand(new string[] { "set", "loop-file", "inf" });
                Player.MpvCommand(new string[] { "set", "pause", "no" });
            } catch { }
            Player.StartPlayback(Wallpaper.Preview);
        } catch { }
    }
    public async void ApplyWallpaper() {
        if (Wallpaper == null) {
            ContentDialog warningDialog = new() {
                Title = "Warning",
                Content = "Wallpaper information is still loading. Please try again",
                CloseButtonText = "Dismiss"
            };
            await warningDialog.ShowAsync();
            return;
        }
        ContentDialog dialog = new() {
            Title = "Apply this wallpaper",
            PrimaryButtonText = "Apply",
            IsPrimaryButtonEnabled = true,
            Content = ApplyDialogContent()
        };
        var dialogResponse = await dialog.ShowAsync();
        Player.MpvCommand(new string[] { "set", "pause", "yes" });
        if (dialogResponse == ContentDialogResult.Primary) {
            dialog.Hide();
            var applicationStatusDialog = new ContentDialog() {
                Title = "Applying Wallpaper",
                CloseButtonText = "Stop"
            };
            applicationStatusDialog.Bind(ContentDialog.ContentProperty, new Binding() {
                Path = "ApplicationStatusWrapper.Status",
                Source = this,
                Mode = BindingMode.TwoWay,
            });
            CancellationTokenSource ctx = new();
            applicationStatusDialog.Opened += (sender, args) => {
                Task.Run(() => {
                    WallpaperHelper.ApplyWallpaperAsync(
                    wallpaper: Wallpaper,
                    applicationStatusWrapper: ApplicationStatusWrapper,
                    selectedResolution: SelectedResolution,
                    selectedFps: SelectedFps,
                    selectedDuration: SelectedDuration,
                    bestSettings: BestSettings,
                    backend: Backend,
                    token: ctx.Token,
                    referrer: WallpaperResponse.Src);
                });
            };
            applicationStatusDialog.Closed += (sender, args) => {
                ctx.Cancel();
            };
            await applicationStatusDialog.ShowAsync();
        }
    }
}
public class DesignApplyWindowViewModel : ApplyWindowViewModel {
    public DesignApplyWindowViewModel() {
        Wallpaper = new() {
            Title = "Garp With Galaxy Impact",
            Preview = "https://www.motionbgs.com/media/6384/garp-with-galaxy-impact.960x540.mp4",
            WallpaperType = WallpaperType.Live,
            Resolution = "Resolution\":\"3840x2160"
        };

    }
}

//wrapper class for ApplicationStatus
public partial class ApplicationStatusWrapper : ObservableObject {
    [ObservableProperty]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ApplicationStatusWrapper))]
    private string status;
}
