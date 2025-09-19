using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using swengine.desktop.ViewModels;

namespace swengine.desktop.Views;

public partial class ApplyWindow : Window
{
    public ApplyWindow()
    {
        InitializeComponent();
        // video.Loaded += ((sender, args) =>
        // {
        //     var datacontext = DataContext as ApplyWindowViewModel;
        //    video.MediaPlayer = datacontext.MediaPlayer;
        // });
        Closing += (sender, args) =>
        {
            //stop all players
           ( DataContext as ApplyWindowViewModel).Player.MpvCommand(new[] {"stop"});
          
        };
    }

    private void ApplyWallpaper(object? sender, RoutedEventArgs e)
    {
        var dataContext = DataContext as ApplyWindowViewModel;
        
    }
}
