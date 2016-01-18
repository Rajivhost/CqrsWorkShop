using System;
using System.Configuration;
using Owin;
using Serilog;
using Topshelf;
using TopShelf.Owin;

namespace Hse.CqrsWorkShop.Api
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.RollingFile(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\CqrsWorkShop-{Date}.log")
#if DEBUG
.WriteTo.ColoredConsole()
            .WriteTo.Trace()
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif

.CreateLogger();

            HostFactory.Run(x =>
            {
                x.SetServiceName("CqrsWorkShopRead");
                x.SetDisplayName("CqrsWorkShop Read");
                x.SetDescription("Windows Service for CqrsWorkShop.");

                x.RunAsNetworkService();
                x.StartAutomatically();
                x.UseSerilog();
                x.Service<EventHandlerService>(service =>
                {

                    service.OwinEndpoint(app =>
                    {
                        app.Domain = "localhost";
                        app.Port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ListeningPort"));

                        service.ConstructUsing(builder => new EventHandlerService());
                        service.WhenStarted((startUp, hostControl) => startUp.Start(hostControl));
                        service.WhenStopped((startUp, hostControl) => startUp.Stop(hostControl));

                        app.ConfigureAppBuilder(appBuilder => new Startup().Configuration(appBuilder));
                    });
                });
                x.EnableServiceRecovery(r => r.RestartService(1));
            });
        }
    }
}
