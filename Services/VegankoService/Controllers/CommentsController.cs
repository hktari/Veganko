using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VegankoService.Data.Comments;
using VegankoService.Models;

namespace VegankoService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : Controller
    {
        private readonly ICommentRepository commentRepository;

        public CommentsController(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        // GET: api/Comments
        [HttpGet("{id}")]
        public ActionResult<Comment> Get(string id)
        {
            Comment comment = commentRepository.Get(id);
            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        [HttpGet]
        public ActionResult<PagedList<Comment>> GetAll(string productId, int page = 1, int pageSize = 10)
        {
            if (productId == null)
            {
                return BadRequest("ProductId required");
            }

            if (page < 1)
            {
                return BadRequest("Page index starts with 1.");
            }

            return commentRepository.GetAll(productId, page, pageSize);
        }

        // POST: api/Comments
        [HttpPost]
        public ActionResult<Comment> Post([FromBody] CommentInput input)
        {
            Comment comment = new Comment();
            comment.UtcDatePosted = DateTime.UtcNow;
            input.MapToComment(comment);

            commentRepository.Create(comment);
            return CreatedAtAction(
                nameof(CommentsController.Get), "comments", new { id = comment.Id }, comment);
        }

        // PUT: api/Comments/5
        [HttpPut("{id}")]
        public ActionResult<Comment> Put(string id, [FromBody] CommentInput input)
        {
            var comment = commentRepository.Get(id);
            if (comment == null)
            {
                return NotFound();
            }

            input.MapToComment(comment);
            commentRepository.Update(comment);
            return comment;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var comment = commentRepository.Get(id);
            if (comment == null)
            {
                return NotFound();
            }

            commentRepository.Delete(id);
            return Ok();
        }
    }
}
