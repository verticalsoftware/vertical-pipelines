using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vertical.Examples.Shared;

namespace Vertical.Examples.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureServices(services =>
                        {
                            services.ConfigureCustomPipeline();
                            services.AddSharedServices();
                            services.AddSwaggerGen();
                            services.AddMvc();
                        })
                        .Configure(app =>
                        {
                            app.UseSwagger();

                            app.UseSwaggerUI(cfg =>
                            {
                                cfg.SwaggerEndpoint("/swagger/v1/swagger.json", "Example API");
                                cfg.RoutePrefix = string.Empty;
                            });
            
                            app.UseRouting();

                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                            });
                        });
                })
                .Build();

            host.Run();
        }
    }
}
