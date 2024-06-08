using BlogEngine.Data.Models;
using BlogEngine.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace BlogEngine.Server.Components.Pages;

public partial class Blog : ComponentBase
{
    private List<PostDto>? _posts = new();
    [Inject] private ApplicationDbContext DbContext { get; set; }
    protected override async Task OnInitializedAsync()
    {
        var allPosts = await DbContext.Posts.Where(p => p.IsPublished).ToListAsync();
        foreach (var post in allPosts)
        {
            _posts.Add(post.ToDto());
        }
    }
}