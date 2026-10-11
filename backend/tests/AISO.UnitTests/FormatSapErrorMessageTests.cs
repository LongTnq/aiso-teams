using AISO.Bot;
using AISO.Domain.SalesOrders;
using AISO.SapIntegration;
using Xunit;

namespace AISO.UnitTests;

public class FormatSapErrorMessageTests
{
    [Fact]
    public void FormatSapErrorMessage_WhenSimulateErrorPresent_ShowsOnlyFailingMaterial()
    {
        var rawMsg = "Chưa có giá PR00 cho item (item:material):10 :JSH02493-A2-S1-A0 , 20 :JSH02493-Z02-A0Sales area TV01/T1, VNDMaterial JSH02493-Z02-A0 is not defined for sales";
        var lineItems = new List<CreateSalesOrderItemDto>
        {
            new() { Material = "JSH02493-A2-S1-A0", OrderQty = 4, Plant = "TV01", Unit = "EA" },
            new() { Material = "JSH02493-Z02-A0", OrderQty = 2, Plant = "TV01", Unit = "EA" },
            new() { Material = "JSH00819-066-A1", OrderQty = 3, Plant = "TV01", Unit = "EA" },
        };

        var result = TeamsBot.FormatSapErrorMessage(rawMsg, lineItems, "TV01", "T1", "SAP_ERROR");

        Assert.Contains("[SAP_ERROR] Item validation error", result);
        Assert.Contains("Material JSH02493-Z02-A0 is not defined for sales", result);
        Assert.DoesNotContain("JSH02493-A2-S1-A0", result);
        Assert.DoesNotContain("JSH00819-066-A1", result);
        Assert.DoesNotContain("Missing PR00 price for item", result);
        Assert.Contains("Sales area TV01/T1", result);
        Assert.DoesNotContain("VNDMaterial", result);
    }

    [Fact]
    public void FormatSapErrorMessage_WhenMissingPr00_ShowsOnlyReportedItems()
    {
        var rawMsg = "Chưa có giá PR00 cho item (item:material):20 :AC306Sales area TV01/T1, VND";
        var lineItems = new List<CreateSalesOrderItemDto>
        {
            new() { Material = "65_TV", OrderQty = 1, Plant = "TV01", Unit = "EA" },
            new() { Material = "AC306", OrderQty = 2, Plant = "TV01", Unit = "EA" },
            new() { Material = "DMSG-020", OrderQty = 3, Plant = "TV01", Unit = "EA" },
        };

        var result = TeamsBot.FormatSapErrorMessage(rawMsg, lineItems, "TV01", "T1", "SAP_ERROR");

        Assert.Contains("[SAP_ERROR] Missing PR00 price for item (item: material)", result);
        Assert.Contains("20 :AC306", result);
        Assert.DoesNotContain("65_TV", result);
        Assert.DoesNotContain("DMSG-020", result);
        Assert.Contains("Sales area TV01/T1", result);
    }

    [Fact]
    public void FormatSapErrorMessage_WhenMultipleItemsMissingPr00_ShowsBothReportedItems()
    {
        var rawMsg = "Chưa có giá PR00 cho item (item:material):10 :65_TV , 20 :AC306Sales area TV01/T1, VND";
        var lineItems = new List<CreateSalesOrderItemDto>
        {
            new() { Material = "65_TV", OrderQty = 1, Plant = "TV01", Unit = "EA" },
            new() { Material = "AC306", OrderQty = 2, Plant = "TV01", Unit = "EA" },
            new() { Material = "DMSG-020", OrderQty = 3, Plant = "TV01", Unit = "EA" },
        };

        var result = TeamsBot.FormatSapErrorMessage(rawMsg, lineItems, "TV01", "T1", "SAP_ERROR");

        Assert.Contains("[SAP_ERROR] Missing PR00 price for item (item: material)", result);
        Assert.Contains("10 :65_TV", result);
        Assert.Contains("20 :AC306", result);
        Assert.DoesNotContain("DMSG-020", result);
        Assert.Contains("Sales area TV01/T1", result);
    }

    [Fact]
    public void FormatSapErrorMessage_WhenStandardErrorMessage_PrependsErrorCode()
    {
        var rawMsg = "Customer 1005000008 is not defined in sales area TV01/T1/10";
        var result = TeamsBot.FormatSapErrorMessage(rawMsg, null, "TV01", "T1", "SAP_ERROR");

        Assert.Equal("[SAP_ERROR] Customer 1005000008 is not defined in sales area TV01/T1/10", result);
    }

    [Fact]
    public void FormatSapErrorMessage_WhenSimulateErrorGeneric_FormatsGenericMessage()
    {
        var rawMsg = "Chưa có giá PR00 cho item (item:material):10 :65_TVSales area TV01/T1, VNDPricing error in procedure RVAA01";
        var lineItems = new List<CreateSalesOrderItemDto>
        {
            new() { Material = "65_TV", OrderQty = 1, Plant = "TV01", Unit = "EA" }
        };

        var result = TeamsBot.FormatSapErrorMessage(rawMsg, lineItems, "TV01", "T1", "SAP_ERROR");

        Assert.Contains("[SAP_ERROR] Item validation error", result);
        Assert.Contains("Pricing error in procedure RVAA01", result);
        Assert.Contains("Sales area TV01/T1", result);
    }

    [Fact]
    public void FormatSapErrorMessage_WhenMsgNullOrEmpty_ReturnsOriginal()
    {
        Assert.Null(TeamsBot.FormatSapErrorMessage(null!));
        Assert.Equal("", TeamsBot.FormatSapErrorMessage(""));
        Assert.Equal("   ", TeamsBot.FormatSapErrorMessage("   "));
    }
}
