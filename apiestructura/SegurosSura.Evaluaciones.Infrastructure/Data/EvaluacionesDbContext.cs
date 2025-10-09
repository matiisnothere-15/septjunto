using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Infrastructure.Data;

public class EvaluacionesDbContext : DbContext
{
    public EvaluacionesDbContext(DbContextOptions<EvaluacionesDbContext> options) : base(options)
    {
    }

    public DbSet<Proyecto> Proyectos { get; set; }
    public DbSet<Componente> Componentes { get; set; }
    public DbSet<Complejidad> Complejidades { get; set; }
    public DbSet<RelacionComponenteComplejidad> RelacionesComponenteComplejidad { get; set; }
    public DbSet<Evaluacion> Evaluaciones { get; set; }
    public DbSet<EvaluacionDetalle> EvaluacionDetalles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Proyecto
        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).HasMaxLength(1000);
            entity.Property(e => e.Fecha).IsRequired();
            entity.Property(e => e.HorasTotales).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.DiasEstimados).IsRequired();
            entity.Property(e => e.Riesgo).IsRequired().HasColumnType("decimal(18,2)");
        });

        // Configuración de Componente
        modelBuilder.Entity<Componente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Activo).IsRequired();
            
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // Configuración de Complejidad
        modelBuilder.Entity<Complejidad>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Orden).IsRequired();
            entity.Property(e => e.Activo).IsRequired();
            
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.HasIndex(e => e.Orden).IsUnique();
        });

        // Configuración de RelacionComponenteComplejidad
        modelBuilder.Entity<RelacionComponenteComplejidad>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Horas).IsRequired().HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Componente)
                  .WithMany(c => c.RelacionesComplejidad)
                  .HasForeignKey(e => e.ComponenteId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Complejidad)
                  .WithMany(c => c.RelacionesComponente)
                  .HasForeignKey(e => e.ComplejidadId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.ComponenteId, e.ComplejidadId }).IsUnique();
        });

        // Configuración de Evaluacion
        modelBuilder.Entity<Evaluacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Fecha).IsRequired();
            entity.Property(e => e.NombreProyecto).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DeltaRiesgoPct).HasColumnType("decimal(18,2)");
            entity.Property(e => e.HorasTotales).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.HorasTotalesConRiesgo).HasColumnType("decimal(18,2)");
        });

        // Configuración de EvaluacionDetalle
        modelBuilder.Entity<EvaluacionDetalle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HorasBase).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.DescripcionTarea).IsRequired().HasMaxLength(500);
            
            entity.HasOne(e => e.Evaluacion)
                  .WithMany(e => e.Detalles)
                  .HasForeignKey(e => e.EvaluacionId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Componente)
                  .WithMany(c => c.EvaluacionDetalles)
                  .HasForeignKey(e => e.ComponenteId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Complejidad)
                  .WithMany(c => c.EvaluacionDetalles)
                  .HasForeignKey(e => e.ComplejidadId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data para componentes básicos
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Componentes básicos
        var componentesBasicos = new[]
        {
            new Componente { Id = Guid.NewGuid(), Nombre = "Function", Descripcion = "Azure Function", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "Data Factory", Descripcion = "Pipeline ADF", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "API REST", Descripcion = "Web API", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "Base de Datos", Descripcion = "SQL/NoSQL", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "Frontend", Descripcion = "Angular/React", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "Logic App", Descripcion = "Azure Logic App", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "Power BI", Descripcion = "Reportes BI", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "Data Lake", Descripcion = "Azure Data Lake", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "Event Hub", Descripcion = "Azure Event Hub", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "Service Bus", Descripcion = "Azure Service Bus", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "Key Vault", Descripcion = "Azure Key Vault", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "App Service", Descripcion = "Azure App Service", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "Container", Descripcion = "Docker/Kubernetes", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "DevOps", Descripcion = "CI/CD Pipeline", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "Testing", Descripcion = "Unit/Integration Tests", Activo = true },
            new Componente { Id = Guid.NewGuid(), Nombre = "Documentación", Descripcion = "Técnica y Usuario", Activo = true }
        };

        modelBuilder.Entity<Componente>().HasData(componentesBasicos);

        // Complejidades básicas
        var complejidadesBasicas = new[]
        {
            new Complejidad { Id = Guid.NewGuid(), Nombre = "Muy Baja", Orden = 1, Activo = true },
            new Complejidad { Id = Guid.NewGuid(), Nombre = "Baja", Orden = 2, Activo = true },
            new Complejidad { Id = Guid.NewGuid(), Nombre = "Media", Orden = 3, Activo = true },
            new Complejidad { Id = Guid.NewGuid(), Nombre = "Alta", Orden = 4, Activo = true },
            new Complejidad { Id = Guid.NewGuid(), Nombre = "Muy Alta", Orden = 5, Activo = true }
        };

        modelBuilder.Entity<Complejidad>().HasData(complejidadesBasicas);
    }
}
