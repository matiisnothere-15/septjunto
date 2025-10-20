using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SegurosSura.Evaluaciones.Domain.Entities;

public class Evaluacion
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    // Campo solo para mostrar; el DbContext lo ignora porque no existe físicamente en la BD actual
    [MaxLength(200)]
    public string NombreProyecto { get; set; } = string.Empty;

    public decimal? DeltaRiesgoPct { get; set; }

    // Campo calculado/solo lectura en UI; el DbContext lo ignora si la columna no existe
    public decimal HorasTotales { get; set; } // Horas base totales

    public decimal? HorasTotalesConRiesgo { get; set; }

    // FK requerida por la BD actual (TBL_Evaluacion.ProyectoId)
    [Required]
    public Guid ProyectoId { get; set; }

    public virtual Proyecto? Proyecto { get; set; }

    // Relaciones
    public virtual ICollection<EvaluacionDetalle> Detalles { get; set; } = new List<EvaluacionDetalle>();
}