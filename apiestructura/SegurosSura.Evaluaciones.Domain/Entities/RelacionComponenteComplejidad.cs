using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SegurosSura.Evaluaciones.Domain.Entities;

public class RelacionComponenteComplejidad
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [ForeignKey(nameof(Componente))]
    public Guid ComponenteId { get; set; }
    
    [Required]
    [ForeignKey(nameof(Complejidad))]
    public Guid ComplejidadId { get; set; }
    
    [Required]
    public decimal Horas { get; set; }
    
    // Relaciones
    public virtual Componente Componente { get; set; } = null!;
    public virtual Complejidad Complejidad { get; set; } = null!;
}
