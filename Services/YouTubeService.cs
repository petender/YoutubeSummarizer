using Aliencube.YouTubeSubtitlesExtractor;
using Aliencube.YouTubeSubtitlesExtractor.Abstractions;
using Azure.AI.OpenAI;
using System.Threading;
using System.Web;
using YoutubeSummarizer.Configurations;

namespace YoutubeSummarizer.Services;

    public interface IYouTubeService
    {
        Task<string> Summarize(string videoLink, string videoLanguage = "en", string summaryLanguage = "en");
    }
    public class YouTubeService : IYouTubeService
        {
            private readonly IYouTubeVideo _youTubeVideo;
            private readonly OpenAIClient _openAIClient;
            private readonly OpenAISettings _openAISettings;    
            private readonly PromptSettings _promptSettings;
            public YouTubeService(
                IYouTubeVideo youTubeVideo, 
                OpenAIClient openAIClient, 
                OpenAISettings openAISettings, 
                PromptSettings promptSettings)
            {
                _youTubeVideo = youTubeVideo ?? throw new ArgumentNullException(nameof(youTubeVideo));
                _openAIClient = openAIClient ?? throw new ArgumentNullException(nameof(openAIClient));
                _openAISettings = openAISettings ?? throw new ArgumentNullException(nameof(openAISettings));
                _promptSettings = promptSettings ?? throw new ArgumentNullException(nameof(promptSettings));
            }

        public async Task<string> Summarize(string videoLink, string videoLanguage = "en", string summaryLanguage = "en")
        {
            
            var subtitle = await GetSubtitle(videoLink, videoLanguage);
            var summary = await GetSummary(subtitle, summaryLanguage);
            return summary;
        }

        

       private async Task<string> GetSubtitle(string videoUrl, string videoLanguage)
        {
            var subtitle = await _youTubeVideo.ExtractSubtitleAsync(videoUrl, videoLanguage);
            var aggregated = subtitle.Content.Select(p => p.Text).Aggregate((a, b) => $"{a}\n{b}");
            return aggregated;
        }

        private async Task<string> GetSummary(string subtitle, string summaryLanguage)
        {
            
            var deploymentName = _openAISettings.DeploymentName;
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                MaxTokens = _promptSettings.MaxTokens,
                Temperature = _promptSettings.Temperature,
                DeploymentName = deploymentName,
                Messages =
                {
                    new ChatMessage(ChatRole.System, _promptSettings.System),
                    new ChatMessage(ChatRole.System, $"Here is the transcript. Summarize in 5 bullet points in the given language: {summaryLanguage}"),
                    new ChatMessage(ChatRole.User, subtitle)    
                }
            };
        // create var summary

            var summary = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);
            return summary.Value.Choices[0].Message.Content;
            }
 
        
    }   

