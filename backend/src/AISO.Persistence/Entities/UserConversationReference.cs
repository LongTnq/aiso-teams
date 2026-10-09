using System;

namespace AISO.Persistence.Entities;

/// <summary>
/// Stores the Bot Framework Conversation Reference for proactive messaging.
/// </summary>
public class UserConversationReference
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Entra ID Object ID / Teams User ID.</summary>
    public string TeamsUserId { get; set; } = string.Empty;

    /// <summary>JSON serialized ConversationReference.</summary>
    public string ConversationReferenceJson { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
