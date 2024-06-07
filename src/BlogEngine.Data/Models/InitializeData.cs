using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedConstants = BlogEngine.Shared.Models.Constants;

namespace BlogEngine.Data.Models;

public static class InitializeData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger("InitializeData");
        
        InitializeRoles(serviceProvider, context, logger);
    }
    
    private static void InitializeRoles(IServiceProvider serviceProvider, ApplicationDbContext context, ILogger logger)
    {
        if (!context.Roles.Any())
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var createTask = roleManager.CreateAsync(new Role { Name =  SharedConstants.User});
            createTask.Wait();
            var identityResult = createTask.Result;

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    logger.LogError($"{error.Code}: {error.Description}");
                }
            }
            createTask = roleManager.CreateAsync(new Role { Name = SharedConstants.Admin });
            createTask.Wait();
            identityResult = createTask.Result;

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    logger.LogError($"{error.Code}: {error.Description}");
                }
            }
        }
    }
}