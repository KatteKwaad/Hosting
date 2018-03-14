#if NET461
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GenericHostSample
{
    public static class ServiceBaseLifetimeHostExtensions
    {
        public static IHostBuilder UseServiceBaseLifetime(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) => services.AddSingleton<IHostLifetime, ServiceBaseLifetime>());
        }

        public static Task RunAsServiceAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken = default)
        {
            return hostBuilder.UseServiceBaseLifetime().Build().RunAsync(cancellationToken);
        }
    }

    public class ServiceBaseLifetime : ServiceBase, IHostLifetime
    {
        private Func<object, Task> _startCallback;
        private object _startState;

        public ServiceBaseLifetime(IApplicationLifetime applicationLifetime)
        {
            ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        }

        private IApplicationLifetime ApplicationLifetime { get; }

        public Task RegisterDelayStartCallbackAsync(Func<object, Task> callback, object state)
        {
            _startCallback = callback ?? throw new ArgumentNullException(nameof(callback));
            _startState = state;

            ApplicationLifetime.ApplicationStopping.Register(Stop);

            Task.Run(() => Run(this)); // Otherwise this would block and prevent Start from finishing.
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Stop();
            return Task.CompletedTask;
        }

        protected override void OnStart(string[] args)
        {
            _startCallback(_startState);
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            ApplicationLifetime.StopApplication();
            base.OnStop();
        }
    }
}
#endif