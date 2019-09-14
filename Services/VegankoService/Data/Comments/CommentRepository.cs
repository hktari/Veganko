using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models;
using VegankoService.Models.Comments;

namespace VegankoService.Data.Comments
{
    public class CommentRepository : ICommentRepository
    {
        private readonly VegankoContext context;

        public CommentRepository(VegankoContext context)
        {
            this.context = context;
        }

        public void Create(Comment comment)
        {
            context.Comment.Add(comment);
            context.SaveChanges();
        }

        public void Delete(string id)
        {
            context.Comment.Remove(
                context.Comment.FirstOrDefault(c => c.Id == id) ?? throw new ArgumentException("Can't find comment with id:" + id));
            context.SaveChanges();
        }

        public Comment Get(string id)
        {
            return context.Comment.FirstOrDefault(c => c.Id == id);
        }

        public PagedList<CommentOutput> GetAll(string productId, int page, int pageSize = 10)
        {
            int pageIdx = page - 1;
            if (pageIdx < 0)
            {
                throw new ArgumentException("Page idx < 0 !");
            }

            IEnumerable<Comment> comments = 
                context.Comment
                .Where(c => c.ProductId == productId)
                .Skip(pageSize * pageIdx)
                .Take(pageSize);

            IEnumerable<CommentOutput> commentOutputs =
                from comment in comments
                join customer in context.Customer on comment.UserId equals customer.Id
                join appUser in context.Users on customer.IdentityId equals appUser.Id
                join userRole in context.UserRoles on customer.IdentityId equals userRole.UserId
                join role in context.Roles on userRole.RoleId equals role.Id
                select new CommentOutput
                {
                    Id = comment.Id,
                    ProductId = comment.ProductId,
                    Rating = comment.Rating,
                    Text = comment.Text,
                    UserAvatarId = customer.AvatarId,
                    UserEmail = appUser.Email,
                    UserId = appUser.Id,
                    Username = appUser.UserName,
                    UserProfileBackgroundId = customer.ProfileBackgroundId,
                    UserRole = role.Name,
                    UtcDatePosted = comment.UtcDatePosted
                };

            return new PagedList<CommentOutput>
            {
                Items = commentOutputs,
                Page = page,
                PageSize = pageSize,
                TotalCount = context.Comment.Where(c => c.ProductId == productId).Count(),
            };
        }

        public void Update(Comment comment)
        {
            context.Comment.Update(comment);
            context.SaveChanges();
        }
    }
}
