

namespace MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.Models;
public record User(string   UserName,
                   DateTime EntryDate,
                   string   Password,
                   string   PasswordConfirmation
);
