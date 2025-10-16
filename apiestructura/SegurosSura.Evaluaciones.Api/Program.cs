using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Infrastructure.Data;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Infrastructure.Repositories;
using MediatR;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using SegurosSura.Evaluaciones.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
// Registrar validadores únicamente desde la capa Application
builder.Services.AddValidatorsFromAssembly(typeof(SegurosSura.Evaluaciones.Application.Componentes.Commands.Create.CreateComponenteCommand).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();
// Selecciona política según ambiente
app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "FrontendOrigins");
app.UseAuthorization();
app.MapControllers();

// Ensure database is created when using InMemory provider in Development (no manual seed)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<EvaluacionesDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
