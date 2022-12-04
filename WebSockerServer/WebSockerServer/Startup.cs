using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebSockerServer.Middleware;

namespace WebSockerServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebSocketManager();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebSockets();

            app.UseWebSocketServer();

            app.Run(async context =>
            {
                Console.WriteLine("hello from the third request delegate");
                await context.Response.WriteAsync("Hello from the third request delegate");
            });
        }

        public void WriteRequestParam(HttpContext context)
        {
            Console.WriteLine("request methood : " + context.Request.Method);
            Console.WriteLine("request protocol : " + context.Request.Protocol);

            if (context.Request.Headers != null)
            {
                foreach (var header in context.Request.Headers)
                {
                    Console.WriteLine($"--> {header.Key} : {header.Value}");
                }
            }
        }
    }
}
