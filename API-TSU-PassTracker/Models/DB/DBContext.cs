using Microsoft.EntityFrameworkCore;
using API_TSU_PassTracker.Models.DB;

namespace API_TSU_PassTracker.Models.DB
{
    public class TsuPassTrackerDBContext : DbContext
    {
        public DbSet<User> User { get; set; }

        public DbSet<Request> Request { get; set; }

        public DbSet<Confirmation> Confirmation { get; set; }

        public TsuPassTrackerDBContext(DbContextOptions<TsuPassTrackerDBContext> options) : base(options)
        {

        }

    }
}