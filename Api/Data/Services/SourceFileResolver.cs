using Api.Common.Exceptions;
using Api.Data.Interfaces;
using Infrastructure.IO;

namespace Api.Data.Services
{
    public class SourceFileResolver : ISourceFileResolver
    {
        private readonly IConfiguration _configuration;

        public SourceFileResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ResolveRequiredPath(string key, string fallbackPath)
        {
            var configuredPath = _configuration[$"SourceFiles:{key}"] ?? fallbackPath;
            var resolvedPath = PathResolver.ResolveExistingPath(configuredPath);

            if (!File.Exists(resolvedPath))
            {
                throw new ApiException(
                    $"No se encontro la fuente de datos configurada para {key}. Ruta esperada: {resolvedPath}",
                    StatusCodes.Status404NotFound);
            }

            return resolvedPath;
        }
    }
}
