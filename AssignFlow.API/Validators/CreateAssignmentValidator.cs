using AssignFlow.Models.Assignments;
using FluentValidation;

namespace AssignFlow.API.Validators;

public class CreateAssignmentValidator : AbstractValidator<CreateAssignmentDto>
{
    public CreateAssignmentValidator()
    {
        RuleFor(x => x.CourseOfferingId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(10000);
        RuleFor(x => x.DeadlineUtc).NotEmpty();
        RuleFor(x => x.MaximumMarks).GreaterThan(0).LessThanOrEqualTo(10000);
    }
}
