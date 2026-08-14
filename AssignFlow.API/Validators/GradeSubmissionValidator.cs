using AssignFlow.Models.Submissions;
using FluentValidation;

namespace AssignFlow.API.Validators;

public class GradeSubmissionValidator : AbstractValidator<GradeSubmissionDto>
{
    public GradeSubmissionValidator()
    {
        RuleFor(x => x.Marks).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Feedback).NotEmpty().MaximumLength(5000);
    }
}
