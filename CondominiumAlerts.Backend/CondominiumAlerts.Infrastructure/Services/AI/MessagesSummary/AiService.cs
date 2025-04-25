using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Infrastructure.Services.AI.MessagesSummary;
using Microsoft.Extensions.Logging;


namespace CondominiumAlerts.Infrastructure.Services.AI.MessagesSummary;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AiService> _logger;
    
    public AiService(HttpClient httpClient, ILogger<AiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<Summary?> GenerateSummary(List<MessageSummaryDto> messages, User user, Condominium condominium, CancellationToken cancellationToken)
    {
        var formattedMessages = messages.Select(m => $"[{m.CreatedAt:yyyy-MM-dd HH:mm:ss}] {m.CreatorUserFullname} - {m.CreatorUsername} - {m.Text}").ToList();

        var requestBody = new
        {
            model = "deepseek/deepseek-chat:free",
            messages = new[]
            {
                new { role = "user", content = $"Generame un resumen claro y conciso de lo que se habló en estas conversaciones:\n\n{string.Join("\n", formattedMessages)}\n\n---\n\nResumen:" }
            }
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"
        );

        using var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
        {
            Content = jsonContent
        };
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "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");

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error al obtener resumen: {response.StatusCode}");
            return null;
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        try
        {
            var deepSeekResponse = JsonSerializer.Deserialize<DeepSeekResponse>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (deepSeekResponse?.Choices?.Count > 0)
            {
                var summary = new Summary()
                {
                    Id = Guid.NewGuid(),
                    Content = deepSeekResponse.Choices[0].Message.Content,
                    User = user,
                    TriggeredBy = user.Id,
                    Condominium = condominium,
                    CondominiumId = condominium.Id,
                    CreatedAt = DateTime.UtcNow
                };
                return summary;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al deserializar la respuesta: {ex.Message}");
        }

        return null;
    }
    
}