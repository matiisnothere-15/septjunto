using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SegurosSura.Evaluaciones.Domain.Entities;

public class Evaluacion
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime Fecha { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string NombreProyecto { get; set; } = string.Empty;
    
    public decimal? DeltaRiesgoPct { get; set; }
    
    [Required]
    public decimal HorasTotales { get; set; }
    
    public decimal? HorasTotalesConRiesgo { get; set; }
    
    // Relaciones
    public virtual ICollection<EvaluacionDetalle> Detalles { get; set; } = new List<EvaluacionDetalle>();
}
