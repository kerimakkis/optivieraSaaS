using Microsoft.AspNetCore.Identity;
using Optiviera.Models;

namespace Optiviera.Services.Interfaces
{
    public interface ITRolesService
    {
        public Task<IEnumerable<string>> GetUserRolesAsync(WaveUser user);
        public Task<bool> AddUserToRoleAsync(WaveUser user, string roleName);
        public Task<bool> RemoveUserFromRolesAsync(WaveUser user, IEnumerable<string> roles);
        public Task<List<IdentityRole>> GetRolesAync();
    }
}
