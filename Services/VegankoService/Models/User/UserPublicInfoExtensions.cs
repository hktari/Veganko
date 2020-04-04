using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Users;

namespace VegankoService.Models.User
{
    public static class UserPublicInfoExtensions
    {
        public static void MapToCustomer(this UserPublicInfo dto, Customer customer)
        {
            customer.Id = dto.Id;
            customer.ProfileBackgroundId = dto.ProfileBackgroundId;
            customer.AvatarId = dto.AvatarId;
            customer.Description = dto.Description;
            customer.Label = dto.Label;
        }
    }
}
