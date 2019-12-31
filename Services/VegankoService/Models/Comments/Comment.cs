using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.Comments
{
    public class Comment
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public int? Rating { get; set; }
        public string Text { get; set; }
        public DateTime UtcDatePosted { get; set; }
    }
}
