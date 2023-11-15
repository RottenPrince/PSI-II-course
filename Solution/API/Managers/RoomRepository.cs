using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Managers
{
    public class RoomRepository : Repository<RoomModel>
    {
        public override IQueryable<RoomModel> Query => _context.Rooms
                        .Include(q => q.Questions)
                        .Include(q => q.SolveRuns);
        public RoomRepository(AppDbContext context) : base(context) { }
        public override Task<RoomModel?> GetById(int id)
        {
            return Query
                    .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
