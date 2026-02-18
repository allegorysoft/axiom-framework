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
    public void RegisterClassesWithCorrectLifetimeViaMarkerInterfaces()
    {
        var transient = Registrar.ServiceCollection.FirstOrDefault(
            s => s.ServiceType == typeof(TransientProductManager));
        transient.ShouldNotBeNull();
        transient.ImplementationType.ShouldBe(typeof(TransientProductManager));
        transient.Lifetime.ShouldBe(ServiceLifetime.Transient);

        var scoped = Registrar.ServiceCollection.FirstOrDefault(
            s => s.ServiceType == typeof(ScopedProductManager));
        scoped.ShouldNotBeNull();
        scoped.ImplementationType.ShouldBe(typeof(ScopedProductManager));
        scoped.Lifetime.ShouldBe(ServiceLifetime.Scoped);

        var singleton = Registrar.ServiceCollection.FirstOrDefault(
            s => s.ServiceType == typeof(SingletonProductManager));
        singleton.ShouldNotBeNull();
        singleton.ImplementationType.ShouldBe(typeof(SingletonProductManager));
        singleton.Lifetime.ShouldBe(ServiceLifetime.Singleton);
    }

    [Fact]
    public void RegisterDerivedClassWithInheritedLifetimeViaMarkerInterface()
    {
        var service = Registrar.ServiceCollection.FirstOrDefault(
            s => s.ServiceType == typeof(InheritedProductManager));

        service.ShouldNotBeNull();
        service.ImplementationType.ShouldBe(typeof(InheritedProductManager));
        service.Lifetime.ShouldBe(ServiceLifetime.Singleton);
    }

    [Fact]
    public void RegisterGenericClassViaMarkerInterface()
    {
        var service = Registrar.ServiceCollection.FirstOrDefault(
            s => s.ServiceType == typeof(GenericProductManager<>));

        service.ShouldNotBeNull();
        service.ImplementationType.ShouldBe(typeof(GenericProductManager<>));
        service.Lifetime.ShouldBe(ServiceLifetime.Transient);
    }

    [Fact]
    public void RegisterInterfacesWithCorrectLifetimeViaMarkerInterfaces()
    {
        var transient = Registrar.ServiceCollection.FirstOrDefault(
            s => s.ServiceType == typeof(ITransientOrderManager));
        transient.ShouldNotBeNull();
        transient.ImplementationType.ShouldBe(typeof(TransientOrderManager));
        transient.Lifetime.ShouldBe(ServiceLifetime.Transient);

        var scoped = Registrar.ServiceCollection.FirstOrDefault(
            s => s.ServiceType == typeof(IScopedOrderManager));
        scoped.ShouldNotBeNull();
        scoped.ImplementationType.ShouldBe(typeof(ScopedOrderManager));
        scoped.Lifetime.ShouldBe(ServiceLifetime.Scoped);

        var singleton = Registrar.ServiceCollection.FirstOrDefault(
            s => s.ServiceType == typeof(ISingletonOrderManager));
        singleton.ShouldNotBeNull();
        singleton.ImplementationType.ShouldBe(typeof(ExtendedNameSingletonOrderManager));
        singleton.Lifetime.ShouldBe(ServiceLifetime.Singleton);

        // Concrete classes are also registered alongside their interfaces (3 total)
        Registrar.ServiceCollection.Count(s =>
                s.ServiceType == typeof(TransientOrderManager) ||
                s.ServiceType == typeof(ScopedOrderManager) ||
                s.ServiceType == typeof(ExtendedNameSingletonOrderManager))
            .ShouldBe(3);
    }

    [Fact]
    public void SkipInterfaceRegistrationWhenNameDoesNotMatchViaMarkerInterface()
    {
        var service = Registrar.ServiceCollection.FirstOrDefault(
            s => s.ServiceType == typeof(IOrderManager));
        service.ShouldBeNull();

        var implementation = Registrar.ServiceCollection.FirstOrDefault(
            s => s.ServiceType == typeof(OrderNameNotMatchedManager));
        implementation.ShouldNotBeNull();
        implementation.ImplementationType.ShouldBe(typeof(OrderNameNotMatchedManager));
        implementation.Lifetime.ShouldBe(ServiceLifetime.Transient);
    }

    [Fact]
    public void RegisterGenericInterfaceViaMarkerInterface()
    {
        var openGeneric = Registrar.ServiceCollection.FirstOrDefault(
            s => s.ServiceType == typeof(IOrderRepository<>));
        openGeneric.ShouldNotBeNull();
        openGeneric.ImplementationType.ShouldBe(typeof(OrderRepository<>));
        openGeneric.Lifetime.ShouldBe(ServiceLifetime.Transient);

        var closedGeneric = Registrar.ServiceCollection.FirstOrDefault(
            s => s.ServiceType == typeof(IOrderRepository<int>));
        closedGeneric.ShouldNotBeNull();
        closedGeneric.ImplementationType.ShouldBe(typeof(OrderRepository));
        closedGeneric.Lifetime.ShouldBe(ServiceLifetime.Transient);
    }
}