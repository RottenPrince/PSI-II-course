using Microsoft.EntityFrameworkCore;

namespace API.Exceptions
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
