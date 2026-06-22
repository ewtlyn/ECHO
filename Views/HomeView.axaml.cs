using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ECHO.Context;
using ECHO.Models;
using Microsoft.EntityFrameworkCore;
using Material.Icons;
using Material.Icons.Avalonia;

namespace ECHO.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
        LoadFeed();
    }

    private void LoadFeed()
    {
        FeedPanel.Children.Clear();

        using var db = new PostgresContext();

        var posts = db.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        if (posts.Count == 0)
        {
            FeedPanel.Children.Add(new TextBlock
            {
                Text = "Пока нет постов",
                FontSize = 16,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            return;
        }

        foreach (var post in posts)
        {
            FeedPanel.Children.Add(CreatePostCard(post));
        }
    }

    private Border CreatePostCard(Post post)
    {
        var currentUserId = UserSession.CurrentUser?.UserId ?? 0;
        var isLikedByCurrentUser = post.Likes.Any(l => l.UserId == currentUserId);

        var root = new Border
        {
            Background = Brush.Parse("#1A1A1F"),
            BorderBrush = Brush.Parse("#2A2A31"),
            BorderThickness = new Avalonia.Thickness(1),
            CornerRadius = new CornerRadius(16),
            Padding = new Avalonia.Thickness(22, 18),
            MaxWidth = 780
        };

        var mainGrid = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto")
        };

        var headerGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*")
        };

        var avatarBorder = new Border
        {
            Width = 46,
            Height = 46,
            CornerRadius = new CornerRadius(23),
            Background = Brushes.LightGray,
            ClipToBounds = true
        };

        var avatarImage = new Image
        {
            Stretch = Stretch.UniformToFill
        };

        if (!string.IsNullOrWhiteSpace(post.User.AvatarPath) && File.Exists(post.User.AvatarPath))
        {
            using var avatarStream = File.OpenRead(post.User.AvatarPath);
            avatarImage.Source = new Bitmap(avatarStream);
        }

        avatarBorder.Child = avatarImage;

        var userInfoPanel = new StackPanel
        {
            Margin = new Avalonia.Thickness(14, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 4
        };

        userInfoPanel.Children.Add(new TextBlock
        {
            Text = post.User.Username,
            FontSize = 18,
            FontWeight = FontWeight.SemiBold,
            Foreground = Brushes.White
        });

        userInfoPanel.Children.Add(new TextBlock
        {
            Text = post.CreatedAt.ToString("dd.MM.yy HH:mm"),
            FontSize = 11,
            Foreground = Brush.Parse("#666872")
        });

        Grid.SetColumn(userInfoPanel, 1);

        headerGrid.Children.Add(avatarBorder);
        headerGrid.Children.Add(userInfoPanel);

        mainGrid.Children.Add(headerGrid);

        if (!string.IsNullOrWhiteSpace(post.ImagePath) && File.Exists(post.ImagePath))
        {
            var imageBorder = new Border
            {
                Width = 460,
                Height = 390,
                Margin = new Avalonia.Thickness(0, 16, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                CornerRadius = new CornerRadius(6),
                ClipToBounds = true
            };

            var postImage = new Image
            {
                Stretch = Stretch.UniformToFill
            };

            using var imageStream = File.OpenRead(post.ImagePath);
            postImage.Source = new Bitmap(imageStream);

            imageBorder.Child = postImage;

            Grid.SetRow(imageBorder, 1);
            mainGrid.Children.Add(imageBorder);
        }

        var actionsGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,Auto,Auto,*"),
            Margin = new Avalonia.Thickness(0, 18, 0, 0)
        };

        var likeButton = new Button
        {
            Background = Brushes.Transparent,
            BorderThickness = new Avalonia.Thickness(0),
            Padding = new Avalonia.Thickness(0),
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
        };

        var likePanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6
        };

        var likeIcon = new MaterialIcon
        {
            Kind = isLikedByCurrentUser ? MaterialIconKind.Heart : MaterialIconKind.HeartOutline,
            Width = 18,
            Height = 18,
            Foreground = isLikedByCurrentUser ? Brush.Parse("#FF4D6D") : Brush.Parse("#D8D8DA")
        };

        var likeText = new TextBlock
        {
            Text = $"{post.Likes.Count} likes",
            FontSize = 14,
            Foreground = Brush.Parse("#D8D8DA"),
            VerticalAlignment = VerticalAlignment.Center
        };

        likePanel.Children.Add(likeIcon);
        likePanel.Children.Add(likeText);

        likeButton.Content = likePanel;
        likeButton.Click += (_, _) => ToggleLike(post.PostId);

        var commentsButton = new Button
        {
            Background = Brushes.Transparent,
            BorderThickness = new Avalonia.Thickness(0),
            Padding = new Avalonia.Thickness(0),
            Margin = new Avalonia.Thickness(24, 0, 0, 0)
        };

        var commentsPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6
        };

        commentsPanel.Children.Add(new MaterialIcon
        {
            Kind = MaterialIconKind.CommentOutline,
            Width = 18,
            Height = 18,
            Foreground = Brush.Parse("#D8D8DA")
        });

        commentsPanel.Children.Add(new TextBlock
        {
            Text = $"{post.Comments.Count} comments",
            FontSize = 14,
            Foreground = Brush.Parse("#D8D8DA"),
            VerticalAlignment = VerticalAlignment.Center
        });

        commentsButton.Content = commentsPanel;
        commentsButton.Click += async (_, _) => await OpenComments(post.PostId);

        var addToAlbumButton = new Button
        {
            Background = Brushes.Transparent,
            BorderThickness = new Avalonia.Thickness(0),
            Padding = new Avalonia.Thickness(0),
            Margin = new Avalonia.Thickness(24, 0, 0, 0)
        };

        var addToAlbumPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6
        };

        addToAlbumPanel.Children.Add(new MaterialIcon
        {
            Kind = MaterialIconKind.AddCircleOutline,
            Width = 18,
            Height = 18,
            Foreground = Brush.Parse("#D8D8DA")
        });
        
        addToAlbumPanel.Children.Add(new TextBlock
        {
            Text = "save",
            FontSize = 14,
            Foreground = Brush.Parse("#D8D8DA"),
            VerticalAlignment = VerticalAlignment.Center
        });

        addToAlbumButton.Content = addToAlbumPanel;
        addToAlbumButton.Click += async (_, _) => await OpenSaveToBoard(post.PostId);

        actionsGrid.Children.Add(likeButton);

        Grid.SetColumn(commentsButton, 1);
        actionsGrid.Children.Add(commentsButton);

        Grid.SetColumn(addToAlbumButton, 2);
        actionsGrid.Children.Add(addToAlbumButton);

        Grid.SetRow(actionsGrid, 2);
        mainGrid.Children.Add(actionsGrid);

        var descriptionText = new TextBlock
        {
            Margin = new Avalonia.Thickness(0, 18, 0, 0),
            FontSize = 14,
            Foreground = Brushes.White,
            TextWrapping = TextWrapping.Wrap
        };

        descriptionText.Inlines.Add(new Run(post.User.Username + " ")
        {
            FontWeight = FontWeight.Bold
        });

        descriptionText.Inlines.Add(new Run(post.Description));

        Grid.SetRow(descriptionText, 3);
        mainGrid.Children.Add(descriptionText);

        root.Child = mainGrid;
        return root;
    }

    private async System.Threading.Tasks.Task OpenComments(int postId)
    {
        var window = new CommentsWindow(postId);

        var parentWindow = TopLevel.GetTopLevel(this) as Window;
        if (parentWindow != null)
            await window.ShowDialog(parentWindow);
        else
            window.Show();

        LoadFeed();
    }

    private async System.Threading.Tasks.Task OpenSaveToBoard(int postId)
    {
        var window = new SaveToBoardWindow(postId);

        var parentWindow = TopLevel.GetTopLevel(this) as Window;
        if (parentWindow != null)
            await window.ShowDialog(parentWindow);
        else
            window.Show();

        LoadFeed();
    }

    private void ToggleLike(int postId)
    {
        if (UserSession.CurrentUser == null)
            return;

        using var db = new PostgresContext();

        var existingLike = db.Likes.FirstOrDefault(l =>
            l.PostId == postId &&
            l.UserId == UserSession.CurrentUser.UserId);

        if (existingLike != null)
        {
            db.Likes.Remove(existingLike);
        }
        else
        {
            var like = new Like
            {
                PostId = postId,
                UserId = UserSession.CurrentUser.UserId,
                Time = System.DateTime.Now
            };

            db.Likes.Add(like);
        }

        db.SaveChanges();
        LoadFeed();
    }
}