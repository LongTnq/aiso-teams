using AISO.Bot.Services;
using Microsoft.AspNetCore.Mvc;

namespace AISO.Api.Controllers;

[Route("api/notifications")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly ProactiveMessagingService _proactiveMessagingService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        ProactiveMessagingService proactiveMessagingService,
        ILogger<NotificationController> logger)
    {
        _proactiveMessagingService = proactiveMessagingService;
        _logger = logger;
    }

    public class NotificationPayload
    {
        public string SapUserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] NotificationPayload payload, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(payload.SapUserId) || string.IsNullOrWhiteSpace(payload.Message))
        {
            return BadRequest("SapUserId and Message are required.");
        }

        var success = await _proactiveMessagingService.SendMessageBySapUserAsync(payload.SapUserId, payload.Message, ct);

        if (success)
        {
            return Ok();
        }
        else
        {
            return NotFound("User not mapped or conversation reference not found.");
        }
    }
}
