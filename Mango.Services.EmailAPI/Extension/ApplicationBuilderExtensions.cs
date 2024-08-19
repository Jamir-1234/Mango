using Mango.Services.EmailAPI.Messaging;
using System.Runtime.CompilerServices;

namespace Mango.Services.EmailAPI.Extension
{
    public static class ApplicationBuilderExtensions
    {
        public static IAzureServiceBusConusmer ServiceBusConusmer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConusmer = app.ApplicationServices.GetService<IAzureServiceBusConusmer>();
            var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStarted.Register(OnStop);
            return app;
        }

        private static void OnStart()
        {
            ServiceBusConusmer.Start();
        }
        private static void OnStop()
        {
            ServiceBusConusmer.Stop();
        }

        
    }
}
