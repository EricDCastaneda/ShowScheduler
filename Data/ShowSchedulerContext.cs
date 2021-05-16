using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ShowScheduler.Data
{
    public class ShowSchedulerContext : IdentityDbContext
    {
        public ShowSchedulerContext(DbContextOptions<ShowSchedulerContext> options)
            : base(options)
        {
        }

        public DbSet<ShowScheduler.Models.Show> Show { get; set; }
        public DbSet<ShowScheduler.Models.Band> Band { get; set; }

    }
}
