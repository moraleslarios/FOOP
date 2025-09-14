

namespace MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit.Validators;
public class UserValidator : AbstractValidator<User>
{


    public UserValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(x => x.UserName).MinimumLength(3).WithMessage("UserName must be at least 5 characters");
        RuleFor(x => x.Password).NotEmpty()
            .Length(8, 50).WithMessage("Password must be between 8 and 50 characters")
            .WithMessage("Password is required");
        RuleFor(x => x.PasswordConfirmation).NotEmpty()
            .Length(8, 50).WithMessage("Password must be between 8 and 50 characters")
            .WithMessage("Password is required");
    }



}
