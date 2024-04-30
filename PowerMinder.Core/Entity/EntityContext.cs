using Microsoft.EntityFrameworkCore;

namespace PowerMinder.Core.Entity
{
    public class EntityContext : DbContext
    {
        public EntityContext(DbContextOptions<EntityContext> options) : base(options)
        {

        }

        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<TestTimeZone> TestTimeZone { get; set; }
    }
}
