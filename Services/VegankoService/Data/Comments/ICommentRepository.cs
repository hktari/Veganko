using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models;
using VegankoService.Models.Comments;

namespace VegankoService.Data.Comments
{
    public interface ICommentRepository
    {
        PagedList<CommentOutput> GetAll(string productId, int page, int pageSize = 10);
        Comment Get(string id);
        void Create(Comment comment);
        void Update(Comment comment);
        void Delete(string id);
    }
}
