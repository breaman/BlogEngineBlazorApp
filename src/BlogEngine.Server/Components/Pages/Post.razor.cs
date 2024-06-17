using System.Runtime.InteropServices.JavaScript;
using BlogEngine.Data.Models;
using BlogEngine.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace BlogEngine.Server.Components.Pages;

public partial class Post : ComponentBase
{
    [Parameter]
    public int Year { get; set; }
    [Parameter]
    public int Month { get; set; }
    [Parameter]
    public int Day { get; set; }
    [Parameter]
    public string Slug { get; set; }

    private PostDto _post;

    [Inject] private ApplicationDbContext DbContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var postDate = new DateTime(Year, Month, Day);
        var post = await DbContext.Posts.SingleOrDefaultAsync(p => p.Slug == Slug && p.IsPublished && p.PostedOn == postDate);
        _post = post.ToDto();
    }
}