using System.Web.Http;
using LightInject;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Diagnostics;
using Newtonsoft.Json.Serialization;
using Owin;
using Topshelf;

namespace Hse.CqrsWorkShop.Api
{
    public class DomainService : ServiceControl
    {
        public bool Start(HostControl hostControl)
        {
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }
    }

    public class Startup : ApiBootstrapper
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureWebApi(app);
            //ConfigureStaticFiles(IAppBuilder app);
            ConfigureDiagnostics(app);
        }

        //private static void ConfigureStaticFiles(IAppBuilder app)
        //{
        //    app.UseFileServer(new FileServerOptions
        //    {
        //        RequestPath = PathString.Empty,
        //        FileSystem = new PhysicalFileSystem(@".\Web\StaticPages")
        //    });

        //    app.UseStaticFiles(new StaticFileOptions
        //    {
        //        RequestPath = new PathString("/css"),
        //        FileSystem = new PhysicalFileSystem(@".\Web\Content\css"),
        //    });

        //    app.UseStaticFiles(new StaticFileOptions
        //    {
        //        RequestPath = new PathString("/js"),
        //        FileSystem = new PhysicalFileSystem(@".\Web\Content\js"),
        //    });
        //}

        private static void ConfigureRouting(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.MapHttpAttributeRoutes();
        }

        private static void ConfigureFormatters(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Formatters.Remove(httpConfiguration.Formatters.XmlFormatter);
            httpConfiguration.Formatters.Remove(httpConfiguration.Formatters.FormUrlEncodedFormatter);
            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
        }

        private static void ConfigureDiagnostics(IAppBuilder app)
        {
#if DEBUG
            app.UseErrorPage(new ErrorPageOptions
            {
                //Shows the OWIN environment dictionary keys and values.
                //This detail is enabled by default if you are running your app from VS unless disabled in code. 
                ShowEnvironment = true,
                //Hides cookie details
                ShowCookies = true,
                //Shows the lines of code throwing this exception.
                //This detail is enabled by default if you are running your app from VS unless disabled in code. 
                ShowSourceCode = true
            });
#endif

            app.UseWelcomePage();
        }

        private void ConfigureWebApi(IAppBuilder app)
        {
            app.Map("/api", builder =>
            {
                var httpConfiguration = new HttpConfiguration();

                this.RegisterApiControllers();
                this.EnableWebApi(httpConfiguration);

                ConfigureRouting(httpConfiguration);
                ConfigureFormatters(httpConfiguration);

                builder.UseCors(CorsOptions.AllowAll);

                builder.UseWebApi(httpConfiguration);
            });
        }
    }
}