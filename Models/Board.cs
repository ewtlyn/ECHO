using System;
using System.Collections.Generic;

namespace ECHO.Models;

public partial class Board
{
    public int BoardId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<BoardPost> BoardPosts { get; set; } = new List<BoardPost>();

    public virtual User User { get; set; } = null!;
}
