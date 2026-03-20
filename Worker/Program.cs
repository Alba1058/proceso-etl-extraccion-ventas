using Application.Interfaces;
using Application.Services;
using Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddHttpClient("SalesApi", client =>
            {
                var baseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl");
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    client.BaseAddress = new Uri(baseUrl);
                }

                var apiKey = builder.Configuration.GetValue<string>("ApiSettings:ApiKey");
                if (!string.IsNullOrEmpty(apiKey))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                }
            });

            builder.Services.AddTransient<IETLService, ETLService>();
            builder.Services.AddTransient<ITransformationService, TransformationService>();

            builder.Services.AddInfrastructure();

            builder.Services.AddHostedService<EtlWorker>();

            var host = builder.Build();
            host.Run();
        }
    }
}