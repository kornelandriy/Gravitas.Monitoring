using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gravitas.Monitoring.Models;

namespace Gravitas.Monitoring.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Gravitas.Monitoring.Models.Contact> Contact { get; set; } = default!;
    }
}
