using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AISO.Bot.Services;

public class ProactiveMessagingService
{
    private readonly IBotFrameworkHttpAdapter _adapter;
    private readonly UserMappingService _userMappingService;
    private readonly string _botAppId;
    private readonly ILogger<ProactiveMessagingService> _logger;

    public ProactiveMessagingService(
        IBotFrameworkHttpAdapter adapter,
        UserMappingService userMappingService,
        IConfiguration configuration,
        ILogger<ProactiveMessagingService> logger)
    {
        _adapter = adapter;
        _userMappingService = userMappingService;
        _botAppId = configuration["MicrosoftAppId"] ?? string.Empty;
        _logger = logger;
    }

    public async Task<bool> SendMessageBySapUserAsync(string sapUserId, string message, CancellationToken cancellationToken = default)
    {
        var teamsUserId = await _userMappingService.GetTeamsUserIdBySapUserAsync(sapUserId, cancellationToken);
        if (string.IsNullOrEmpty(teamsUserId))
        {
            _logger.LogWarning("No Teams user mapped for SAP user {SapUserId}", sapUserId);
            return false;
        }

        return await SendMessageByTeamsUserAsync(teamsUserId, message, cancellationToken);
    }

    public async Task<bool> SendMessageByTeamsUserAsync(string teamsUserId, string message, CancellationToken cancellationToken = default)
    {
        var referenceJson = await _userMappingService.GetConversationReferenceAsync(teamsUserId, cancellationToken);
        if (string.IsNullOrEmpty(referenceJson))
        {
            _logger.LogWarning("No conversation reference found for Teams user {TeamsUserId}", teamsUserId);
            return false;
        }

        var reference = JsonConvert.DeserializeObject<ConversationReference>(referenceJson);
        if (reference == null)
            return false;

        await ((CloudAdapter)_adapter).ContinueConversationAsync(
            _botAppId,
            reference,
            async (turnContext, ct) =>
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(message), ct);
            },
            cancellationToken);

        return true;
    }
}
