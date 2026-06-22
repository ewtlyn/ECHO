using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using ECHO.Context;
using ECHO.Models;

namespace ECHO.Views;

public partial class AuthStartView : Window
{
    private string? _selectedAvatarPath;

    public AuthStartView()
    {
        InitializeComponent();
        ShowPreview();
    }

    private void ShowPreview()
    {
        PreviewPanel.IsVisible = true;
        LoginPanel.IsVisible = false;
        RegisterPanel.IsVisible = false;
    }

    private void ShowLogin()
    {
        PreviewPanel.IsVisible = false;
        LoginPanel.IsVisible = true;
        RegisterPanel.IsVisible = false;
    }

    private void ShowRegister()
    {
        PreviewPanel.IsVisible = false;
        LoginPanel.IsVisible = false;
        RegisterPanel.IsVisible = true;
    }

    private void CreateAccountButton_Click(object? sender, RoutedEventArgs e)
    {
        ShowRegister();
    }

    private void LogInButton_Click(object? sender, RoutedEventArgs e)
    {
        ShowLogin();
    }

    private void BackToPreview_Click(object? sender, RoutedEventArgs e)
    {
        ShowPreview();
    }

    private void LoginSubmit_Click(object? sender, RoutedEventArgs e)
    {
        using var db = new PostgresContext();

        var username = LoginUsernameTextBox.Text?.Trim();
        var password = LoginPasswordTextBox.Text;

        var user = db.Users.FirstOrDefault(u =>
            u.Username == username &&
            u.Password == password);

        if (user == null)
        {
            LoginErrorText.Text = "Неверный логин или пароль";
            return;
        }

        UserSession.CurrentUser = user;

        var mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }

    private void RegisterSubmit_Click(object? sender, RoutedEventArgs e)
    {
        using var db = new PostgresContext();

        var username = RegisterUsernameTextBox.Text?.Trim();
        var password = RegisterPasswordTextBox.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            RegisterErrorText.Text = "Заполните все поля";
            return;
        }

        var exists = db.Users.Any(u => u.Username == username);

        if (exists)
        {
            RegisterErrorText.Text = "Такой пользователь уже существует";
            return;
        }

        var user = new User
        {
            Username = username,
            Password = password,
            AvatarPath = _selectedAvatarPath,
            CreatedAt = DateTime.Now
        };

        db.Users.Add(user);
        db.SaveChanges();

        UserSession.CurrentUser = user;

        var mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }

    private async void ChooseAvatar_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
            return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(
            new Avalonia.Platform.Storage.FilePickerOpenOptions
            {
                Title = "Choose avatar",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new Avalonia.Platform.Storage.FilePickerFileType("Images")
                    {
                        Patterns = ["*.png", "*.jpg", "*.jpeg", "*.webp"]
                    }
                ]
            });

        if (files.Count == 0)
            return;

        _selectedAvatarPath = files[0].Path.LocalPath;

        using var stream = File.OpenRead(_selectedAvatarPath);
        AvatarPreviewImage.Source = new Bitmap(stream);
    }
}