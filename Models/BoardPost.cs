using System;
using System.Collections.Generic;

namespace ECHO.Models;

public partial class BoardPost
{
    public int BoardPostId { get; set; }

    public int BoardId { get; set; }

    public int PostId { get; set; }

    public virtual Board Board { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
}
