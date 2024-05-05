namespace YoutubeSummarizer.Configurations;

public class OpenAISettings
{
    public const string Name = "OpenAI";
    public string? DeploymentName { get; set; }
    public string Endpoint { get; set; }
    public string ApiKey { get; set; }
}

public class PromptSettings
{
    public const string Name = "Prompt";
    public string System { get; set; }
    public float Temperature { get; set; }
    public int MaxTokens { get; set; }
}
