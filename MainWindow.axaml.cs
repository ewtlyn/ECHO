using Avalonia.Controls;
using Avalonia.Media;
using ECHO.Views;

namespace ECHO;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ShowHome();
    }

    private void ShowHome()
    {
        MainContent.Content = new HomeView();
        SetActiveButton(HomeButton, true);
        SetActiveButton(GalleryButton, false);
        SetActiveButton(ProfileButton, false);
    }

    private void ShowGallery()
    {
        MainContent.Content = new GalleryView();
        SetActiveButton(HomeButton, false);
        SetActiveButton(GalleryButton, true);
        SetActiveButton(ProfileButton, false);
    }

    private void ShowProfile()
    {
        MainContent.Content = new ProfileView();
        SetActiveButton(HomeButton, false);
        SetActiveButton(GalleryButton, false);
        SetActiveButton(ProfileButton, true);
    }

    private void SetActiveButton(Button button, bool isActive)
    {
        if (isActive)
        {
            button.Background = new SolidColorBrush(Color.Parse("#3D2F69"));
        }
        else
        {
            button.Background = Brushes.Transparent;
        }
    }
    
    private void HomeButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ShowHome();
    }

    private void GalleryButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ShowGallery();
    }

    private void ProfileButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ShowProfile();
    }
}