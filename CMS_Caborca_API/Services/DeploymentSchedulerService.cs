using CMS_Caborca_API.Data;
using Microsoft.EntityFrameworkCore;

namespace CMS_Caborca_API.Services
{
    public class DeploymentSchedulerService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<DeploymentSchedulerService> _logger;

        public DeploymentSchedulerService(IServiceProvider services, ILogger<DeploymentSchedulerService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de despliegue programado iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<CaborcaContext>();

                        var config = await context.Configuraciones_Del_Sistema
                            .FirstOrDefaultAsync(c => c.Clave_Configuracion == "Deploy_Schedule", stoppingToken);

                        if (config != null && !string.IsNullOrEmpty(config.Valor_Configuracion))
                        {
                            if (DateTime.TryParse(config.Valor_Configuracion, out DateTime scheduleTime))
                            {
                                // Compara en tiempo universal o asume que el input viene con zona horaria adecuada
                                if (DateTime.Now >= scheduleTime || DateTime.UtcNow >= scheduleTime.ToUniversalTime())
                                {
                                    _logger.LogInformation($"Ejecutando despliegue programado... Hora: {DateTime.Now}");

                                    var records = await context.Contenidos_Paginas.ToListAsync(stoppingToken);

                                    foreach (var record in records)
                                    {
                                        if (!string.IsNullOrEmpty(record.Contenido_Borrador_Stage))
                                        {
                                            record.Contenido_Publicado_Produccion = record.Contenido_Borrador_Stage;
                                        }
                                    }

                                    // Limpiar programa
                                    config.Valor_Configuracion = "";
                                    
                                    await context.SaveChangesAsync(stoppingToken);

                                    _logger.LogInformation("Despliegue programado completado con éxito.");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error ejecutando DeploymentSchedulerService.");
                }

                try
                {
                    // Espera 30 segundos antes del siguiente chequeo
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Ignorar la excepción cuando se detiene la aplicación
                    break;
                }
            }
        }
    }
}
