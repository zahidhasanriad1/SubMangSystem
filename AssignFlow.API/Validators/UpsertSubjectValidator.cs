using AssignFlow.Models.Academic;
using FluentValidation;

namespace AssignFlow.API.Validators;

public class UpsertSubjectValidator : AbstractValidator<UpsertSubjectDto>
{
    public UpsertSubjectValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30).Matches("^[A-Za-z0-9-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
    }
}
