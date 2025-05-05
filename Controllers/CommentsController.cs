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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int productionId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("Comment text cannot be empty.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var comment = new Comment
            {
                Text = text,
                ProductionId = productionId,
                UserId = user.Id,
                Username = user.UserName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return await RenderCommentsPartial(productionId);
        }

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

            int productionId = comment.ProductionId;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return await RenderCommentsPartial(productionId);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCommentsPartial(int productionId)
        {
            return await RenderCommentsPartial(productionId);
        }

        private async Task<IActionResult> RenderCommentsPartial(int productionId)
        {
            var comments = await _context.Comments
                .AsNoTracking()
                .Where(c => c.ProductionId == productionId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUsername = user?.UserName;

            ViewBag.ProductionId = productionId;
            return PartialView("~/Views/Shared/_CommentModalPartial.cshtml", comments);
        }
    }
}
