using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Entities;
using AssignFlow.Models.Academic;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Exceptions;

namespace AssignFlow.Services.Services;

public class ClassRoomService : Service<ClassRoom, Guid>, IClassRoomService
{
    private readonly IClassRoomRepository _classRoomRepository;

    public ClassRoomService(IClassRoomRepository classRoomRepository) : base(classRoomRepository)
    {
        _classRoomRepository = classRoomRepository;
    }

    public Task<ICollection<ClassRoomDto>> GetClassRoomsAsync(CancellationToken cancellationToken = default)
    {
        return _classRoomRepository.GetClassRoomsAsync(cancellationToken);
    }

    public async Task<ClassRoomDto> CreateClassRoomAsync(
        UpsertClassRoomDto model,
        CancellationToken cancellationToken = default)
    {
        var name = model.Name.Trim();
        var section = model.Section.Trim();
        if (await _classRoomRepository.ExistsAsync(name, section, model.AcademicYear, null, cancellationToken))
            throw new ConflictException("This class, section, and academic year already exist.");

        var entity = new ClassRoom
        {
            Name = name,
            Section = section,
            AcademicYear = model.AcademicYear,
            IsActive = model.IsActive
        };
        _ = await _classRoomRepository.AddAsync(entity, cancellationToken)
            ? true
            : throw new BadRequestException("Failed to create the class.");
        return Map(entity);
    }

    public async Task<ClassRoomDto> UpdateClassRoomAsync(
        Guid classRoomId,
        UpsertClassRoomDto model,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(classRoomId, cancellationToken);
        var name = model.Name.Trim();
        var section = model.Section.Trim();
        if (await _classRoomRepository.ExistsAsync(name, section, model.AcademicYear, classRoomId, cancellationToken))
            throw new ConflictException("This class, section, and academic year already exist.");

        entity.Name = name;
        entity.Section = section;
        entity.AcademicYear = model.AcademicYear;
        entity.IsActive = model.IsActive;
        _ = await _classRoomRepository.UpdateAsync(entity, cancellationToken)
            ? true
            : throw new BadRequestException("Failed to update the class.");
        return Map(entity);
    }

    private static ClassRoomDto Map(ClassRoom entity)
    {
        return new ClassRoomDto
        {
            ClassRoomId = entity.Id,
            Name = entity.Name,
            Section = entity.Section,
            AcademicYear = entity.AcademicYear,
            IsActive = entity.IsActive
        };
    }
}
