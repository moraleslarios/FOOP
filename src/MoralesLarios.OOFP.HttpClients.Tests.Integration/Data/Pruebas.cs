namespace MoralesLarios.OOFP.HttpClients.Tests.Integration.Data;

public class Pruebas
{
    public long Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Comentarios { get; set; } = string.Empty;


}
