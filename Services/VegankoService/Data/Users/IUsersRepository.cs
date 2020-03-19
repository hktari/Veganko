using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models;
using VegankoService.Models.User;

namespace VegankoService.Data.Users
{
    public interface IUsersRepository
    {
        Customer Get(string id);
        CustomerProfile GetProfile(string identityId);
        PagedList<CustomerProfile> GetAll(int page, int pageSize = 10);
        void Update(CustomerProfile customerProfile);
        Task Delete(string identityId);
    }
}
