

namespace MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit.Models;
public record User(string   UserName,
                   DateTime EntryDate,
                   string   Password,
                   string   PasswordConfirmation
);
