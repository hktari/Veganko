using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.User;

namespace Veganko.Services
{
    interface IUserService
    {
        Task<UserPublicInfo> Get(string id);
        Task<IEnumerable<UserPublicInfo>> GetByIds(IEnumerable<string> id);
        Task<IEnumerable<UserPublicInfo>> GetAll(int page = 1, int pageSize = 20);
        Task<UserPublicInfo> Edit(UserPublicInfo user);
    }
}
