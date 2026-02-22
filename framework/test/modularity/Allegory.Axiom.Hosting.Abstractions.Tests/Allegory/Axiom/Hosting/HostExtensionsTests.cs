using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Testing.Platform.Services;
using Shouldly;
using Xunit;

namespace Allegory.Axiom.Hosting;

public class HostExtensionsTests
{
    [Fact]
    public async ValueTask ShouldConfigureApplication()
    {
        var builder = Host.CreateApplicationBuilder();
        await builder.ConfigureApplicationAsync();

        AxiomHostingAbstractionsTestsPackage.ConfigureApplication.ShouldBeTrue();
        AxiomHostingAbstractionsTestsPackage.PostConfigureApplication.ShouldBeTrue();
    }

    [Fact]
    public async ValueTask ShouldDiscoverAssembliesAndRegisterServices()
    {
        var builder = Host.CreateApplicationBuilder();
        await builder.ConfigureApplicationAsync();
        var host = builder.Build();
        await host.InitializeApplicationAsync();

        var application = host.Services.GetRequiredService<AxiomHostApplication>();

        application.Assemblies.ShouldContain(typeof(HostExtensionsTests).Assembly);
        application.Assemblies.ShouldContain(typeof(Assembly1.Assembly1Package).Assembly);
        application.Assemblies.ShouldContain(typeof(Assembly2.Assembly2Package).Assembly);
        application.Assemblies.ShouldContain(typeof(Assembly3.Assembly3Package).Assembly);

        var service = host.Services.GetService<TestService>();
        service.ShouldNotBeNull();
        service.GetNumber().ShouldBe(1);
    }

    [Fact]
    public async ValueTask ShouldInitializeApplication()
    {
        var builder = Host.CreateApplicationBuilder();
        await builder.ConfigureApplicationAsync();
        var host = builder.Build();
        await host.InitializeApplicationAsync();

        AxiomHostingAbstractionsTestsPackage.InitializeApplication.ShouldBeTrue();
    }
}