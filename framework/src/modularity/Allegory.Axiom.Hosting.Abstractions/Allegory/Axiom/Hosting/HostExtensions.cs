using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allegory.Axiom.Hosting;

public static class HostExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public async ValueTask ConfigureApplicationAsync(
            Action<HostApplicationOptions>? optionsAction = null)
        {
            var options = new HostApplicationOptions();
            optionsAction?.Invoke(options);

            var application = HostApplication.Create(builder, options);

            foreach (var assembly in application.Assemblies)
            {
                var configureMethod = assembly.GetTypes().SingleOrDefault(
                        t => typeof(IConfigureApplication).IsAssignableFrom(t) &&
                             t is {IsClass: true, IsAbstract: false})?
                    .GetMethod(nameof(IConfigureApplication.ConfigureAsync));

                if (configureMethod != null)
                {
                    await (ValueTask) configureMethod.Invoke(null, [builder])!;
                }
            }

            foreach (var assembly in application.Assemblies)
            {
                var configureMethod = assembly.GetTypes().SingleOrDefault(
                        t => typeof(IPostConfigureApplication).IsAssignableFrom(t) &&
                             t is {IsClass: true, IsAbstract: false})?
                    .GetMethod(nameof(IPostConfigureApplication.PostConfigureAsync));

                if (configureMethod != null)
                {
                    await (ValueTask) configureMethod.Invoke(null, [builder])!;
                }
            }
        }
    }

    extension(IHost host)
    {
        public async ValueTask InitializeApplicationAsync()
        {
            //TODO: Add concurrent parameter

            var application = host.Services.GetRequiredService<HostApplication>();

            foreach (var assembly in application.Assemblies)
            {
                var configureMethod = assembly.GetTypes().SingleOrDefault(
                        t => typeof(IInitializeApplication).IsAssignableFrom(t) &&
                             t is {IsClass: true, IsAbstract: false})?
                    .GetMethod(nameof(IInitializeApplication.InitializeAsync));

                if (configureMethod != null)
                {
                    await (ValueTask) configureMethod.Invoke(null, [host])!;
                }
            }
        }
    }
}