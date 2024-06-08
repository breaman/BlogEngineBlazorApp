using System.Net.Http.Json;
using System.Text.Json;
using BlogEngine.Shared.DTOs;
using BlogEngine.Shared.Models;
using FluentValidation;
using SharedConstants = BlogEngine.Shared.Models.Constants;

namespace BlogEngine.Shared.ViewModels;

public class PostViewModel
{
    public PostDto? Post { get; set; }

    public List<string> ErrorMessages { get; set; } = new();

    public async Task LoadPost(HttpClient client, int postId)
    {
        var postDto = await client.GetFromJsonAsync<PostDto>($"{SharedConstants.PostApiUrl}/{postId}");
        Post = postDto;
    }
    
    public async Task CreatePost(HttpClient client)
    {
        ErrorMessages.Clear();
        await UpsertPost(client);
    }

    public async Task UpdatePost(HttpClient client)
    {
        ErrorMessages.Clear();
        await UpsertPost(client);
    }

    private async Task UpsertPost(HttpClient client)
    {
        HttpResponseMessage? result = null;

        if (Post.PostId > 0)
            result = await client.PutAsJsonAsync($"{SharedConstants.PostApiUrl}/update/{Post.PostId}", Post);
        else
            result = await client.PostAsJsonAsync($"{SharedConstants.PostApiUrl}/create", Post);

        var jsonString = await result.Content.ReadAsStringAsync();
        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        if (result.IsSuccessStatusCode)
        {
            Post = JsonSerializer.Deserialize<PostDto>(jsonString, options);
        }
        else
        {
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(jsonString, options);
            foreach (var errors in problemDetails.Errors)
            {
                foreach (var errorMessage in errors.Value)
                {
                    ErrorMessages.Add(errorMessage);
                }
            }
        }
    }
}

public class PostViewModelValidator : AbstractValidator<PostViewModel>
{
    public PostViewModelValidator()
    {
        RuleFor(vm => vm.Post).SetValidator(new PostDtoValidator());
    }
}