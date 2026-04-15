using Api.Common.Extensions;
using Api.Data.Interfaces;
using Api.Data.Persistence;
using Api.Data.Repositories;
using Api.Data.Services;
using Infrastructure.Extractors.CSV;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddSingleton<CsvReaderService>();
builder.Services.AddSingleton<ISourceFileResolver, SourceFileResolver>();
builder.Services.AddScoped<ICustomerSourceRepository, CustomerSourceRepository>();
builder.Services.AddScoped<IProductSourceRepository, ProductSourceRepository>();
builder.Services.AddScoped<IVentasSourceRepository, VentasSourceRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

var app = builder.Build();

app.UseApiExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "SistemaAnalisisVentas.Api" }));

app.Run();
