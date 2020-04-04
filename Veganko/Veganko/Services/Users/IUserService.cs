using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Common.Models.Users;
using Veganko.Models;
using Veganko.Models.User;

namespace Veganko.Services
{
    public interface IUserService
    {
        void ClearCurrentUser();
        void SetCurrentUser(UserPublicInfo user);
        Task EnsureCurrentUserIsSet();
        UserPublicInfo CurrentUser { get; }
        Task<UserPublicInfo> Get(string id);
        Task<IEnumerable<UserPublicInfo>> GetByIds(IEnumerable<string> id);
        Task<UserPublicInfo> Edit(UserPublicInfo user);
        Task SetRole(UserPublicInfo user, UserRole role);
        Task<PagedList<UserPublicInfo>> GetAll(UserQuery query);
    }
}
