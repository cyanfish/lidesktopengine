using Xunit;

namespace DesktopEngine.Tests;

public class Class1
{
    [Fact]
    public void TestApi()
    {
        EngineConfig config = new()
        {
            AuthToken = "456"
        };
        Assert.Equal(456, ExternalEngineApi.StartListening(ref config));
        Assert.Equal(2, ExternalEngineApi.GetStatus());
        Assert.Equal(3, ExternalEngineApi.StopListening());
    }
}