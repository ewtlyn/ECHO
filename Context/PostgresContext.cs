using System;
using System.Collections.Generic;
using ECHO.Models;
using Microsoft.EntityFrameworkCore;

namespace ECHO.Context;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<AlbumPost> AlbumPosts { get; set; }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<BoardPost> BoardPosts { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.AlbumId).HasName("Albums_pkey");

            entity.ToTable("Albums", "ECHO");

            entity.Property(e => e.AlbumId).HasColumnName("AlbumID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Albums)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Albums_UserID_fkey");
        });

        modelBuilder.Entity<AlbumPost>(entity =>
        {
            entity.HasKey(e => e.AlbumPostId).HasName("AlbumPosts_pkey");

            entity.ToTable("AlbumPosts", "ECHO");

            entity.HasIndex(e => new { e.AlbumId, e.PostId }, "AlbumPosts_AlbumID_PostID_key").IsUnique();

            entity.Property(e => e.AlbumPostId).HasColumnName("AlbumPostID");
            entity.Property(e => e.AlbumId).HasColumnName("AlbumID");
            entity.Property(e => e.PostId).HasColumnName("PostID");

            entity.HasOne(d => d.Album).WithMany(p => p.AlbumPosts)
                .HasForeignKey(d => d.AlbumId)
                .HasConstraintName("AlbumPosts_AlbumID_fkey");

            entity.HasOne(d => d.Post).WithMany(p => p.AlbumPosts)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("AlbumPosts_PostID_fkey");
        });

        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.BoardId).HasName("Boards_pkey");

            entity.ToTable("Boards", "ECHO");

            entity.Property(e => e.BoardId).HasColumnName("BoardID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Boards)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Boards_UserID_fkey");
        });

        modelBuilder.Entity<BoardPost>(entity =>
        {
            entity.HasKey(e => e.BoardPostId).HasName("BoardPosts_pkey");

            entity.ToTable("BoardPosts", "ECHO");

            entity.HasIndex(e => new { e.BoardId, e.PostId }, "BoardPosts_BoardID_PostID_key").IsUnique();

            entity.Property(e => e.BoardPostId).HasColumnName("BoardPostID");
            entity.Property(e => e.BoardId).HasColumnName("BoardID");
            entity.Property(e => e.PostId).HasColumnName("PostID");

            entity.HasOne(d => d.Board).WithMany(p => p.BoardPosts)
                .HasForeignKey(d => d.BoardId)
                .HasConstraintName("BoardPosts_BoardID_fkey");

            entity.HasOne(d => d.Post).WithMany(p => p.BoardPosts)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("BoardPosts_PostID_fkey");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("Comments_pkey");

            entity.ToTable("Comments", "ECHO");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("Comments_PostID_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Comments_UserID_fkey");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("Likes_pkey");

            entity.ToTable("Likes", "ECHO");

            entity.HasIndex(e => new { e.UserId, e.PostId }, "Likes_UserID_PostID_key").IsUnique();

            entity.Property(e => e.LikeId).HasColumnName("LikeID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Post).WithMany(p => p.Likes)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("Likes_PostID_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Likes_UserID_fkey");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("Posts_pkey");

            entity.ToTable("Posts", "ECHO");

            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Posts_UserID_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("Users_pkey");

            entity.ToTable("Users", "ECHO");

            entity.HasIndex(e => e.Username, "Users_Username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.AvatarPath).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
