using System.Reflection;
using Allegory.Axiom.DependencyInjection;

namespace Allegory.Axiom.Hosting;

public class AxiomHostApplicationOptions
{
    public Assembly? StartupAssembly { get; set; }
    public IAxiomHostApplicationBuilder? ApplicationBuilder { get; set; }
    public AssemblyDependencyRegistrar? DependencyRegistrar { get; set; }

    //Local plugin, Remote plugin

    internal AxiomHostApplicationOptions() {}
}