using LogiKnow.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LogiKnow.Infrastructure.AI;

public class OpenAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly int _maxTokens;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(IConfiguration config, ILogger<OpenAIService> logger)
    {
        var apiKey = config["OpenAI:ApiKey"] ?? "REPLACE";
        _model = config["OpenAI:Model"] ?? "gpt-4o";
        _maxTokens = int.TryParse(config["OpenAI:MaxTokens"], out var mt) ? mt : 500;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _logger = logger;
    }

    public async Task<string> GenerateExplanationAsync(
        string termNameEn, string termNameAr, string category,
        string definitionEn, string lang, string style,
        CancellationToken ct = default)
    {
        var (systemPrompt, userPrompt) = PromptBuilder.BuildExplainTermPrompt(
            termNameEn, termNameAr, category, definitionEn, lang, style);

        try
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                max_tokens = _maxTokens,
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content, ct);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(responseJson);
            var text = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            return text ?? definitionEn;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate AI explanation for term {TermName}", termNameEn);
            return $"[AI service unavailable] {definitionEn}";
        }
    }
}
