using AssignFlow.Models.Submissions;
using FluentValidation;

namespace AssignFlow.API.Validators;

public class UpsertSubmissionValidator : AbstractValidator<UpsertSubmissionDto>
{
    public UpsertSubmissionValidator()
    {
        RuleFor(x => x.Answer).NotEmpty().MaximumLength(50000);
    }
}
