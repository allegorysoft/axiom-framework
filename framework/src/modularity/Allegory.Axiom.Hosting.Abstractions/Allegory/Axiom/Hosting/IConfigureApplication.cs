using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Allegory.Axiom.Hosting;

public interface IConfigureApplication
{
    static abstract ValueTask ConfigureAsync(IHostApplicationBuilder builder);
}