using EnvCreatorApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EnvCreatorApi.Data
{
    public class EnvCreatorContext : IdentityDbContext<User>
    {
        public EnvCreatorContext(DbContextOptions<EnvCreatorContext> options)
            : base(options)
        {
        }

        public DbSet<Environment2D> Environments { get; set; }
        public DbSet<Object2D> Objects { get; set; }
    }
}