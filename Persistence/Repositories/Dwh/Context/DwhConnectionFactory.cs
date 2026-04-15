using Microsoft.Data.SqlClient;

namespace Persistence.Repositories.Dwh.Context
{
    public class DwhConnectionFactory
    {
        public string ConnectionString { get; }

        public DwhConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public SqlConnection CreateMasterConnection()
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString)
            {
                InitialCatalog = "master"
            };

            return new SqlConnection(builder.ConnectionString);
        }

        public IEnumerable<string> GetConnectionStringCandidates(bool useMasterDatabase = false)
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString);

            if (useMasterDatabase)
            {
                builder.InitialCatalog = "master";
            }

            builder.TrustServerCertificate = true;
            builder.Encrypt = false;

            var candidates = new List<string>();
            var originalServer = builder.DataSource;

            foreach (var server in GetServerCandidates(originalServer))
            {
                var candidateBuilder = new SqlConnectionStringBuilder(builder.ConnectionString)
                {
                    DataSource = server,
                    TrustServerCertificate = true,
                    Encrypt = false
                };

                candidates.Add(candidateBuilder.ConnectionString);
            }

            return candidates.Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static IEnumerable<string> GetServerCandidates(string originalServer)
        {
            var machineName = Environment.MachineName;
            var instanceName = GetInstanceName(originalServer);

            yield return string.IsNullOrWhiteSpace(originalServer) ? "." : originalServer;
            if (!string.IsNullOrWhiteSpace(instanceName))
            {
                yield return $@".\{instanceName}";
                yield return $@"{machineName}\{instanceName}";
            }
            yield return ".";
            yield return "(local)";
            yield return machineName;
            yield return $@".\SQLEXPRESS";
            yield return $@"{machineName}\SQLEXPRESS";
            yield return @"(localdb)\MSSQLLocalDB";
            yield return "127.0.0.1";
        }

        private static string? GetInstanceName(string originalServer)
        {
            if (string.IsNullOrWhiteSpace(originalServer))
            {
                return null;
            }

            var parts = originalServer.Split('\\', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return parts.Length > 1 ? parts[^1] : null;
        }
    }
}
