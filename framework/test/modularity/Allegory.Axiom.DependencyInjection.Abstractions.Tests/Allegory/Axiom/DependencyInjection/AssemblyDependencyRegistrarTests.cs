using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Allegory.Axiom.DependencyInjection;

public class AssemblyDependencyRegistrarTests
{
    private AssemblyDependencyRegistrar Registrar { get; } = new(new ServiceCollection());

    public AssemblyDependencyRegistrarTests()
    {
        Registrar.Register(typeof(AssemblyDependencyRegistrarTests).Assembly);
    }

    [Fact]
    public void RegisterClassImplementationsViaMarkerInterfaces()
    {
        var transient = Registrar.ServiceCollection.FirstOrDefault(
            f => f.ServiceType == typeof(TransientProductManager));
        var scoped = Registrar.ServiceCollection.FirstOrDefault(
            f => f.ServiceType == typeof(ScopedProductManager));
        var singleton = Registrar.ServiceCollection.FirstOrDefault(
            f => f.ServiceType == typeof(SingletonProductManager));

        transient.ShouldNotBeNull();
        scoped.ShouldNotBeNull();
        singleton.ShouldNotBeNull();

        transient.ImplementationType.ShouldBe(typeof(TransientProductManager));
        scoped.ImplementationType.ShouldBe(typeof(ScopedProductManager));
        singleton.ImplementationType.ShouldBe(typeof(SingletonProductManager));

        transient.Lifetime.ShouldBe(ServiceLifetime.Transient);
        scoped.Lifetime.ShouldBe(ServiceLifetime.Scoped);
        singleton.Lifetime.ShouldBe(ServiceLifetime.Singleton);
    }

    [Fact]
    public void RegisterInterfaceImplementation()
    {
        var service = Registrar.ServiceCollection.FirstOrDefault(
            f => f.ServiceType == typeof(IOrderManager));

        service.ShouldNotBeNull();
        service.ImplementationType.ShouldBe(typeof(OrderManager));
        service.Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }
}

public class TransientProductManager : ITransientService {}

public class ScopedProductManager : IScopedService {}

public class SingletonProductManager : ISingletonService {}

public interface IOrderManager : IScopedService {}

public class OrderManager : IOrderManager {}