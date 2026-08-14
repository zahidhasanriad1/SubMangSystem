using AssignFlow.Domain.Entities;
using AssignFlow.Models.Academic;

namespace AssignFlow.Services.Interfaces;

public interface IClassRoomService : IService<ClassRoom, Guid>
{
    Task<ICollection<ClassRoomDto>> GetClassRoomsAsync(CancellationToken cancellationToken = default);
    Task<ClassRoomDto> CreateClassRoomAsync(UpsertClassRoomDto model, CancellationToken cancellationToken = default);
    Task<ClassRoomDto> UpdateClassRoomAsync(Guid classRoomId, UpsertClassRoomDto model, CancellationToken cancellationToken = default);
    Task<bool> DeleteClassRoomAsync(Guid classRoomId, CancellationToken cancellationToken = default);
}
