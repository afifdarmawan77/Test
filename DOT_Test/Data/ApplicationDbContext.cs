using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;


namespace DOT_Test.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<City> City { get; set; } = null!;
        public DbSet<Province> Province { get; set; } = null!;
        public DbSet<ApplicationUser> ApplicationUser { get; set; } = null!;
        public DbSet<ApplicationRole> ApplicationRole { get; set; }
    }
}
