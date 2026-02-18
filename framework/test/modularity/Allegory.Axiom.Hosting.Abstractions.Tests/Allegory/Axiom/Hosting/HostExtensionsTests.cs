using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Allegory.Axiom.Hosting;

public class HostExtensionsTests
{
    [Fact]
    public async ValueTask Test1()
    {
        var builder = Host.CreateApplicationBuilder();
        await builder.ConfigureApplicationAsync(o =>
        {
            o.StartupAssembly = GetType().Assembly;
        });
        var host = builder.Build();

        await host.InitializeApplicationAsync();
        //await host.RunAsync(token: TestContext.Current.CancellationToken);
    }
}