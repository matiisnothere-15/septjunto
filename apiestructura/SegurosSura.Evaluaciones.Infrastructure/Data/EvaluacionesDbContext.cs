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

        // Establecer el esquema por defecto
        modelBuilder.HasDefaultSchema("Proyectos");

        // Configuración de Proyecto
        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.ToTable("TBL_Proyecto");
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
            entity.ToTable("TBL_Componente");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Activo).IsRequired();
            entity.Property(e => e.ProyectoId).IsRequired();
            
            entity.HasIndex(e => e.Nombre).IsUnique();

            entity.HasOne(e => e.Proyecto)
                  .WithMany(p => p.Componentes)
                  .HasForeignKey(e => e.ProyectoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Complejidad
        modelBuilder.Entity<Complejidad>(entity =>
        {
            entity.ToTable("TBL_Complejidad");
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
            entity.ToTable("TBL_RelacionCC");
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
            entity.ToTable("TBL_Evaluacion");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Fecha).IsRequired();
            // La BD existente no tiene la columna NombreProyecto; Ignorar para evitar errores en consultas.
            entity.Ignore(e => e.NombreProyecto);
            entity.Property(e => e.DeltaRiesgoPct).HasColumnType("decimal(18,2)");
            // La BD existente no tiene HorasTotales; Ignorar para evitar errores en consultas.
            entity.Ignore(e => e.HorasTotales);
            entity.Property(e => e.HorasTotalesConRiesgo).HasColumnType("decimal(18,2)");

            // FK a Proyecto (existe en BD como NOT NULL)
            entity.Property(e => e.ProyectoId).IsRequired();
            entity.HasOne(e => e.Proyecto)
                  .WithMany(p => p.Evaluaciones)
                  .HasForeignKey(e => e.ProyectoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de EvaluacionDetalle
        modelBuilder.Entity<EvaluacionDetalle>(entity =>
        {
            entity.ToTable("TBL_EvaluacionDetalle");
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

    // Seed data básicos (solo catálogos imprescindibles)
    SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Complejidades básicas con GUIDs constantes (evitar Guid.NewGuid en HasData)
        modelBuilder.Entity<Complejidad>().HasData(
            new Complejidad { Id = new Guid("11111111-1111-1111-1111-111111111111"), Nombre = "Muy Baja", Orden = 1, Activo = true },
            new Complejidad { Id = new Guid("22222222-2222-2222-2222-222222222222"), Nombre = "Baja", Orden = 2, Activo = true },
            new Complejidad { Id = new Guid("33333333-3333-3333-3333-333333333333"), Nombre = "Media", Orden = 3, Activo = true },
            new Complejidad { Id = new Guid("44444444-4444-4444-4444-444444444444"), Nombre = "Alta", Orden = 4, Activo = true },
            new Complejidad { Id = new Guid("55555555-5555-5555-5555-555555555555"), Nombre = "Muy Alta", Orden = 5, Activo = true }
        );
    }
}
