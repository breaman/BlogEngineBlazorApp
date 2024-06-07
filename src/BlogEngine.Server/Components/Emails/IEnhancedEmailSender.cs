using Microsoft.AspNetCore.Identity;

namespace BlogEngine.Server.Components.Emails;

public interface IEnhancedEmailSender<TUser> : IEmailSender<TUser> where TUser : class
{
    Task SendCongratulationsEmail(TUser user, string email);
}