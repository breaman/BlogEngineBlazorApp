using BlogEngine.Shared.DTOs;

namespace BlogEngine.Data.Models;

public class Post : FingerPrintEntityBase
{
    public string? Title { get; set; }
    public string? Slug { get; set; }
    public string? Content { get; set; }
    public DateTime? PostedOn { get; set; }
    public bool IsPublished { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }

    public PostDto ToDto()
    {
        return new PostDto
        {
            PostId = Id,
            Title = Title,
            Content = Content,
            IsPublished = IsPublished,
            PostedOn = DateOnly.FromDateTime(PostedOn ?? DateTime.Now),
            Slug = Slug
        };
    }

    public void FromDto(PostDto dto, int userId)
    {
        Title = dto.Title;
        Content = dto.Content;
        IsPublished = dto.IsPublished;
        PostedOn = dto.PostedOn?.ToDateTime(new TimeOnly(0, 0, 0));
        Slug = dto.Slug;
        UserId = userId;
    }
}