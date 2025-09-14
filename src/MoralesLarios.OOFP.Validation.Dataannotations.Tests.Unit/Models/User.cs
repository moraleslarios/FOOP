using System.ComponentModel.DataAnnotations;

namespace MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit.Models;
public record User
{
    [Required]
    [StringLength(5)]
    public string? UserName { get; init; }
    public DateTime EntryDate { get; init; }
    [Required]
    [StringLength(8)]
    public string? Password { get; init; }
    [Required]
    [StringLength(8)]
    public string? PasswordConfirmation { get; init; }

    public User(string? userName, DateTime entryDate, string? password, string? passwordConfirmation)
    {
        UserName = userName;
        EntryDate = entryDate;
        Password = password;
        PasswordConfirmation = passwordConfirmation;
    }
}
