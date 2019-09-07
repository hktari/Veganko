using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models;

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

        public PagedList<Comment> GetAll(string productId, int page, int pageSize = 10)
        {
            int pageIdx = page - 1;
            if (pageIdx < 0)
            {
                throw new ArgumentException("Page idx < 0 !");
            }

            return new PagedList<Comment>
            {
                Items = context.Comment.Where(c => c.ProductId == productId).Skip(pageSize * pageIdx).Take(pageSize),
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
