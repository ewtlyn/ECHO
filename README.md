# ECHO

A desktop application for storing and sharing photos with social features: a public feed, themed boards, likes, and comments.

![Platform](https://img.shields.io/badge/platform-Desktop-7C3AED)
![C#](https://img.shields.io/badge/C%23-.NET%208-512BD4)
![Avalonia](https://img.shields.io/badge/UI-Avalonia-blueviolet)
![PostgreSQL](https://img.shields.io/badge/DB-PostgreSQL-336791)

---

## About

ECHO is a desktop counterpart to Pinterest/Instagram, where users publish photos, save them to themed boards, like posts, and leave comments. The interface uses a dark color scheme with a purple accent — photos stay front and center, and the eyes don't tire during long browsing sessions.

## Tech Stack

- **Language:** C# (.NET 8)
- **UI:** Avalonia UI
- **ORM:** Entity Framework Core
- **Database:** PostgreSQL (via the Npgsql provider)
- **Architecture:** Views/Windows split by single responsibility, navigation through swapping `Content` inside a `ContentControl`

## Screenshots

<table>
  <tr>
    <td align="center"><b>Feed (HomeView)</b></td>
    <td align="center"><b>Boards gallery (GalleryView)</b></td>
  </tr>
  <tr>
    <td><img src="screenshots/![Uploading СОЗДАНИЕ АККАУНТА.png…]()
" width="400"/></td>
    <td><img src="screenshots/gallery.png" width="400"/></td>
  </tr>
  <tr>
    <td align="center"><b>User profile (ProfileView)</b></td>
    <td align="center"><b>Creating a post (CreatePostWindow)</b></td>
  </tr>
  <tr>
    <td><img src="screenshots/profile.png" width="400"/></td>
    <td><img src="screenshots/create_post.png" width="400"/></td>
  </tr>
</table>

> Screenshots live in the `screenshots/` folder. Replace the files with your own — the filenames can stay the same.

## Architecture

The project follows separation of concerns: every window/view consists of a markup file (`.axaml`) and a code-behind file (`.axaml.cs`).

**Main views:**
- `HomeView` — feed of posts from all users with likes, comments, and saving to a board
- `GalleryView` — gallery of themed boards with image previews
- `ProfileView` — personal profile, post creation, sign-out

**Dialog windows:**
- `CommentsWindow` — viewing and adding comments to a post
- `CreatePostWindow` — creating a new post
- `SaveToBoardWindow` — saving a post to a board
- `BoardDetailsWindow` — detailed view of a board

**Database (ECHO schema):** `Users`, `Posts`, `Likes`, `Comments`, `Boards`, `BoardPosts`, `Albums`, `AlbumPosts` — many-to-many relations between boards and posts through `BoardPosts`, a unique composite key `(UserID, PostID)` on the `Likes` table prevents duplicate likes.

**Session state** is stored in a static `UserSession` class (`CurrentUser`, `Logout()`), giving access to the authenticated user's data from anywhere in the app without passing it through constructors.

## Running locally

```bash
git clone https://github.com/<your-username>/echo.git
cd echo
```

1. Create a PostgreSQL database and apply the schema from `database/schema.sql` (if present)
2. Set the connection string in `appsettings.json` or `PostgresContext`
3. Restore dependencies and run:

```bash
dotnet restore
dotnet run
```

## Features

- Post feed with likes and comments
- Themed boards for saving posts
- User profile with personal posts
- Authentication and registration

## Roadmap

- Search across posts
- Following system
- Tags
- Cloud media storage
- Feed pagination
