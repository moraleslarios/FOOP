

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Models;

[Index("Name", Name = "IX_Countries_Name", IsUnique = true)]
public partial class Countries
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public string? FlagUri { get; set; }
}