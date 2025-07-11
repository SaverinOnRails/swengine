using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using AvaloniaEdit.Document;

using CommunityToolkit.Mvvm.ComponentModel;
using AsyncImageLoader.Loaders;
using swengine.desktop.Models;
using swengine.desktop.Services;

namespace swengine.desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    public MainWindowViewModel() {
        //set provider initially to motionbgs.com
        SetProvider();
        Search();
        RequestMoveToTop += RequestMoveToTop;
        RequestClearImageLoader += RequestClearImageLoader;
    }

    public IBgsProvider BgsProvider;
    public string[] Providers => new[] {
        "Motionbgs.com",
        "Moewalls.com",
        "Desktophut.com",
        "Wallhaven.cc",
        "Wallhaven.cc(random)",
        "Wallpaperscraft.com",
        "Wallpapers-clan.com" , };

    public string[] Backends => new[] { "SWWW", "YIN", "PLASMA", "GNOME" };
    private bool _appendingToInfinteScroll = false;

    private string _selectedProvider = "Motionbgs.com";
    private string _selectedBackend = "SWWW";
    public event EventHandler<EventArgs> RequestMoveToTop;
    public event EventHandler<EventArgs> RequestClearImageLoader;

    private CancellationTokenSource _searchDebounceToken = new();
    public AsyncImageLoader.Loaders.BaseWebImageLoader BaseLoader => new BaseWebImageLoader();

    private int _infiniteScrollPage = 1;
    public string SelectedProvider {
        get => _selectedProvider;
        set {
            SetProperty(ref _selectedProvider, value);
            SetProvider();
        }
    }
    public string SelectedBackend {
        get => _selectedBackend;
        set {
            SetProperty(ref _selectedBackend, value);
            SetBackend();
        }
    }

    [ObservableProperty]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(MainWindowViewModel))]
    private string searchTerm = "";

    //current page
    [ObservableProperty] private int currentPage = 1;
    [ObservableProperty] private List<WallpaperResponse> wallpaperResponses;
    [ObservableProperty] private bool dataLoading = false;

    [ObservableProperty] private string selectedFile = null;

    [ObservableProperty]
    private TextDocument customScriptsContent = new() {
        Text = ""
    };
    [ObservableProperty] public bool infinteScrollLoading = false;
    // async void GetWallpapers()
    // {
    //     DataLoading = true;
    //     WallpaperResponses = await BgsProvider.LatestAsync(Page: CurrentPage);
    //     DataLoading = false;
    // }

    private void SetProvider() {
        switch (SelectedProvider) {
            case "Motionbgs.com":
                BgsProvider = new MotionBgsService();
                break;
            case "Moewalls.com":
                BgsProvider = new MoewallsService();

                break;
            case "Wallhaven.cc":
                BgsProvider = new WallHavenService();
                break;
            case "Mylivewallpapers.com":
                BgsProvider = new MyLiveWallpapersService();
                break;
            case "Wallpaperscraft.com":
                BgsProvider = new WallpapersCraftService();
                break;
            case "Wallpapers-clan.com":
                BgsProvider = new WallpapersClanService();
                break;
            case "Wallhaven.cc(random)":
                BgsProvider = new WallHavenRandomService();
                break;
            case "Desktophut.com":
                BgsProvider = new DesktopHutService();
                break;
            default:
                break;
        }
        //changing provider should reset _infinteScrollPage to the current page
        //
        _infiniteScrollPage = CurrentPage;
        _appendingToInfinteScroll = false;
        RequestMoveToTop?.Invoke(this, EventArgs.Empty);
        try {
            ClearImageLoader();
        } catch { }
        Search();
    }
    private void SetBackend() {

    }
    public void Paginate(string seek) {
        if (seek == "up") {
            CurrentPage++;
        } else if (seek == "down" && CurrentPage > 1) {
            CurrentPage--;
        }
        _infiniteScrollPage = CurrentPage;
        _appendingToInfinteScroll = false;
        ClearImageLoader();
        RequestMoveToTop.Invoke(this, EventArgs.Empty);
        Search();
    }
    public async void Search() {
        //cancel previoud requests
        await _searchDebounceToken.CancelAsync();
        _searchDebounceToken = new();
        DataLoading = true;

        if (_searchDebounceToken.IsCancellationRequested) return;
        if (SearchTerm.Length == 0) {
            //empty search
            if (_searchDebounceToken.IsCancellationRequested) return;
            WallpaperResponses = await BgsProvider.LatestAsync(Page: CurrentPage);
            DataLoading = false;
            return;
        }
        if (_searchDebounceToken.IsCancellationRequested) return;
        WallpaperResponses = await BgsProvider.SearchAsync(SearchTerm, CurrentPage);
        DataLoading = false;

    }
    private void ClearImageLoader() {
        RequestClearImageLoader.Invoke(this, EventArgs.Empty);
    }
    public async void AppendToInfinteScroll() {
        if (_appendingToInfinteScroll) return;

        _appendingToInfinteScroll = true;
        InfinteScrollLoading = true;

        try {
            var responses = SearchTerm.Length == 0
                ? await BgsProvider?.LatestAsync(_infiniteScrollPage + 1)
                : await BgsProvider?.SearchAsync(SearchTerm, _infiniteScrollPage + 1);

            if (responses != null) {
                foreach (var response in responses) {
                    if (response != null) {
                        WallpaperResponses.Add(response);
                    }
                }
                _infiniteScrollPage++;
            }
        } catch {
        } finally {
            InfinteScrollLoading = false;
            _appendingToInfinteScroll = false;
        }
    }
}
