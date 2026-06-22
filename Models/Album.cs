using System;
using System.Collections.Generic;

namespace ECHO.Models;

public partial class Album
{
    public int AlbumId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? CoverPath { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AlbumPost> AlbumPosts { get; set; } = new List<AlbumPost>();

    public virtual User User { get; set; } = null!;
}
