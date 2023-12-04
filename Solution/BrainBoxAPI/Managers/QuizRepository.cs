using BrainBoxAPI.Data;
using BrainBoxAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BrainBoxAPI.Managers
{
    public class QuizRepository : Repository<QuizModel>
    {
        public override IQueryable<QuizModel> Query => _context.QuizModels
                        .Include(q => q.Room)
                        .Include(q => q.User);
        public QuizRepository(AppDbContext context) : base(context) { }
        public override Task<QuizModel?> GetById(int id)
        {
            return Query
                    .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
