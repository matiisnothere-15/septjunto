using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SegurosSura.Evaluaciones.Infrastructure.Data;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Infrastructure.Repositories;
using MediatR;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using SegurosSura.Evaluaciones.Domain.Entities;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using SegurosSura.Evaluaciones.Api.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        // Evitar ciclos de referencia en serialización (navegaciones EF)
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
// Registrar validadores únicamente desde la capa Application
builder.Services.AddValidatorsFromAssembly(typeof(SegurosSura.Evaluaciones.Application.Componentes.Commands.Create.CreateComponenteCommand).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SegurosSura.Evaluaciones.Api",
        Version = "v1",
        Description = "API para gestión de proyectos, componentes, complejidades y evaluaciones"
    });

    // Evitar colisiones de schemaId cuando existen tipos con el mismo nombre en distintos namespaces
    options.CustomSchemaIds(type => (type.FullName ?? type.Name).Replace("+", "."));
});

// Database: Allow opting into SQL Server in Development via config/env flag; default InMemory in Dev
builder.Services.AddDbContext<EvaluacionesDbContext>(options =>
{
    var useSqlInDev = builder.Configuration.GetValue<bool>("UseSqlInDev")
                     || string.Equals(Environment.GetEnvironmentVariable("USE_SQL_IN_DEV"), "true", StringComparison.OrdinalIgnoreCase);

    if (builder.Environment.IsDevelopment() && !useSqlInDev)
    {
        options.UseInMemoryDatabase("SegurosSuraEvaluacionesDev");
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Repositories
builder.Services.AddScoped<IComponenteRepository, ComponenteRepository>();
builder.Services.AddScoped<IComplejidadRepository, ComplejidadRepository>();
builder.Services.AddScoped<IRelacionComponenteComplejidadRepository, RelacionComponenteComplejidadRepository>();
builder.Services.AddScoped<IEvaluacionRepository, EvaluacionRepository>();
builder.Services.AddScoped<IProyectoRepository, ProyectoRepository>();

// MediatR: solo escanear la capa Application
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SegurosSura.Evaluaciones.Application.Componentes.Commands.Create.CreateComponenteCommand).Assembly));

// Validation Behavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SegurosSura.Evaluaciones.Application.Behaviors.ValidationBehavior<,>));

// CORS
builder.Services.AddCors(options =>
{
    // En desarrollo permitimos todo
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    // En producción, orígenes explícitos desde configuración: "Cors:Origins": ["https://app.ejemplo.com"]
    options.AddPolicy("FrontendOrigins", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();
        if (origins.Length > 0)
        {
            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // Si no hay orígenes definidos, no permitir ninguno explícitamente.
            policy.WithOrigins(Array.Empty<string>());
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Exception handling middleware
app.UseMiddleware<SegurosSura.Evaluaciones.Api.Middleware.ExceptionHandlingMiddleware>();

// En desarrollo evitamos forzar HTTPS para simplificar el consumo desde el front (SSR) y evitar
// problemas de certificados. En producción mantenemos la redirección a HTTPS.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
// Selecciona política según ambiente
app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "FrontendOrigins");
app.UseAuthorization();
app.MapControllers();

// Endpoint de diagnóstico para listar rutas y métodos HTTP expuestos
app.MapGet("/__endpoints", (IEnumerable<EndpointDataSource> srcs) =>
    srcs.SelectMany(s => s.Endpoints)
        .OfType<RouteEndpoint>()
        .Select(e => new
        {
            Pattern = e.RoutePattern.RawText,
            Methods = e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods
        })
);

// Plan B: MapPost explícito en Desarrollo para validar rápidamente la ruta exacta
if (app.Environment.IsDevelopment())
{
    app.MapPost("/api/proyectos", async ([FromBody] ProyectosController.CreateProyectoRequest body, IMediator mediator, CancellationToken ct) =>
    {
        var created = await mediator.Send(new SegurosSura.Evaluaciones.Application.Proyectos.Commands.Create.CreateProyectoCommand(
            body.Nombre,
            body.Descripcion,
            body.DiasEstimados,
            body.Fecha,
            body.HorasTotales,
            body.Riesgo
        ), ct);
        return Results.Created($"/api/proyectos/{created.Id}", created);
    });
}

// Ensure database is created when using InMemory provider in Development (no manual seed)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<EvaluacionesDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
