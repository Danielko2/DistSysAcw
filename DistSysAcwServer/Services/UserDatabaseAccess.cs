using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DistSysAcwServer.Models
{
    public class UserDataAccess
    {
        private readonly UserContext _context;

        public UserDataAccess(UserContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(string username, string role)
        {
            var newUser = new User
            {
                UserName = username,
                ApiKey = Guid.NewGuid().ToString(),
                Role = role
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }


        public async Task<bool> CheckUserExistsAsync(string apiKey)
        {
            return await _context.Users.AnyAsync(u => u.ApiKey == apiKey);
        }
        public async Task<bool> CheckAnyUserExistsAsync()
        {
            return await _context.Users.AnyAsync();
        }


        public async Task<bool> CheckUserExistsAsync(string apiKey, string username)
        {
            return await _context.Users.AnyAsync(u => u.ApiKey == apiKey && u.UserName == username);
        }

        public async Task<bool> CheckUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username);
        }
        public async Task<bool> DeleteUserAsync(string apiKey)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<bool> ChangeUserRoleAsync(string apiKey, string newRole)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.ApiKey == apiKey);
            if (user != null)
            {
                user.Role = newRole;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }



    }
}
