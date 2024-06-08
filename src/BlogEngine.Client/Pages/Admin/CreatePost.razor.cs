namespace BlogEngine.Client.Pages.Admin;

public partial class CreatePost : ComponentBase
{
    private FluentValidationValidator? _fluentValidationValidator;
    private bool _isProcessing;

    private readonly PostViewModel _viewModel = new()
    {
        Post = new PostDto { PostedOn = DateTime.Now }
    };

    [Inject] private HttpClient Client { get; set; }
    [Inject] private IToastService ToastService { get; set; }
    [Inject] private IJSRuntime JsRuntime { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    private async Task SavePost()
    {
        if (await _fluentValidationValidator.ValidateAsync())
            try
            {
                _isProcessing = true;
                await _viewModel.CreatePost(Client);

                if (_viewModel.ErrorMessages.Count == 0)
                    // ToastService.ShowSuccess("Post created successfully.");
                    NavigationManager.NavigateTo("/admin/posts");
                else
                    ToastService.ShowError("Unable to create the post, please correct the errors and try again.");
            }
            finally
            {
                _isProcessing = false;
            }
        else
        {
            ToastService.ShowError("Unable to create the post, please correct the errors and try again.");
        }
    }
}