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
        CustomerProfile Get(string identityId);
        PagedList<CustomerProfile> GetAll(int page, int pageSize = 10);

    }
}
