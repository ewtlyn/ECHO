using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ECHO.Context;
using ECHO.Models;
using Microsoft.EntityFrameworkCore;

namespace ECHO.Views;

public partial class GalleryView : UserControl
{
    public GalleryView()
    {
        InitializeComponent();
        LoadBoards();
    }

    private void LoadBoards()
    {
        BoardsPanel.Children.Clear();

        if (UserSession.CurrentUser == null) 
            return;

        using var db = new PostgresContext();

        var boards = db.Boards
            .Include(b => b.BoardPosts)
            .ThenInclude(bp => bp.Post)
            .Where(b => b.UserId == UserSession.CurrentUser.UserId)
            .OrderByDescending(b => b.CreatedAt)
            .ToList();

        if (boards.Count == 0)
        {
            BoardsPanel.Children.Add(CreateEmptyStateCard());
            return;
        }

        foreach (var board in boards)
        {
            BoardsPanel.Children.Add(CreateBoardCard(board));
        }
    }

    private Border CreateEmptyStateCard()
    {
        return new Border
        {
            Width = 340,
            Height = 420,
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
                        Width = 64,
                        Height = 64,
                        CornerRadius = new CornerRadius(32),
                        Background = Brush.Parse("#1E2028"),
                        BorderBrush = Brush.Parse("#3A3C47"),
                        BorderThickness = new Avalonia.Thickness(1),
                        Child = new TextBlock
                        {
                            Text = "+",
                            FontSize = 28,
                            Foreground = Brushes.White,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    },
                    new TextBlock
                    {
                        Text = "Create your first board",
                        FontSize = 18,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center
                    },
                    new TextBlock
                    {
                        Text = "Save inspirations, aesthetics and moodboards here",
                        FontSize = 13,
                        Foreground = Brush.Parse("#8A8D98"),
                        TextAlignment = TextAlignment.Center,
                        MaxWidth = 220,
                        TextWrapping = TextWrapping.Wrap,
                        HorizontalAlignment = HorizontalAlignment.Center
                    }
                }
            }
        };
    }

    private Border CreateBoardCard(Board board)
    {
        var root = new Border
        {
            Width = 340,
            Background = Brush.Parse("#13141A"),
            BorderBrush = Brush.Parse("#23242B"),
            BorderThickness = new Avalonia.Thickness(1),
            CornerRadius = new CornerRadius(22),
            Padding = new Avalonia.Thickness(14),
            Cursor = new Cursor(StandardCursorType.Hand)
        };

        var panel = new StackPanel
        {
            Spacing = 14
        };

        var preview = CreatePreviewLayout(board);

        panel.Children.Add(preview);

        var titleRow = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto")
        };

        titleRow.Children.Add(new TextBlock
        {
            Text = board.Title,
            FontSize = 18,
            FontWeight = FontWeight.SemiBold,
            Foreground = Brushes.White,
            VerticalAlignment = VerticalAlignment.Center
        });

        var badge = new Border
        {
            Padding = new Avalonia.Thickness(10, 4),
            Background = Brush.Parse("#1E2028"),
            CornerRadius = new CornerRadius(999),
            Child = new TextBlock
            {
                Text = $"{board.BoardPosts.Count}",
                FontSize = 12,
                Foreground = Brush.Parse("#C8CAD2"),
                HorizontalAlignment = HorizontalAlignment.Center
            }
        };
        Grid.SetColumn(badge, 1);
        titleRow.Children.Add(badge);

        panel.Children.Add(titleRow);

        if (!string.IsNullOrWhiteSpace(board.Description))
        {
            panel.Children.Add(new TextBlock
            {
                Text = board.Description,
                FontSize = 13,
                Foreground = Brush.Parse("#9A9CA6"),
                TextWrapping = TextWrapping.Wrap,
                MaxHeight = 38
            });
        }

        panel.Children.Add(new TextBlock
        {
            Text = $"{board.BoardPosts.Count} saved photos",
            FontSize = 12,
            Foreground = Brush.Parse("#6F727C")
        });

        root.Child = panel;

        root.PointerPressed += async (_, _) =>
        {
            var window = new BoardDetailsWindow(board.BoardId);
            var parent = TopLevel.GetTopLevel(this) as Window;

            if (parent != null) await window.ShowDialog(parent);
            else window.Show();

            LoadBoards();
        };

        return root;
    }

    private Control CreatePreviewLayout(Board board)
    {
        var posts = board.BoardPosts
            .Select(bp => bp.Post)
            .Where(p => !string.IsNullOrWhiteSpace(p.ImagePath))
            .Take(3)
            .ToList();

        if (posts.Count == 0)
        {
            return new Border
            {
                Height = 280,
                Background = Brush.Parse("#1A1B21"),
                CornerRadius = new CornerRadius(18),
                Child = new StackPanel
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Spacing = 10,
                    Children =
                    {
                        new Border
                        {
                            Width = 52,
                            Height = 52,
                            CornerRadius = new CornerRadius(26),
                            Background = Brush.Parse("#23242B"),
                            Child = new TextBlock
                            {
                                Text = "✦",
                                FontSize = 20,
                                Foreground = Brush.Parse("#B0B3BE"),
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        },
                        new TextBlock
                        {
                            Text = "Empty board",
                            Foreground = Brush.Parse("#B0B3BE"),
                            FontSize = 14,
                            HorizontalAlignment = HorizontalAlignment.Center
                        }
                    }
                }
            };
        }

        var grid = new Grid
        {
            Height = 280,
            ColumnDefinitions = new ColumnDefinitions("2*,*"),
            RowDefinitions = new RowDefinitions("*,*")
        };

        for (int i = 0; i < posts.Count; i++)
        {
            var imageBorder = new Border
            {
                Margin = new Avalonia.Thickness(4),
                CornerRadius = new CornerRadius(16),
                ClipToBounds = true,
                Background = Brush.Parse("#1A1B21")
            };

            var image = new Image
            {
                Stretch = Stretch.UniformToFill
            };

            try
            {
                if (File.Exists(posts[i].ImagePath))
                {
                    using var stream = File.OpenRead(posts[i].ImagePath);
                    image.Source = new Bitmap(stream);
                }
            }
            catch
            {
            }

            imageBorder.Child = image;

            if (i == 0)
            {
                Grid.SetColumn(imageBorder, 0);
                Grid.SetRow(imageBorder, 0);
                Grid.SetRowSpan(imageBorder, 2);
            }
            else if (i == 1)
            {
                Grid.SetColumn(imageBorder, 1);
                Grid.SetRow(imageBorder, 0);
            }
            else
            {
                Grid.SetColumn(imageBorder, 1);
                Grid.SetRow(imageBorder, 1);
            }

            grid.Children.Add(imageBorder);
        }

        return grid;
    }

    private void OpenCreateBoardPanel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CreateBoardPanel.IsVisible = true;
        CreateBoardErrorText.Text = "";
    }

    private void CancelCreateBoard_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CreateBoardPanel.IsVisible = false;
        BoardTitleTextBox.Text = "";
        BoardDescriptionTextBox.Text = "";
        CreateBoardErrorText.Text = "";
    }

    private void CreateBoard_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CreateBoardErrorText.Text = "";

        if (UserSession.CurrentUser == null)
        {
            CreateBoardErrorText.Text = "Пользователь не авторизован";
            return;
        }

        var title = BoardTitleTextBox.Text?.Trim();
        var description = BoardDescriptionTextBox.Text?.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            CreateBoardErrorText.Text = "Введите название доски";
            return;
        }

        using var db = new PostgresContext();

        var board = new Board
        {
            UserId = UserSession.CurrentUser.UserId,
            Title = title,
            Description = string.IsNullOrWhiteSpace(description) ? null : description,
            CreatedAt = DateTime.Now
        };

        db.Boards.Add(board);
        db.SaveChanges();

        BoardTitleTextBox.Text = "";
        BoardDescriptionTextBox.Text = "";
        CreateBoardPanel.IsVisible = false;

        LoadBoards();
    }
}