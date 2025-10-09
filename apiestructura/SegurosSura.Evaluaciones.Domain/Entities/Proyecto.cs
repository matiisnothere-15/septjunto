using System.ComponentModel.DataAnnotations;

namespace SegurosSura.Evaluaciones.Domain.Entities;

public class Proyecto
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Descripcion { get; set; } = string.Empty;
    
    [Required]
    public DateTime Fecha { get; set; }
    
    [Required]
    public decimal HorasTotales { get; set; }
    
    [Required]
    public int DiasEstimados { get; set; }
    
    [Required]
    public decimal Riesgo { get; set; }
    
    // Relaciones
    public virtual ICollection<Componente> Componentes { get; set; } = new List<Componente>();
    public virtual ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
}
