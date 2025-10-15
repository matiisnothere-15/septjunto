using System.ComponentModel.DataAnnotations;

namespace SegurosSura.Evaluaciones.Domain.Entities;

public class Componente
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;
    
    [Required]
    public bool Activo { get; set; } = true;
    
    // FK requerido según la BD: cada componente pertenece a un proyecto
    [Required]
    public Guid ProyectoId { get; set; }
    public virtual Proyecto? Proyecto { get; set; }
    
    // Relaciones
    public virtual ICollection<RelacionComponenteComplejidad> RelacionesComplejidad { get; set; } = new List<RelacionComponenteComplejidad>();
    public virtual ICollection<EvaluacionDetalle> EvaluacionDetalles { get; set; } = new List<EvaluacionDetalle>();
}
