using Aliencube.YouTubeSubtitlesExtractor;
using Aliencube.YouTubeSubtitlesExtractor.Abstractions;
using Azure;
using Azure.AI.OpenAI;
using YoutubeSummarizer.Components;
using YoutubeSummarizer.Configurations;
using YoutubeSummarizer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddScoped<IYouTubeService, YouTubeService>()
    .AddScoped<IYouTubeVideo, YouTubeVideo>()
    .AddSingleton<OpenAISettings>(p => p.GetService<IConfiguration>()
                                        .GetSection(OpenAISettings.Name)
                                        .Get<OpenAISettings>())
    .AddSingleton<PromptSettings>(p => p.GetService<IConfiguration>()
                                        .GetSection(PromptSettings.Name)
                                        .Get<PromptSettings>())
    .AddScoped<OpenAIClient>(p =>
    {
        var openAISettings = p.GetService<OpenAISettings>();
        var endpoint = new Uri(openAISettings.Endpoint);
        var credential = new AzureKeyCredential(openAISettings.ApiKey);
        var client = new OpenAIClient(endpoint, credential);
        return client;
    })
    .AddHttpClient()


    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
