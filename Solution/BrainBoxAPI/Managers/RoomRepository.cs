using BrainBoxAPI.Data;
using BrainBoxAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BrainBoxAPI.Managers
{
    public class RoomRepository : Repository<RoomModel>, IRoomRepository
    {
        public override IQueryable<RoomModel> Query => _context.Rooms
                        .Include(q => q.Questions)
                        .Include(q => q.Quizs);
        public RoomRepository(AppDbContext context) : base(context) { }
        public override Task<RoomModel?> GetById(int id)
        {
            return Query
                    .FirstOrDefaultAsync(q => q.Id == id);
        }

        public Task<RoomModel?> GetByUniqueCode(string uniqueCode)
        {
            return Query
                .FirstOrDefaultAsync(q => q.UniqueCode == uniqueCode);
        }
    }
}
