using BrainBoxAPI.Models;
using BrainBoxAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BrainBoxAPI.Managers
{
    public class QuestionRepository : Repository<QuestionModel>
    {
        public override IQueryable<QuestionModel> Query => _context.Questions.Include(q => q.AnswerOptions);

        public QuestionRepository(AppDbContext context) : base(context) { }

        public override Task<QuestionModel?> GetById(int id)
        {
            return Query.FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
