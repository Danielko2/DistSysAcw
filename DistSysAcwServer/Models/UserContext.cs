using Microsoft.EntityFrameworkCore;

namespace DistSysAcwServer.Models
{
    public class UserContext : DbContext
    {
        public UserContext() : base() { }

        public DbSet<User> Users { get; set; }

        //TODO: Task13

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DistSysAcw;");
        }
        public async Task<User> CreateUserAsync(string username)
        {
            var newUser = new User
            {
                UserName = username,
                ApiKey = Guid.NewGuid().ToString()
            };
            this.Users.Add(newUser);
            await SaveChangesAsync();

            return newUser; // This includes the new ApiKey which is a GUID
        }

        public async Task<bool> CheckUserExistsAsync(string apiKey)
        {
            return await this.Users.AnyAsync(u => u.ApiKey == apiKey);

        }
        public async Task<bool> CheckUserExistsAsync(string apiKey, string username)
        {
            return await this.Users.AnyAsync(u => u.ApiKey == apiKey && u.UserName == username);
        }
        public async Task<User> GetUserAsync(string apiKey)
        {
            return await this.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
        }
        public async Task<bool> DeleteUserAsync(string apiKey)
        {
            var user = await this.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
            if (user != null)
            {
                this.Users.Remove(user);
                await SaveChangesAsync();

                return true;
            }
            return false;
        }


    }
}