using System.ComponentModel.DataAnnotations;

namespace MoralesLarios.OOFP.HttpClients.Tests.Integration.Data;
public class PruebaComplexDto
{
    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Lugar { get; set; } = string.Empty;
    
    public long Precio { get; set; }
    
    public DateTime Fecha { get; set; }
    
    [MaxLength(2000)]
    public string Comentarios { get; set; } = string.Empty;
}
