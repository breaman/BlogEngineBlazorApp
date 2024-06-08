using FluentValidation;

namespace BlogEngine.Shared.DTOs;

public class PostDto
{
    public int PostId { get; set; }
    public string? Title { get; set; }
    public string? Slug { get; set; }
    public string? Content { get; set; }
    public DateTime? PostedOn { get; set; }
    public bool IsPublished { get; set; }
}

public class PostDtoValidator : AbstractValidator<PostDto>
{
    public PostDtoValidator()
    {
        RuleFor(viewModel => viewModel.Title).NotEmpty();
        RuleFor(viewModel => viewModel.Slug).NotEmpty();
        RuleFor(viewModel => viewModel.Content).NotEmpty();
        RuleFor(viewModel => viewModel.PostedOn).NotEmpty();
    }
}