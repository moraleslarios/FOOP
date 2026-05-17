namespace MoralesLarios.OOFP.HttpClients.Tests.Integration.Data;
public class SimplePruebaComplex
{
    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; }
    [Required]
    [MaxLength(200)]
    public string Lugar { get; set; }
    public long Precio  { get; set; }
    public DateTime Fecha { get; set; }
    public string Comentarios { get; set; }
}
