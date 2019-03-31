using Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace iDEdge
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.Run(context =>
            {
                string song = context.Request.Path.ToString().Trim('/');
                int result = Program.Make(song);
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync(result.ToString());
            });
        }
    }

    public class IdeController : ApiController
    {
        public string Get()
        {
            return "架构师 www.itsvse.com";
        }
    }
}
