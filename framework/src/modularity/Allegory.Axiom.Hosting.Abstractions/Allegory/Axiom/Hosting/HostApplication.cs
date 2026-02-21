using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Allegory.Axiom.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;

namespace Allegory.Axiom.Hosting;

public sealed class HostApplication
{
    internal static string DependencyInjectionAssemblyName => "Allegory.Axiom.DependencyInjection.Abstractions";

    public Guid Id { get; } = Guid.NewGuid();
    public Assembly StartupAssembly => _options.StartupAssembly!;

    private readonly HashSet<Assembly> _assemblies = [];
    public IReadOnlyCollection<Assembly> Assemblies => _assemblies;

    private readonly HostApplicationOptions _options;

    private HostApplication(HostApplicationOptions options)
    {
        _options = options;
    }

    internal static HostApplication Create(
        IHostApplicationBuilder builder,
        HostApplicationOptions options)
    {
        //TODO: Implement Local, Remote plugins.
        options.StartupAssembly ??= Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(options.StartupAssembly);

        var application = new HostApplication(options);
        builder.Services.AddSingleton(application);
        application.FillPackages();
        application.RegisterServices(builder.Services);

        return application;
    }

    private void FillPackages()
    {
        var dependencyContext = DependencyContext.Load(StartupAssembly);
        if (dependencyContext == null)
        {
            //TODO: To get all packages as references we should create a CLI command.
            //Such as; axm create-package-references 
            FillPackagesByReferencedAssemblies();
        }
        else
        {
            FillPackagesByDependencyContext(dependencyContext);
        }
    }

    private void FillPackagesByReferencedAssemblies()
    {
        foreach (var assembly in StartupAssembly.GetReferencedAssemblies())
        {
            _assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyName(assembly));
        }
    }

    private void FillPackagesByDependencyContext(DependencyContext context)
    {
        var checkedLibraries = new Dictionary<string, bool>();

        foreach (var library in context.RuntimeLibraries)
        {
            if (!HasTransitiveDependency(context, library, DependencyInjectionAssemblyName, checkedLibraries))
            {
                continue;
            }

            foreach (var assemblyName in library.GetDefaultAssemblyNames(context))
            {
                _assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName));
            }
        }
    }

    private bool HasTransitiveDependency(
        DependencyContext context,
        RuntimeLibrary library,
        string targetName,
        Dictionary<string, bool> checkedLibraries)
    {
        if (checkedLibraries.TryGetValue(library.Name, out var result))
        {
            return result;
        }

        // To include DependencyInjection.Abstractions, add " || library.Name == targetName" to the condition
        if (library.Dependencies.Any(d => d.Name == targetName))
        {
            return checkedLibraries[library.Name] = true;
        }

        foreach (var dependency in library.Dependencies)
        {
            var dependencyLibrary = context.RuntimeLibraries.FirstOrDefault(d => d.Name == dependency.Name);
            if (dependencyLibrary != null &&
                HasTransitiveDependency(context, dependencyLibrary, targetName, checkedLibraries))
            {
                return checkedLibraries[library.Name] = true;
            }
        }

        return checkedLibraries[library.Name] = false;
    }

    private void RegisterServices(IServiceCollection services)
    {
        var registrar = _options.DependencyRegistrar ?? new AssemblyDependencyRegistrar(services);
        foreach (var assembly in Assemblies)
        {
            registrar.Register(assembly);
        }
    }
}