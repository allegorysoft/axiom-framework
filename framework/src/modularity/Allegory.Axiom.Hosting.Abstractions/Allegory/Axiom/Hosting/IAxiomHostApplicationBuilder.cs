using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Allegory.Axiom.Hosting;

public interface IAxiomHostApplicationBuilder
{
    ValueTask<AxiomHostApplication> BuildAsync(IHostApplicationBuilder builder, Assembly startupAssembly);
}