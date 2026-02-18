namespace Allegory.Axiom.DependencyInjection;

internal class TransientProductManager : ITransientService {}

internal class ScopedProductManager : IScopedService {}

internal class SingletonProductManager : ISingletonService {}

internal class InheritedProductManager : SingletonProductManager {}

internal class GenericProductManager<T> : ITransientService {}

internal interface ITransientOrderManager : ITransientService {}

internal interface IScopedOrderManager {}

internal interface ISingletonOrderManager : ISingletonService {}

internal class TransientOrderManager : ITransientOrderManager {}

internal class ScopedOrderManager : IScopedOrderManager, IScopedService {}

internal class ExtendedNameSingletonOrderManager : ISingletonOrderManager {}

internal interface IOrderManager : ITransientService {}

internal class OrderNameNotMatchedManager : IOrderManager {}

internal interface IOrderRepository<T> : ITransientService {}

internal class OrderRepository<T> : IOrderRepository<T> {}

internal class OrderRepository : IOrderRepository<int> {}