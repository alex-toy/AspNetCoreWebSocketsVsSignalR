using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebSockerServer
{
    public static class AppBuilderExtension
    {
        public static void AddRegularConfig(this ApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
