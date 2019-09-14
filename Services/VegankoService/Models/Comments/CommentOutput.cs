using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models.User;

namespace VegankoService.Models.Comments
{
    public class CommentOutput : Comment
    {
        public string Username { get; set; }
        public string UserEmail { get; set; }
        public string UserProfileBackgroundId { get; set; }
        public string UserAvatarId { get; set; }
        public string UserRole { get; set; }
    }
}
