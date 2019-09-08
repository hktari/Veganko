﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models.User;

namespace VegankoService.Data.Users
{
    public class UsersRepository : IUsersRepository
    {
        private readonly VegankoContext context;

        public UsersRepository(VegankoContext context)
        {
            this.context = context;
        }

        public CustomerProfile Get(string identityId)
        {
             IQueryable<CustomerProfile> query = 
                from customer in context.Customer
                where customer.IdentityId == identityId
                join appUser in context.Users on customer.IdentityId equals appUser.Id
                join userRole in context.UserRoles on customer.IdentityId equals userRole.UserId
                join role in context.Roles on userRole.RoleId equals role.Id
                select new CustomerProfile
                {
                    Id = customer.Id,
                    Username = appUser.UserName,
                    Email = appUser.Email,
                    AvatarId = customer.AvatarId,
                    Description = customer.Description,
                    Label = customer.Label,
                    ProfileBackgroundId = customer.ProfileBackgroundId,
                    Role = role.Name
                };

            return query.First();
        }
    }
}
