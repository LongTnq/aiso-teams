using AISO.Bot.Cards.Builders;
using AISO.Domain.Approvals;
using Newtonsoft.Json;
using Xunit;

namespace AISO.UnitTests;

public class PendingApprovalsCardTests
{
    [Fact]
    public void BuildPendingApprovalsCard_RendersOrderAndRequester()
    {
        var attachment = TeamsCardBuilder.BuildPendingApprovalsCard(new
        {
            count = 1,
            items = new[]
            {
                new
                {
                    orderId = "0000000009",
                    requestedBy = "DEV-024",
                    comment = "please approve",
                    requestedAt = "2026-07-22 12:00:00Z"
                }
            }
        });

        var json = JsonConvert.SerializeObject(attachment.Content);
        Assert.Contains("Pending approvals", json);
        Assert.Contains("0000000009", json);
        Assert.Contains("DEV-024", json);
        Assert.Contains("Approve", json);
        Assert.Contains("Reject approval", json);
        Assert.Contains("filter_pending_approvals", json);
        Assert.Contains("Requested by", json);
        Assert.DoesNotContain("Sales organization", json);
        Assert.DoesNotContain("\"id\":\"salesOrg\"", json);
    }

    [Fact]
    public void BuildPendingApprovalsCard_FiltersAndKeepsDynamicChoices()
    {
        var requestedAt = new DateTimeOffset(2026, 7, 22, 12, 0, 0, TimeSpan.Zero);
        var approvals = new[]
        {
            Approval("0000000009", "DEV-024", "UE00", "priority", requestedAt),
            Approval("0000000010", "DEV-025", "US00", "standard", requestedAt)
        };

        var attachment = TeamsCardBuilder.BuildPendingApprovalsCard(
            approvals,
            search: "priority",
            requester: "DEV-024");

        var json = JsonConvert.SerializeObject(attachment.Content);
        Assert.Contains("0000000009", json);
        Assert.DoesNotContain("0000000010", json);
        Assert.Contains("DEV-025", json); // still in requester choices
        Assert.Contains("\"value\":\"priority\"", json);
        Assert.DoesNotContain("Sales organization", json);
        Assert.DoesNotContain("\"id\":\"salesOrg\"", json);
    }

    [Fact]
    public void BuildNotAuthorizedCard_ShowsRoles()
    {
        var attachment = TeamsCardBuilder.BuildNotAuthorizedCard(
            "You do not have permission.",
            "Employee",
            "Manager");

        var json = JsonConvert.SerializeObject(attachment.Content);
        Assert.Contains("Not authorized", json);
        Assert.Contains("Employee", json);
        Assert.Contains("Manager", json);
    }

    [Fact]
    public void BuildOverdueOrdersCard_RendersDaysLate()
    {
        var attachment = TeamsCardBuilder.BuildOverdueOrdersCard(new
        {
            count = 1,
            orders = new[]
            {
                new
                {
                    soNumber = "0000000001",
                    customerName = "Acme",
                    daysPastDue = 5,
                    formattedValue = "100 USD",
                    scheduledDeliveryDate = "01 Jan 2026",
                    salesOrg = "UE00"
                }
            }
        });

        var json = JsonConvert.SerializeObject(attachment.Content);
        Assert.Contains("Overdue orders", json);
        Assert.Contains("5 days late", json);
        Assert.Contains("0000000001", json);
    }

    [Fact]
    public void BuildReleaseRequestNotificationCard_RendersDetailsAndActionButtons()
    {
        var requestedAt = new DateTimeOffset(2026, 10, 12, 1, 0, 0, TimeSpan.Zero);
        var attachment = TeamsCardBuilder.BuildReleaseRequestNotificationCard(
            salesOrderNumber: "0000013771",
            requestedBy: "DEV-244",
            salesOrg: "TV01",
            formattedNetValue: "780,000 VND",
            comment: "Urgent order for project",
            requestedAt: requestedAt);

        var json = JsonConvert.SerializeObject(attachment.Content);
        Assert.Contains("Release approval requested", json);
        Assert.Contains("0000013771", json);
        Assert.Contains("DEV-244", json);
        Assert.Contains("TV01", json);
        Assert.Contains("780,000 VND", json);
        Assert.Contains("Urgent order for project", json);
        Assert.Contains("view_details", json);
        Assert.Contains("approve_so", json);
        Assert.Contains("reject_approval", json);
    }

    [Fact]
    public void BuildOrderApprovedNotificationCard_RendersDetailsAndActionButtons()
    {
        var approvedAt = new DateTimeOffset(2026, 10, 12, 17, 58, 0, TimeSpan.Zero);
        var attachment = TeamsCardBuilder.BuildOrderApprovedNotificationCard(
            salesOrderNumber: "0000013798",
            approvedBy: "DEV-230",
            salesOrg: "TV01",
            formattedNetValue: "212,500 VND",
            comment: "Approved by manager",
            approvedAt: approvedAt);

        var json = JsonConvert.SerializeObject(attachment.Content);
        Assert.Contains("Sales order approved & released", json);
        Assert.Contains("0000013798", json);
        Assert.Contains("DEV-230", json);
        Assert.Contains("TV01", json);
        Assert.Contains("212,500 VND", json);
        Assert.Contains("Approved by manager", json);
        Assert.Contains("view_details", json);
    }

    [Fact]
    public void BuildOrderRejectedNotificationCard_RendersDetailsAndActionButtons()
    {
        var declinedAt = new DateTimeOffset(2026, 10, 12, 18, 0, 0, TimeSpan.Zero);
        var attachment = TeamsCardBuilder.BuildOrderRejectedNotificationCard(
            salesOrderNumber: "0000013798",
            declinedBy: "DEV-230",
            salesOrg: "TV01",
            formattedNetValue: "212,500 VND",
            comment: "Budget exceeded",
            declinedAt: declinedAt);

        var json = JsonConvert.SerializeObject(attachment.Content);
        Assert.Contains("Release request declined", json);
        Assert.Contains("0000013798", json);
        Assert.Contains("DEV-230", json);
        Assert.Contains("TV01", json);
        Assert.Contains("212,500 VND", json);
        Assert.Contains("Budget exceeded", json);
        Assert.Contains("view_details", json);
    }

    private static OrderApprovalRequest Approval(
        string soNumber,
        string requestedBy,
        string salesOrg,
        string comment,
        DateTimeOffset requestedAt) =>
        new()
        {
            Id = Guid.NewGuid(),
            SoNumber = soNumber,
            RequestedBySapUser = requestedBy,
            SalesOrg = salesOrg,
            Comment = comment,
            Status = ApprovalStatus.Pending,
            RequestedAt = requestedAt
        };
}
