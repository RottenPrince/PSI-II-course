using Microsoft.EntityFrameworkCore;

namespace BrainBoxAPI.Exceptions
{
    public class DbConstraintFailedException : Exception
    {
        public DbUpdateException InnerException { get; }
        public DbConstraintFailedException(DbUpdateException ex)
        {
            InnerException = ex;
        }
    }
}
