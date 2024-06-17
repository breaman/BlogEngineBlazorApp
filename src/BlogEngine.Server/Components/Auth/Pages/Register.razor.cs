using System.Text;
using System.Text.Encodings.Web;
using Blazored.FluentValidation;
using BlogEngine.Data.Models;
using BlogEngine.Server.Components.Emails;
using BlogEngine.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SharedConstants = BlogEngine.Shared.Models.Constants;

namespace BlogEngine.Server.Components.Auth.Pages;

public partial class Register : ComponentBase
{
    private FluentValidationValidator _fluentValidationValidator;
    private List<IdentityError>? _identityErrors = new();

    [SupplyParameterFromForm] private RegisterDto Dto { get; } = new();

    [SupplyParameterFromQuery] private string? ReturnUrl { get; }

    [Inject] protected UserManager<User>? UserManager { get; set; }

    [Inject] protected SignInManager<User>? SignInManager { get; set; }

    [Inject] protected ILogger<Register> Logger { get; set; }

    [Inject] protected NavigationManager NavigationManager { get; set; }

    [Inject] protected IEnhancedEmailSender<User> EmailSender { get; set; }

    [Inject] internal IdentityRedirectManager RedirectManager { get; set; }

    private string? Message => _identityErrors is null
        ? null
        : $"Error: {string.Join(", ", _identityErrors.Select(error => error.Description))}";

    private async Task RegisterUser()
    {
        _identityErrors = new List<IdentityError>();
        
        if (Dto.Email?.Trim() != "mike@stokesbary.me")
        {
            Logger.LogError("User is not in the approved list of users");
            _identityErrors.Add(new IdentityError()
            {
                Description = "User is not in the approved list of users.",
                Code = "INVALID_USER"
            });
            return;
        }
        
        if (await _fluentValidationValidator.ValidateAsync())
        {
            var user = new User
            {
                FirstName = Dto.FirstName?.Trim(),
                LastName = Dto.LastName?.Trim(),
                Email = Dto.Email?.Trim(),
                UserName = Dto.Email?.Trim(),
                MemberSince = DateTime.UtcNow
            };

            var result = await UserManager.CreateAsync(user, Dto.Password);

            if (!result.Succeeded)
            {
                _identityErrors = result.Errors.ToList();
                return;
            }

            Logger.LogInformation("User created a new account with password.");

            await UserManager.AddToRoleAsync(user, SharedConstants.User);

            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = user.Id, ["code"] = code, ["returnUrl"] = ReturnUrl });

            await EmailSender.SendConfirmationLinkAsync(user, user.Email, HtmlEncoder.Default.Encode(callbackUrl));

            if (UserManager.Options.SignIn.RequireConfirmedEmail)
                RedirectManager.RedirectTo("Account/RegistrationConfirmation",
                    new Dictionary<string, object?> { ["email"] = Dto.Email, ["returnUrl"] = ReturnUrl });

            await SignInManager.SignInAsync(user, false);
            RedirectManager.RedirectTo(ReturnUrl);
        }
    }
}