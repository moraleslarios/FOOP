using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Models;

[Table("VINOS")]
public class Vino
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Bodega { get; set; } = string.Empty;
    
    [Required]
    public int Año { get; set; }
    
    [MaxLength(200)]
    public string Patrocinador { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string DO { get; set; } = string.Empty;
    
    [Required]
    public DateTime FechaEntrada { get; set; }
    
    [MaxLength(2000)]
    public string Comentarios { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<VinosCata> VinosCatas { get; set; } = new List<VinosCata>();
    public virtual ICollection<VinosCatasPuntuacion> VinosCatasPuntuaciones { get; set; } = new List<VinosCatasPuntuacion>();
}