using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Entities;
using AssignFlow.Models.Academic;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Exceptions;

namespace AssignFlow.Services.Services;

public class SubjectService : Service<Subject, Guid>, ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;

    public SubjectService(ISubjectRepository subjectRepository) : base(subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public Task<ICollection<SubjectDto>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        return _subjectRepository.GetSubjectsAsync(cancellationToken);
    }

    public async Task<SubjectDto> CreateSubjectAsync(UpsertSubjectDto model, CancellationToken cancellationToken = default)
    {
        var code = model.Code.Trim().ToUpperInvariant();
        if (await _subjectRepository.CodeExistsAsync(code, null, cancellationToken))
            throw new ConflictException("Subject code already exists.");

        var entity = new Subject { Code = code, Name = model.Name.Trim(), IsActive = model.IsActive };
        _ = await _subjectRepository.AddAsync(entity, cancellationToken)
            ? true
            : throw new BadRequestException("Failed to create the subject.");
        return Map(entity);
    }

    public async Task<SubjectDto> UpdateSubjectAsync(
        Guid subjectId,
        UpsertSubjectDto model,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(subjectId, cancellationToken);
        var code = model.Code.Trim().ToUpperInvariant();
        if (await _subjectRepository.CodeExistsAsync(code, subjectId, cancellationToken))
            throw new ConflictException("Subject code already exists.");

        entity.Code = code;
        entity.Name = model.Name.Trim();
        entity.IsActive = model.IsActive;
        _ = await _subjectRepository.UpdateAsync(entity, cancellationToken)
            ? true
            : throw new BadRequestException("Failed to update the subject.");
        return Map(entity);
    }

    public async Task<bool> DeleteSubjectAsync(Guid subjectId, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(subjectId, cancellationToken);
        if (await _subjectRepository.HasCourseOfferingsAsync(subjectId, cancellationToken))
            throw new ConflictException("Remove the subject's course offerings before deleting it.");

        return await _subjectRepository.DeleteAsync(entity, cancellationToken)
            ? true
            : throw new BadRequestException("Failed to delete the subject.");
    }

    private static SubjectDto Map(Subject entity)
    {
        return new SubjectDto
        {
            SubjectId = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            IsActive = entity.IsActive
        };
    }
}
