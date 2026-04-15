using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories.Dwh;
using Persistence.Repositories.Dwh.Context;
using SistemaAnalisisVentas.Persistence.Repositories.Dwh.Context;

namespace Persistence.DependencyInjection
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string dataWarehouseConnectionString)
        {
            if (string.IsNullOrWhiteSpace(dataWarehouseConnectionString))
            {
                throw new InvalidOperationException("No se configuro la conexion DataWarehouseDb.");
            }

            services.AddDbContext<VentasAnaliticaContext>(options =>
                options.UseSqlServer(dataWarehouseConnectionString));

            services.AddSingleton<DataWarehouseSchemaProvider>();
            services.AddSingleton<DwhConnectionFactory>(_ =>
                new DwhConnectionFactory(dataWarehouseConnectionString));
            services.AddTransient<IDwhRepository, DwhRepository>();

            return services;
        }
    }
}
