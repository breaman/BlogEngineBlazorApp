using System.Text.RegularExpressions;

namespace BlogEngine.Client.Components;

public partial class PostComponent : ComponentBase
{
    private EditContext? _postContext;

    [Parameter] public PostDto? Post { get; set; }

    protected override void OnParametersSet()
    {
        _postContext = new EditContext(Post);
        // _postContext.OnFieldChanged += PostContext_OnFieldChanged;
    }

    private void PostContext_OnFieldChanged(object sender, FieldChangedEventArgs eventArgs)
    {
        // Console.WriteLine(eventArgs.FieldIdentifier.FieldName);
        // Team.DataChanged = true;
    }

    private async Task UpdateSlug(string title)
    {
        Post.Title = title;
        if (string.IsNullOrWhiteSpace(Post.Slug))
        {
            title = title.ToLower();
            title = Regex.Replace(title, "[^a-zA-Z0-9 -]", "");
            title = title.Trim().Replace(' ', '-');
            Post.Slug = title;
        }
    }

    private void ReplaceSlug()
    {
        var title = Post.Title.ToLower();
        title = Regex.Replace(title, "[^a-zA-Z0-9 -]", "");
        title = title.Trim().Replace(' ', '-');
        Post.Slug = title;
    }
}