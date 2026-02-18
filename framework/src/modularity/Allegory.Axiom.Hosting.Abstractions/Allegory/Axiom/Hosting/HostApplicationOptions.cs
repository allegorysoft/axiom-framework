using System.Reflection;
using Allegory.Axiom.DependencyInjection;

namespace Allegory.Axiom.Hosting;

public class HostApplicationOptions
{
    //Local plugin, Remote plugin

    public Assembly? StartupAssembly { get; set; }
    public AssemblyDependencyRegistrar? DependencyRegistrar { get; set; }

    internal HostApplicationOptions() {}
}