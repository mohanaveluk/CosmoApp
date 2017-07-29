using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;

namespace Cosmo.Forwarder
{
    public class StartService
    {
        Type valuesControllerType = typeof(Cosmo.Forwarder.StatusController);

        public void Configuration(IAppBuilder appBuilder)
        {
            var configuration = new HttpConfiguration();
            configuration.MapHttpAttributeRoutes();

            configuration.Routes.MapHttpRoute(
                name: "OwinAPI",
                routeTemplate: "owin/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            configuration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            appBuilder.UseWebApi(configuration);
        }
    }
}
