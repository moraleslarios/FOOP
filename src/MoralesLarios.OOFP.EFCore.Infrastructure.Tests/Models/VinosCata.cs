using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Models;

[Table("VINOSCATAS")]
public class VinosCata
{
    [Required]
    public int IdVino { get; set; }
    
    [Required]
    public int IdCata { get; set; }

    // Navigation properties
    public virtual Vino Vino { get; set; } = null!;
    public virtual Cata Cata { get; set; } = null!;
}