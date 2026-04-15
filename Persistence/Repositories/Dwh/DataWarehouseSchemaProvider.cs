namespace Persistence.Repositories.Dwh
{
    public class DataWarehouseSchemaProvider
    {
        private readonly string _schemaScriptPath;

        public DataWarehouseSchemaProvider()
        {
            _schemaScriptPath = ResolveExistingPath(Path.Combine("Scripts", "Sql", "CreateSalesAnalyticsWarehouse.sql"));
        }

        public async Task<string> GetSchemaScriptAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_schemaScriptPath))
            {
                throw new FileNotFoundException("No se encontro el script SQL del Data Warehouse.", _schemaScriptPath);
            }

            return await File.ReadAllTextAsync(_schemaScriptPath, cancellationToken);
        }

        private static string ResolveExistingPath(string configuredPath)
        {
            if (Path.IsPathRooted(configuredPath))
            {
                return configuredPath;
            }

            var current = Directory.GetCurrentDirectory();
            var directory = new DirectoryInfo(current);

            while (directory is not null)
            {
                var candidate = Path.Combine(directory.FullName, configuredPath);
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            return Path.GetFullPath(Path.Combine(current, configuredPath));
        }
    }
}
