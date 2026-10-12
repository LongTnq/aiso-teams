using AISO.Bot;
using Xunit;

namespace AISO.UnitTests;

public class QuickCreateOrderTests
{
    [Theory]
    [InlineData("tạo nhanh đơn hàng cho khách 1005000016")]
    [InlineData("tao nhanh don hang cho khach 1005000016")]
    [InlineData("tạo đơn nhanh khách 1005000016 sp JSH00819-214-A0 sl 2")]
    [InlineData("tạo trực tiếp đơn hàng")]
    [InlineData("tạo luôn đơn hàng cho khách 1005000016")]
    [InlineData("tạo đơn: khách 1005000016, vật liệu JSH00819-214-A0")]
    [InlineData("demo tạo đơn")]
    [InlineData("tạo đơn demo")]
    [InlineData("demo create order")]
    [InlineData("quick create customer 1005000016")]
    [InlineData("quick-create")]
    [InlineData("/quick-create")]
    [InlineData("tạo đơn cho khách 1005000016 không cần qua thẻ")]
    public void IsQuickCreateIntent_MatchesQuickCreatePhrases(string message)
    {
        Assert.True(TeamsBot.IsQuickCreateIntent(message));
    }

    [Theory]
    [InlineData("tạo đơn hàng")]
    [InlineData("tạo đơn")]
    [InlineData("create order")]
    [InlineData("create sales order")]
    [InlineData("danh sách đơn hàng")]
    [InlineData("chi tiết đơn 0000013813")]
    [InlineData("hello")]
    public void IsQuickCreateIntent_PreservesNormalWizardAndOtherCommands(string message)
    {
        Assert.False(TeamsBot.IsQuickCreateIntent(message));
    }

    [Fact]
    public void ParseQuickCreateParams_ExtractsAllParametersCorrectly()
    {
        var prompt = "tạo nhanh đơn hàng cho khách 1005000016 vật liệu JSH00819-214-A0 số lượng 3";
        var (customer, material, qty) = TeamsBot.ParseQuickCreateParams(prompt);

        Assert.Equal("1005000016", customer);
        Assert.Equal("JSH00819-214-A0", material);
        Assert.Equal(3m, qty);
    }

    [Fact]
    public void ParseQuickCreateParams_WhenDefault_ProvidesSafeDemoFallbacks()
    {
        var prompt = "demo tạo đơn";
        var (customer, material, qty) = TeamsBot.ParseQuickCreateParams(prompt);

        Assert.Equal("1005000016", customer);
        Assert.Equal("JSH00819-214-A0", material);
        Assert.Equal(1m, qty);
    }

    [Fact]
    public void ParseQuickCreateParams_EnglishSyntax_ExtractsCorrectly()
    {
        var prompt = "quick create customer 1005000016 material JSH00819-212-A0 qty 5";
        var (customer, material, qty) = TeamsBot.ParseQuickCreateParams(prompt);

        Assert.Equal("1005000016", customer);
        Assert.Equal("JSH00819-212-A0", material);
        Assert.Equal(5m, qty);
    }
}
