using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Allegory.Axiom.DependencyInjection;

internal readonly struct ImplementationType(Type type)
{
    public Type Type { get; } = type;
    public Type[] Interfaces { get; } = type.GetInterfaces();
    public DependencyAttribute? Attribute { get; } = type.GetCustomAttribute<DependencyAttribute>();
    public List<IDependencyAttribute> ServiceAttributes { get; } = type
        .GetCustomAttributes(typeof(DependencyAttribute<>))
        .OfType<IDependencyAttribute>()
        .ToList();

    public ServiceLifetime GetLifetime(IDependencyAttribute? attribute = null)
    {
        return TryGetLifetime(attribute) ?? throw new NullReferenceException("Service lifetime cannot be null.");
    }

    public ServiceLifetime? TryGetLifetime(IDependencyAttribute? attribute = null)
    {
        return
            attribute is {Lifetime: not null} ? attribute.Lifetime :// Specified attribute
            Attribute is {Lifetime: not null} ? Attribute.Lifetime :// Default attribute
            Interfaces.Any(i => i == typeof(ITransientService)) ? ServiceLifetime.Transient :// From implementation
            Interfaces.Any(i => i == typeof(IScopedService)) ? ServiceLifetime.Scoped :
            Interfaces.Any(i => i == typeof(ISingletonService)) ? ServiceLifetime.Singleton :
            null;
    }

    public bool ShouldRegister(Type serviceType)
    {
        var serviceName = serviceType.Name.StartsWith('I') ? serviceType.Name[1..] : serviceType.Name;
        var typeName = Type.Name;

        if (serviceType.IsGenericType)
        {
            serviceName = serviceName[..serviceName.IndexOf('`')];
        }

        if (Type.IsGenericType)
        {
            typeName = typeName[..typeName.IndexOf('`')];
        }

        return typeName.EndsWith(serviceName, StringComparison.OrdinalIgnoreCase);
    }

    public Type GetServiceType(Type serviceType)
    {
        if (Type.IsGenericTypeDefinition)
        {
            if (!serviceType.IsGenericType)
            {
                throw new InvalidOperationException($"'{serviceType}' is not a generic type.");
            }

            if (!Type.GetGenericArguments().SequenceEqual(serviceType.GetGenericArguments()))
            {
                throw new InvalidOperationException(
                    $"The generic arguments of '{serviceType}' do not match the generic arguments of '{Type}'.");
            }

            return serviceType.GetGenericTypeDefinition();
        }

        return serviceType;
    }
}