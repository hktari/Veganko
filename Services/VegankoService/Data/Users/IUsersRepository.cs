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
        CustomerProfile GetProfile(string identityId);
        void Update(CustomerProfile customerProfile);
        Task Delete(string identityId);
        PagedList<CustomerProfile> GetAll(UserQuery query);
    }
}
