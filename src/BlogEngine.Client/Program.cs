using Blazored.Toast;
using BlogEngine.Shared.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

builder.Services.AddTransient<IValidator<PostViewModel>, PostViewModelValidator>();

builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();