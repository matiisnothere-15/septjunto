using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Infrastructure.Data;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Infrastructure.Repositories;
using MediatR;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssembly(typeof(SegurosSura.Evaluaciones.Application.Componentes.Commands.Create.CreateComponenteCommand).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<EvaluacionesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IComponenteRepository, ComponenteRepository>();
builder.Services.AddScoped<IComplejidadRepository, ComplejidadRepository>();
builder.Services.AddScoped<IRelacionComponenteComplejidadRepository, RelacionComponenteComplejidadRepository>();
builder.Services.AddScoped<IEvaluacionRepository, EvaluacionRepository>();
builder.Services.AddScoped<IProyectoRepository, ProyectoRepository>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SegurosSura.Evaluaciones.Application.Componentes.Commands.Create.CreateComponenteCommand).Assembly));

// Validation Behavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SegurosSura.Evaluaciones.Application.Behaviors.ValidationBehavior<,>));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
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
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<EvaluacionesDbContext>();
        context.Database.EnsureCreated();
    }
}

app.Run();
