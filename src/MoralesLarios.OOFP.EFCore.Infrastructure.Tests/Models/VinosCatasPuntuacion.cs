using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Models;

[Table("VINOSCATASPUNTUACION")]
public class VinosCatasPuntuacion
{
    [Required]
    public int IdVino { get; set; }
    
    [Required]
    public int IdCata { get; set; }
    
    [Required]
    [MaxLength(450)] // Standard length for Identity User Id
    public string IdUsuario { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(3,2)")]
    public decimal Puntuacion { get; set; }
    
    [Required]
    public DateTime Fecha { get; set; }

    // Navigation properties
    public virtual Vino Vino { get; set; } = null!;
    public virtual Cata Cata { get; set; } = null!;
    //public virtual IdentityUser Usuario { get; set; } = null!;
}