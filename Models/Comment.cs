using System;
using System.Collections.Generic;

namespace ECHO.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }

    public string Text { get; set; } = null!;

    public DateTime Time { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
