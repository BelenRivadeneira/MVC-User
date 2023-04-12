using MVC.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Crea el host
            var host = CreateHostBuilder(args).Build();

            // Verifica si la base de datos existe, si no la crea
            CreateDbIfNotExists(host);

            // Ejecuta la aplicación
            host.Run();
        }

        // Función para crear la base de datos si no existe
        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<UserContext>();
                    // Inicializa la base de datos
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Configura el Startup
                    webBuilder.UseStartup<Startup>();
                });
    }
}
