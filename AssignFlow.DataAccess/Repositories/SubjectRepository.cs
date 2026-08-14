using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Database;
using AssignFlow.Domain.Entities;
using AssignFlow.Models.Academic;
using Microsoft.EntityFrameworkCore;

namespace AssignFlow.DataAccess.Repositories;

public class SubjectRepository : Repository<Subject, Guid>, ISubjectRepository
{
    public SubjectRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<ICollection<SubjectDto>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Subjects.AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new SubjectDto
            {
                SubjectId = x.Id,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public Task<bool> CodeExistsAsync(string code, Guid? excludedId, CancellationToken cancellationToken = default)
    {
        return DbContext.Subjects.AnyAsync(x => x.Code == code && x.Id != excludedId, cancellationToken);
    }
}
