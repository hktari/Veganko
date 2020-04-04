using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Common.Models.Users;
using Veganko.Models;
using Veganko.Models.User;

namespace Veganko.Services
{
    public class MockUserService : IUserService
    {
        private UserPublicInfo[] users = new UserPublicInfo[]
        {
            new UserPublicInfo
            {
                Id = "0",
                Username = "Boštjan",
                AvatarId = "0",
                Description = "Heyho poop on shoes",
                ProfileBackgroundId = "1",
                Label = "bun manager"
            },
            new UserPublicInfo
            {
                Id = "1",
                Username = "Zala",
                AvatarId = "2",
                Description = "inside me - a monster - a cookie monster !",
                ProfileBackgroundId = "3",
                Label = "bun manager #2"
            },
            new UserPublicInfo
            {
                Id = "2",
                Username = "Magda",
                AvatarId = "1",
                Description = "ni sreče ! življenje je strm klanc povn mehkih kurcov !!?!?!",
                ProfileBackgroundId = "2",
                Label = "..."
            },
        };

        public UserPublicInfo CurrentUser { get; private set; } = new UserPublicInfo
        {
            Username = "test", Email = "test@email.com", ProfileBackgroundId = "0", AvatarId = "0", Role = UserRole.Admin
        };

        public void ClearCurrentUser()
        {
            CurrentUser = null;
        }

        public Task<UserPublicInfo> Edit(UserPublicInfo user)
        {
            CurrentUser = user;
            return Task.FromResult(CurrentUser);
        }

        public Task EnsureCurrentUserIsSet()
        {
            return Task.CompletedTask;
        }

        public Task<UserPublicInfo> Get(string id)
        {
            return Task.FromResult(
                users.FirstOrDefault(user => user.Id == id)) ?? throw new Exception("User not found!");
        }

        public Task<IEnumerable<UserPublicInfo>> GetAll(UserQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserPublicInfo>> GetByIds(IEnumerable<string> userIds)
        {
            return Task.FromResult(
                users.Join(
                    userIds, user => user.Id,
                    userId => userId,
                    (user, userId) => user));
        }

        public void SetCurrentUser(UserPublicInfo user)
        {
            CurrentUser = user;
        }

        public Task SetRole(UserPublicInfo user, UserRole role)
        {
            throw new NotImplementedException();
        }

        Task<PagedList<UserPublicInfo>> IUserService.GetAll(UserQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
