using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StageCraft.Data;
using StageCraft.Models;

namespace StageCraft.Controllers

{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Create a comment (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int productionId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("Comment text cannot be empty.");

            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            var comment = new Comment
            {
                Text = text,
                ProductionId = productionId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow  //  Use UTC for consistency
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return await RenderCommentsPartial(productionId);
        }

        //  Delete a comment (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            var isManager = User.IsInRole("ProductionManager");

            if (comment.UserId != userId && !isAdmin && !isManager)
                return Forbid();

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return await RenderCommentsPartial(comment.ProductionId);
        }

        //  Load comments dynamically (first load)
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCommentsPartial(int productionId)
        {
            return await RenderCommentsPartial(productionId);
        }

        //  Common private method to load comments and render the Partial View
        private async Task<IActionResult> RenderCommentsPartial(int productionId)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.ProductionId == productionId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.ProductionId = productionId;
            return PartialView("~/Views/Shared/_CommentModalPartial.cshtml", comments);
        }
    }
}
