using System;
using Microsoft.Extensions.DependencyInjection;

namespace Allegory.Axiom.DependencyInjection;

internal interface IDependencyAttribute
{
    Type ServiceType { get; }
    ServiceLifetime? Lifetime { get; set; }
    RegistrationStrategy Strategy { get; set; }
    object? ServiceKey { get; set; }
}