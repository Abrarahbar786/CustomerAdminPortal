using CustomerAdminPortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CustomerAdminPortal.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(
            DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }
    }
}