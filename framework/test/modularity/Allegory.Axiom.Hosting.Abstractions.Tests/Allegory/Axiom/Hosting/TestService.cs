using Allegory.Axiom.DependencyInjection;

namespace Allegory.Axiom.Hosting;

public class TestService : ITransientService
{
    public int GetNumber() => 1;
}