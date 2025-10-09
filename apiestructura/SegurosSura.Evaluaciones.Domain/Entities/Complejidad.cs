using System.ComponentModel.DataAnnotations;

namespace SegurosSura.Evaluaciones.Domain.Entities;

public class Complejidad
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;
    
    [Required]
    public int Orden { get; set; }
    
    [Required]
    public bool Activo { get; set; } = true;
    
    // Relaciones
    public virtual ICollection<RelacionComponenteComplejidad> RelacionesComponente { get; set; } = new List<RelacionComponenteComplejidad>();
    public virtual ICollection<EvaluacionDetalle> EvaluacionDetalles { get; set; } = new List<EvaluacionDetalle>();
}
