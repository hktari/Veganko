using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VegankoService.Data;
using VegankoService.Data.Comments;
using VegankoService.Helpers;
using VegankoService.Models;
using VegankoService.Models.Comments;
using VegankoService.Models.User;

namespace VegankoService.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "ApiUser")]
    [ApiController]
    public class CommentsController : Controller
    {
        private readonly ICommentRepository commentRepository;
        private readonly VegankoContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ClaimsPrincipal caller;

        public CommentsController(
            ICommentRepository commentRepository, IHttpContextAccessor httpContextAccessor, VegankoContext context, UserManager<ApplicationUser> userManager)
        {
            this.commentRepository = commentRepository;
            this.context = context;
            this.userManager = userManager;
            caller = httpContextAccessor.HttpContext.User;
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
        public ActionResult<PagedList<CommentOutput>> GetAll(string productId, int page = 1, int pageSize = 10)
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
        public async Task<ActionResult<Comment>> Post([FromBody] CommentInput input)
        {
            Models.User.Customer customer = await Identity.CurrentCustomer(caller, context);

            Comment comment = new Comment();
            comment.UtcDatePosted = DateTime.UtcNow;
            input.MapToComment(comment);
            comment.UserId = customer.Id;
            commentRepository.Create(comment);
            return CreatedAtAction(
                nameof(CommentsController.Get), "comments", new { id = comment.Id }, comment);
        }

        // PUT: api/Comments/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Comment>> Put(string id, [FromBody] CommentInput input)
        {
            var comment = commentRepository.Get(id);
            if (comment == null)
            {
                return NotFound();
            }

            var user = await Identity.CurrentCustomer(caller, context);
            if (user.Id != comment.UserId)
            {
                return Forbid();
            }

            input.MapToComment(comment);
            commentRepository.Update(comment);
            return comment;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            Models.User.Customer customer = await Identity.CurrentCustomer(caller, context);
            
            var comment = commentRepository.Get(id);
            if (comment == null)
            {
                return NotFound();
            }

            var userIdentity = await userManager.FindByIdAsync(
                Identity.GetUserIdentityId(caller));
            var userRole = (await userManager.GetRolesAsync(userIdentity)).FirstOrDefault();
            if (!CanRoleModifyComment(userRole) && comment.UserId != customer.Id)
            {
                return Forbid();
            }

            commentRepository.Delete(id);
            return Ok();
        }

        private bool CanRoleModifyComment(string role)
        {
            return role == Constants.Strings.Roles.Admin
                || role == Constants.Strings.Roles.Manager
                || role == Constants.Strings.Roles.Moderator;
        }
    }
}
