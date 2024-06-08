namespace BlogEngine.Client.Pages.Admin;

public partial class EditPost : ComponentBase
{
    [Parameter]
    public int PostId { get; set; }
    
    private FluentValidationValidator? _fluentValidationValidator;
    private bool _isProcessing;

    private readonly PostViewModel _viewModel = new();
    
    [Inject] private HttpClient Client { get; set; }
    [Inject] private IToastService ToastService { get; set; }
    [Inject] private IJSRuntime JsRuntime { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // only make a call to the server from the client, not from server rendering
        if (!string.IsNullOrWhiteSpace(Client.BaseAddress?.AbsoluteUri))
        {
            await _viewModel.LoadPost(Client, PostId);
        }
    }

    private async Task SavePost()
    {
        if (await _fluentValidationValidator.ValidateAsync())
            try
            {
                _isProcessing = true;
                await _viewModel.UpdatePost(Client);

                if (_viewModel.ErrorMessages.Count == 0)
                    // ToastService.ShowSuccess("Post updated successfully.");
                    NavigationManager.NavigateTo("/admin/posts");
                else
                    ToastService.ShowError("Unable to update the post, please correct the errors and try again.");
            }
            finally
            {
                _isProcessing = false;
            }
        else
        {
            ToastService.ShowError("Unable to update the post, please correct the errors and try again.");
        }
    }
}