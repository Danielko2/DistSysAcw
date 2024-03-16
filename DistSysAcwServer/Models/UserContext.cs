using Microsoft.EntityFrameworkCore;

namespace DistSysAcwServer.Models
{
    public class UserContext : DbContext
    {
        public UserContext() : base() { }

        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<LogArchive> LogArchives { get; set; }
        //TODO: Task13

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DistSysAcw;");
        }
       


    }
}