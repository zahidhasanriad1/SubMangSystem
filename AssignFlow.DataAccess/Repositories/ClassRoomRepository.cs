using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Database;
using AssignFlow.Domain.Entities;
using AssignFlow.Models.Academic;
using Microsoft.EntityFrameworkCore;

namespace AssignFlow.DataAccess.Repositories;

public class ClassRoomRepository : Repository<ClassRoom, Guid>, IClassRoomRepository
{
    public ClassRoomRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<ICollection<ClassRoomDto>> GetClassRoomsAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.ClassRooms.AsNoTracking()
            .OrderByDescending(x => x.AcademicYear)
            .ThenBy(x => x.Name)
            .Select(x => new ClassRoomDto
            {
                ClassRoomId = x.Id,
                Name = x.Name,
                Section = x.Section,
                AcademicYear = x.AcademicYear,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(
        string name,
        string section,
        int academicYear,
        Guid? excludedId,
        CancellationToken cancellationToken = default)
    {
        return DbContext.ClassRooms.AnyAsync(x =>
            x.Name == name &&
            x.Section == section &&
            x.AcademicYear == academicYear &&
            x.Id != excludedId,
            cancellationToken);
    }
}
