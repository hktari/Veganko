using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Users;
using VegankoService.Models;
using VegankoService.Models.User;

namespace VegankoService.Data.Users
{
    public interface IUsersRepository
    {
        Customer Get(string id);
        UserPublicInfo GetProfile(string id);
        void Update(UserPublicInfo customerProfile);
        Task Delete(string identityId);
        PagedList<UserPublicInfo> GetAll(UserQuery query);
        Customer GetByIdentityId(string identityId);
        UserPublicInfo GetProfileByIdentityId(string identityId);
    }
}
