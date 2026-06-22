using System;
using System.Collections.Generic;

namespace ECHO.Models;

public partial class AlbumPost
{
    public int AlbumPostId { get; set; }

    public int AlbumId { get; set; }

    public int PostId { get; set; }

    public virtual Album Album { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
}
