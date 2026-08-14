using AssignFlow.Models.Academic;
using FluentValidation;

namespace AssignFlow.API.Validators;

public class UpsertClassRoomValidator : AbstractValidator<UpsertClassRoomDto>
{
    public UpsertClassRoomValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Section).NotEmpty().MaximumLength(30);
        RuleFor(x => x.AcademicYear).InclusiveBetween(2000, 2200);
    }
}
