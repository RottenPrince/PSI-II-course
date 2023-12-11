using BrainBoxAPI.Models;

namespace BrainBoxAPI.Managers
{
    public interface IRoomRepository : IRepository<RoomModel>
    {
        Task<RoomModel?> GetByUniqueCode(string uniqueCode);
    }
}
