using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ECHO.Context;
using ECHO.Models;

namespace ECHO.Views;

public partial class CreatePostWindow : Window
{
    private string? _selectedImagePath;

    public bool PostCreated { get; private set; }

    public CreatePostWindow()
    {
        InitializeComponent();
    }

    private async void ChooseImage_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
            return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Choose post image",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("Images")
                    {
                        Patterns = ["*.png", "*.jpg", "*.jpeg", "*.webp"]
                    }
                ]
            });

        if (files.Count == 0)
            return;

        _selectedImagePath = files[0].Path.LocalPath;

        using var stream = File.OpenRead(_selectedImagePath);
        PostPreviewImage.Source = new Bitmap(stream);
    }

    private void Publish_Click(object? sender, RoutedEventArgs e)
    {
        ErrorText.Text = "";

        if (UserSession.CurrentUser == null)
        {
            ErrorText.Text = "Пользователь не авторизован";
            return;
        }

        if (string.IsNullOrWhiteSpace(_selectedImagePath))
        {
            ErrorText.Text = "Выберите изображение";
            return;
        }

        var description = DescriptionTextBox.Text?.Trim();

        if (string.IsNullOrWhiteSpace(description))
        {
            ErrorText.Text = "Введите описание";
            return;
        }

        using var db = new PostgresContext();

        var post = new Post
        {
            UserId = UserSession.CurrentUser.UserId,
            ImagePath = _selectedImagePath,
            Description = description,
            CreatedAt = DateTime.Now
        };

        db.Posts.Add(post);
        db.SaveChanges();

        PostCreated = true;
        Close();
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}