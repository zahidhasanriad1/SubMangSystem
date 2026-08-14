using AssignFlow.Domain.Entities;
using AssignFlow.Models.Academic;

namespace AssignFlow.DataAccess.Interfaces;

public interface IClassRoomRepository : IRepository<ClassRoom, Guid>
{
    Task<ICollection<ClassRoomDto>> GetClassRoomsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string name, string section, int academicYear, Guid? excludedId, CancellationToken cancellationToken = default);
}
