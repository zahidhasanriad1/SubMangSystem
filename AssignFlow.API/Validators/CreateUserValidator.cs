using AssignFlow.Models.Users;
using FluentValidation;

namespace AssignFlow.API.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(200);
        RuleFor(x => x.Role).Must(x => new[] { "Admin", "Teacher", "Student" }.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Role must be Admin, Teacher, or Student.");
    }
}
