using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ECHO.Context;
using ECHO.Models;
using Microsoft.EntityFrameworkCore;

namespace ECHO.Views;

public partial class BoardDetailsWindow : Window
{
    private readonly int _boardId;

    public BoardDetailsWindow(int boardId)
    {
        InitializeComponent();
        _boardId = boardId;
        LoadBoard();
    }

    private void LoadBoard()
    {
        SavedPostsPanel.Children.Clear();

        using var db = new PostgresContext();

        var board = db.Boards
            .Include(b => b.BoardPosts)
            .ThenInclude(bp => bp.Post)
            .ThenInclude(p => p.User)
            .FirstOrDefault(b => b.BoardId == _boardId);

        if (board == null)
            return;

        BoardTitleText.Text = board.Title;
        BoardDescriptionText.Text = string.IsNullOrWhiteSpace(board.Description)
            ? "Your saved inspirations and mood references"
            : board.Description;

        var posts = board.BoardPosts
            .Select(bp => bp.Post)
            .ToList();

        BoardCountText.Text = $"{posts.Count} saved photos";

        if (posts.Count == 0)
        {
            SavedPostsPanel.Children.Add(CreateEmptyCard());
            return;
        }

        foreach (var post in posts)
        {
            SavedPostsPanel.Children.Add(CreateMoodboardCard(post));
        }
    }

    private Border CreateEmptyCard()
    {
        return new Border
        {
            Width = 280,
            Height = 340,
            Background = Brush.Parse("#13141A"),
            BorderBrush = Brush.Parse("#23242B"),
            BorderThickness = new Avalonia.Thickness(1),
            CornerRadius = new CornerRadius(22),
            Child = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Spacing = 12,
                Children =
                {
                    new Border
                    {
                        Width = 58,
                        Height = 58,
                        CornerRadius = new CornerRadius(29),
                        Background = Brush.Parse("#1D2028"),
                        Child = new TextBlock
                        {
                            Text = "✦",
                            FontSize = 22,
                            Foreground = Brush.Parse("#B1B4BE"),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    },
                    new TextBlock
                    {
                        Text = "No saved photos yet",
                        FontSize = 17,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center
                    },
                    new TextBlock
                    {
                        Text = "Save posts from the feed to fill this board",
                        FontSize = 13,
                        Foreground = Brush.Parse("#8B8E98"),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                        MaxWidth = 190,
                        TextWrapping = TextWrapping.Wrap
                    }
                }
            }
        };
    }

    private Border CreateMoodboardCard(Post post)
    {
        var root = new Border
        {
            Width = 300,
            Background = Brush.Parse("#13141A"),
            BorderBrush = Brush.Parse("#23242B"),
            BorderThickness = new Avalonia.Thickness(1),
            CornerRadius = new CornerRadius(22),
            ClipToBounds = true
        };

        var panel = new StackPanel
        {
            Spacing = 0
        };

        var imageHost = new Border
        {
            Height = GetCardHeight(post),
            Background = Brush.Parse("#1A1B21"),
            ClipToBounds = true
        };

        var image = new Image
        {
            Stretch = Stretch.UniformToFill
        };

        try
        {
            if (!string.IsNullOrWhiteSpace(post.ImagePath) && File.Exists(post.ImagePath))
            {
                using var stream = File.OpenRead(post.ImagePath);
                image.Source = new Bitmap(stream);
            }
        }
        catch
        {
        }

        imageHost.Child = image;
        panel.Children.Add(imageHost);

        var infoPanel = new StackPanel
        {
            Spacing = 8,
            Margin = new Avalonia.Thickness(14, 14, 14, 14)
        };

        infoPanel.Children.Add(new TextBlock
        {
            Text = post.User?.Username ?? "Unknown",
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            Foreground = Brushes.White
        });

        if (!string.IsNullOrWhiteSpace(post.Description))
        {
            infoPanel.Children.Add(new TextBlock
            {
                Text = post.Description,
                FontSize = 13,
                Foreground = Brush.Parse("#A5A8B2"),
                TextWrapping = TextWrapping.Wrap,
                MaxHeight = 54
            });
        }

        infoPanel.Children.Add(new TextBlock
        {
            Text = post.CreatedAt.ToString("dd.MM.yyyy  HH:mm"),
            FontSize = 11,
            Foreground = Brush.Parse("#6E727D")
        });

        panel.Children.Add(infoPanel);

        root.Child = panel;
        return root;
    }

    private double GetCardHeight(Post post)
    {
        if (string.IsNullOrWhiteSpace(post.Description))
            return 320;

        var length = post.Description.Length;

        if (length < 40) return 300;
        if (length < 90) return 340;
        return 380;
    }

    private void Close_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}