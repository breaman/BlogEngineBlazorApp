using System.Security.Claims;
using BlogEngine.Data.Models;
using BlogEngine.Server.Models;
using BlogEngine.Shared.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SharedConstants = BlogEngine.Shared.Models.Constants;

namespace BlogEngine.Server.Apis;

public static class PostsApi
{
    public static RouteGroupBuilder MapPostsApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup(SharedConstants.PostApiUrl);
        group.RequireAuthorization();

        group.MapGet("/{postId:int}",
            async Task<Results<Ok<PostDto>, ValidationProblem>> (int postId, ClaimsPrincipal user, ApplicationDbContext dbContext,
                ILogger<PostDto> logger) =>
            {
                var validationProblem = Helpers.FetchUserId(out var userId, user, logger);

                if (validationProblem is not null)
                {
                    return validationProblem;
                }

                var post = await dbContext.Posts.SingleOrDefaultAsync(t => t.Id == postId && t.UserId == userId);
            
                if (post is null)
                {
                    Dictionary<string, string[]> problems = new();
                    problems.Add("", ["There is no team with that id for your account"]);
                    return TypedResults.ValidationProblem(problems);
                }

                var dto = post.ToDto();

                return TypedResults.Ok(dto);
            });

        group.MapPost("/create",
            async Task<Results<Created<PostDto>, ValidationProblem>> (PostDto dto,
                ClaimsPrincipal user, ApplicationDbContext dbContext, ILogger<PostDto> logger) =>
            {
                var validationProblem = Helpers.FetchUserId(out var userId, user, logger);

                if (validationProblem is not null) return validationProblem;

                var post = new Post();
                post.FromDto(dto, userId);

                dbContext.Posts.Add(post);

                try
                {
                    await dbContext.SaveChangesAsync();
                    logger.LogInformation("Successfully created Post {title} for user {userId}", dto.Title, userId);

                    dto.PostId = post.Id;
                    return TypedResults.Created("", dto);
                }
                catch (Exception e)
                {
                    logger.LogError("Unable to create Post {title} with the following exception: {exception}",
                        dto.Title, e);
                    Dictionary<string, string[]> problems = new();
                    problems.Add("error",
                        new[]
                        {
                            "An error occurred while trying to save the post. If this continues please contact support."
                        });
                    return TypedResults.ValidationProblem(problems);
                }
            });

        group.MapPut("/update/{teamId:int}", async Task<Results<Ok<PostDto>, ValidationProblem>> (PostDto dto,
            ClaimsPrincipal user, ApplicationDbContext dbContext, ILogger<PostDto> logger) =>
        {
            var validationProblem = Helpers.FetchUserId(out var userId, user, logger);

            if (validationProblem is not null) return validationProblem;

            var existingPost =
                await dbContext.Posts.SingleOrDefaultAsync(p => p.Id == dto.PostId && p.UserId == userId);

            if (existingPost is not null)
            {
                existingPost.FromDto(dto, userId);

                try
                {
                    await dbContext.SaveChangesAsync();
                    logger.LogInformation("Successfully updated Post {title} for user {userId}", dto.Title,
                        userId);
                    return TypedResults.Ok(dto);
                }
                catch (Exception e)
                {
                    logger.LogError("Unable to update Post {title} do to the following exception: {exception}",
                        dto.Title, e);
                    Dictionary<string, string[]> problems = new();
                    problems.Add("error",
                    [
                        "An error occurred while trying to update the Post. If this continues please contact support."
                    ]);
                    return TypedResults.ValidationProblem(problems);
                }
            }

            {
                logger.LogError(
                    "Unable to update Post: {title} since it has already been marked as deleted from the system",
                    dto.Title);
                Dictionary<string, string[]> problems = new();
                problems.Add("error",
                [
                    "Unable to update a Post that has already been deleted from the system."
                ]);
                return TypedResults.ValidationProblem(problems);
            }
        });

        return group;
    }
}