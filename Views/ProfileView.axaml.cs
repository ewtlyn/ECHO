using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using ECHO.Context;

namespace ECHO.Views;

public partial class ProfileView : UserControl
{
    public ProfileView()
    {
        InitializeComponent();
        LoadProfile();
    }

    private void LoadProfile()
    {
        if (UserSession.CurrentUser == null)
            return;

        PostsPanel.Children.Clear();
        using var db = new PostgresContext();

        var currentUser = db.Users.FirstOrDefault(u => u.UserId == UserSession.CurrentUser.UserId);
        if (currentUser == null)
            return;

        UsernameText.Text = currentUser.Username;
        CreatedAtText.Text = $"Registered: {currentUser.CreatedAt:dd.MM.yyyy}";

        var userPosts = db.Posts
            .Where(p => p.UserId == currentUser.UserId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        PostsCountText.Text = $"Posts: {userPosts.Count}";

        if (!string.IsNullOrWhiteSpace(currentUser.AvatarPath) && File.Exists(currentUser.AvatarPath))
        {
            using var avatarStream = File.OpenRead(currentUser.AvatarPath);
            AvatarImage.Source = new Bitmap(avatarStream);
        }

        PostsPanel.Children.Clear();

        if (userPosts.Count == 0)
        {
            PostsPanel.Children.Add(new TextBlock
            {
                Text = "У вас пока нет постов",
                FontSize = 14,
                Foreground = Avalonia.Media.Brushes.Gray
            });

            return;
        }

        foreach (var post in userPosts)
        {
            PostsPanel.Children.Add(CreatePostCard(currentUser.Username!, currentUser.AvatarPath, post));
        }
    }

    private Border CreatePostCard(string username, string? avatarPath, ECHO.Models.Post post)
    {
        var root = new Border
        {
            Background = Avalonia.Media.Brush.Parse("#1A1A1F"),
            BorderBrush = Avalonia.Media.Brush.Parse("#2A2A31"),
            BorderThickness = new Avalonia.Thickness(1),
            CornerRadius = new Avalonia.CornerRadius(16),
            Padding = new Avalonia.Thickness(22, 18),
            MaxWidth = 780
        };

        var mainPanel = new StackPanel
        {
            Spacing = 16
        };

        var headerGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*")
        };

        var avatarBorder = new Border
        {
            Width = 46,
            Height = 46,
            CornerRadius = new Avalonia.CornerRadius(23),
            Background = Avalonia.Media.Brushes.LightGray,
            ClipToBounds = true
        };

        var avatarImage = new Image
        {
            Stretch = Avalonia.Media.Stretch.UniformToFill
        };

        if (!string.IsNullOrWhiteSpace(avatarPath) && File.Exists(avatarPath))
        {
            using var stream = File.OpenRead(avatarPath);
            avatarImage.Source = new Bitmap(stream);
        }

        avatarBorder.Child = avatarImage;

        var infoPanel = new StackPanel
        {
            Margin = new Avalonia.Thickness(14, 0, 0, 0),
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Spacing = 4
        };

        infoPanel.Children.Add(new TextBlock
        {
            Text = username,
            FontSize = 18,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            Foreground = Avalonia.Media.Brushes.White
        });

        infoPanel.Children.Add(new TextBlock
        {
            Text = post.CreatedAt.ToString("dd.MM.yy HH:mm"),
            FontSize = 11,
            Foreground = Avalonia.Media.Brush.Parse("#666872")
        });

        Grid.SetColumn(infoPanel, 1);

        headerGrid.Children.Add(avatarBorder);
        headerGrid.Children.Add(infoPanel);

        mainPanel.Children.Add(headerGrid);

        if (!string.IsNullOrWhiteSpace(post.ImagePath) && File.Exists(post.ImagePath))
        {
            var imageBorder = new Border
            {
                Width = 420,
                Height = 420,
                CornerRadius = new Avalonia.CornerRadius(6),
                ClipToBounds = true
            };

            var postImage = new Image
            {
                Stretch = Avalonia.Media.Stretch.UniformToFill
            };

            using var imageStream = File.OpenRead(post.ImagePath);
            postImage.Source = new Bitmap(imageStream);

            imageBorder.Child = postImage;
            mainPanel.Children.Add(imageBorder);
        }

        if (!string.IsNullOrWhiteSpace(post.Description))
        {
            mainPanel.Children.Add(new TextBlock
            {
                Text = post.Description,
                FontSize = 14,
                Foreground = Avalonia.Media.Brushes.White,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            });
        }

        root.Child = mainPanel;
        return root;
    }

    private void LogOut_Click(object? sender, RoutedEventArgs e)
    {
        UserSession.Logout();

        var authWindow = new AuthStartView();
        authWindow.Show();

        var currentWindow = TopLevel.GetTopLevel(this) as Window;
        currentWindow?.Close();
    }
    
    private async void NewPost_Click(object? sender, RoutedEventArgs e)
    {
        var window = new CreatePostWindow();

        var parentWindow = TopLevel.GetTopLevel(this) as Window;
        if (parentWindow != null)
            await window.ShowDialog(parentWindow);
        else
            window.Show();

        if (window.PostCreated)
        {
            LoadProfile();
        }
    }
    
}