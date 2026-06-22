using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using ECHO.Context;
using ECHO.Models;

namespace ECHO.Views;

public partial class SaveToBoardWindow : Window
{
    private readonly int _postId;

    public bool AddedSuccessfully { get; private set; }

    public SaveToBoardWindow(int postId)
    {
        InitializeComponent();
        _postId = postId;
        LoadBoards();
    }

    private void LoadBoards()
    {
        AlbumsPanel.Children.Clear();
        ErrorText.Text = "";

        if (UserSession.CurrentUser == null)
            return;

        using var db = new PostgresContext();

        var boards = db.Boards
            .Where(b => b.UserId == UserSession.CurrentUser.UserId)
            .OrderByDescending(b => b.CreatedAt)
            .ToList();

        if (boards.Count == 0)
        {
            AlbumsPanel.Children.Add(new TextBlock
            {
                Text = "У вас пока нет досок",
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            return;
        }

        foreach (var board in boards)
        {
            AlbumsPanel.Children.Add(CreateBoardItem(board));
        }
    }

    private Border CreateBoardItem(Board board)
    {
        var root = new Border
        {
            Background = Brush.Parse("#1A1A1F"),
            BorderBrush = Brush.Parse("#2A2A31"),
            BorderThickness = new Avalonia.Thickness(1),
            CornerRadius = new CornerRadius(14),
            Padding = new Avalonia.Thickness(16, 14)
        };

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto")
        };

        var textPanel = new StackPanel
        {
            Spacing = 4
        };

        textPanel.Children.Add(new TextBlock
        {
            Text = board.Title,
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            Foreground = Brushes.White
        });

        textPanel.Children.Add(new TextBlock
        {
            Text = $"Created: {board.CreatedAt:dd.MM.yyyy}",
            FontSize = 12,
            Foreground = Brush.Parse("#A4A5AD")
        });

        var button = new Button
        {
            Content = "Save",
            Width = 82,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Height = 36,
            Background = Brush.Parse("#8B5CF6"),
            CornerRadius = new CornerRadius(12),
            Foreground = Brushes.White
        };

        button.Click += (_, _) => AddPostToBoard(board.BoardId);

        Grid.SetColumn(button, 1);

        grid.Children.Add(textPanel);
        grid.Children.Add(button);

        root.Child = grid;
        return root;
    }

    private void AddPostToBoard(int boardId)
    {
        if (UserSession.CurrentUser == null)
            return;

        using var db = new PostgresContext();

        var exists = db.BoardPosts.Any(x => x.BoardId == boardId && x.PostId == _postId);
        if (exists)
        {
            ErrorText.Text = "Это фото уже есть в этой доске";
            return;
        }

        var boardPost = new BoardPost
        {
            BoardId = boardId,
            PostId = _postId
        };

        db.BoardPosts.Add(boardPost);
        db.SaveChanges();

        AddedSuccessfully = true;
        Close();
    }
}