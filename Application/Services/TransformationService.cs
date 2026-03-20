using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class TransformationService : ITransformationService
    {
        private readonly ILogger<TransformationService> logger;

        public TransformationService(ILogger<TransformationService> logger)
        {
            this.logger = logger;
        }

        public async Task TransformAsync()
        {
            logger.LogInformation("Iniciando transformación de datos...");

            try
            {
                await Task.Delay(500); // Simulación temporal

                logger.LogInformation("Transformación completada correctamente.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error durante la transformación.");
                throw;
            }
        }
    }
}