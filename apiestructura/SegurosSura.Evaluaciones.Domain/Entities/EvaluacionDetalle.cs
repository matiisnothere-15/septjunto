using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SegurosSura.Evaluaciones.Domain.Entities;

public class EvaluacionDetalle
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [ForeignKey(nameof(Evaluacion))]
    public Guid EvaluacionId { get; set; }
    
    [Required]
    [ForeignKey(nameof(Componente))]
    public Guid ComponenteId { get; set; }
    
    [Required]
    [ForeignKey(nameof(Complejidad))]
    public Guid ComplejidadId { get; set; }
    
    [Required]
    public decimal HorasBase { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string DescripcionTarea { get; set; } = string.Empty;
    
    // Relaciones
    public virtual Evaluacion Evaluacion { get; set; } = null!;
    public virtual Componente Componente { get; set; } = null!;
    public virtual Complejidad Complejidad { get; set; } = null!;
}
