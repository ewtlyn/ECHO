using System;
using System.Collections.Generic;

namespace ECHO.Models;

public partial class Post
{
    public int PostId { get; set; }

    public int UserId { get; set; }

    public string ImagePath { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AlbumPost> AlbumPosts { get; set; } = new List<AlbumPost>();

    public virtual ICollection<BoardPost> BoardPosts { get; set; } = new List<BoardPost>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User User { get; set; } = null!;
}
