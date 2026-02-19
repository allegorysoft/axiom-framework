using System;
using System.Threading.Tasks;
using Allegory.Axiom.Hosting;
using Microsoft.Extensions.Hosting;

namespace Allegory.Axiom.Assembly1;

public class Assembly1Package : IConfigureApplication, IPostConfigureApplication
{
    public static ValueTask ConfigureAsync(IHostApplicationBuilder builder)
    {
        Console.WriteLine("Assembly1Package configure executed");
        return ValueTask.CompletedTask;
    }

    public static ValueTask PostConfigureAsync(IHostApplicationBuilder builder)
    {
        return ValueTask.CompletedTask;
    }
}