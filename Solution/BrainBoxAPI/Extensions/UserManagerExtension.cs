using BrainBoxAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BrainBoxAPI.Extensions
{
    public static class UserManagerExtension
    {
        public static Task<ApplicationUser?> FromClaim(this UserManager<ApplicationUser> self, ClaimsPrincipal User)
        {
            var userId = User.FindFirst("Id")?.Value;
            return self.Users
                .Include(u => u.Rooms)
                .Include(u => u.Quizzes)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
