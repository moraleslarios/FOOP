using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Models;

[Table("CATAS")]
public class Cata
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public DateTime Fecha { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Lugar { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string Comentarios { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<VinosCata> VinosCatas { get; set; } = new List<VinosCata>();
    public virtual ICollection<VinosCatasPuntuacion> VinosCatasPuntuaciones { get; set; } = new List<VinosCatasPuntuacion>();
}