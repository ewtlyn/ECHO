using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using ECHO.Context;
using ECHO.Models;
using Microsoft.EntityFrameworkCore;

namespace ECHO.Views;

public partial class CommentsWindow : Window
{
    private readonly int _postId;

    public CommentsWindow(int postId)
    {
        InitializeComponent();
        _postId = postId;
        LoadComments();
    }

    private void LoadComments()
    {
        CommentsPanel.Children.Clear();

        using var db = new PostgresContext();

        var comments = db.Comments
            .Include(c => c.User)
            .Where(c => c.PostId == _postId)
            .OrderBy(c => c.Time)
            .ToList();

        if (comments.Count == 0)
        {
            CommentsPanel.Children.Add(new TextBlock
            {
                Text = "Комментариев пока нет",
                FontSize = 14,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            return;
        }

        foreach (var comment in comments)
        {
            CommentsPanel.Children.Add(CreateCommentCard(comment));
        }
    }

    private Border CreateCommentCard(Comment comment)
    {
        var root = new Border
        {
            Background = Brush.Parse("#1A1A1F"),
            BorderBrush = Brush.Parse("#2A2A31"),
            BorderThickness = new Avalonia.Thickness(1),
            CornerRadius = new CornerRadius(14),
            Padding = new Avalonia.Thickness(16, 14)
        };

        var panel = new StackPanel
        {
            Spacing = 6
        };

        panel.Children.Add(new TextBlock
        {
            Text = comment.User.Username,
            FontSize = 15,
            FontWeight = FontWeight.SemiBold,
            Foreground = Brushes.White
        });

        panel.Children.Add(new TextBlock
        {
            Text = comment.Time.ToString("dd.MM.yy HH:mm"),
            FontSize = 11,
            Foreground = Brush.Parse("#7A7C86")
        });

        panel.Children.Add(new TextBlock
        {
            Text = comment.Text,
            FontSize = 14,
            Foreground = Brushes.White,
            TextWrapping = TextWrapping.Wrap
        });

        root.Child = panel;
        return root;
    }

    private void SendComment_Click(object? sender, RoutedEventArgs e)
    {
        ErrorText.Text = "";

        if (UserSession.CurrentUser == null)
        {
            ErrorText.Text = "Пользователь не авторизован";
            return;
        }

        var text = CommentTextBox.Text?.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            ErrorText.Text = "Введите комментарий";
            return;
        }

        using var db = new PostgresContext();

        var comment = new Comment
        {
            PostId = _postId,
            UserId = UserSession.CurrentUser.UserId,
            Text = text,
            Time = System.DateTime.Now
        };

        db.Comments.Add(comment);
        db.SaveChanges();

        CommentTextBox.Text = "";
        LoadComments();
    }
}