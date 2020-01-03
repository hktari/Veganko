using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.Comments
{
    public class CommentInput
    {
        public string ProductId { get; set; }
        // Not being used
        public string UserId { get; set; }
        public int? Rating { get; set; }
        public string Text { get; set; }

        public void MapToComment(Comment comment)
        {
            comment.ProductId = ProductId;
            comment.UserId = UserId;
            comment.Rating = Rating;
            comment.Text = Text;
        }
    }
}
