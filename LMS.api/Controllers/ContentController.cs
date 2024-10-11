using LMS.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.api.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class ContentController : Controller
    {
        private readonly ApplicationDBContext _context;
        public ContentController(ApplicationDBContext context)
        {
            _context = context;
        }

        // Create a new post
        [HttpPost("create-post")]
        public async Task<IActionResult> CreateNewPost([FromBody] Contents data)
        {
            var posts = new List<Contents>();

            posts = await _context.Contents.ToListAsync();
            _context.Add(data);
            await _context.SaveChangesAsync();

            posts = await _context.Contents.ToListAsync();

            return new JsonResult(posts);
        }

        //Get Posts by the sprint Id
        [HttpGet("view-posts/{SprintId}")]
        public async Task<IActionResult>  GetPostsBySprintId(int SprintId)
        {
            var posts = new List<Contents>();
            posts= await _context.Contents.Where(c=>c.SprintId==SprintId).ToListAsync();

            return new JsonResult(posts);
        }

        //Get Posts by the post Id
        [HttpGet("view-post/{Id}")]
        public async Task<IActionResult> GetPostById(int Id)
        {
            var posts = new List<Contents>();
            posts = await _context.Contents.Where(c => c.ContentId == Id).ToListAsync();

            return new JsonResult(posts);
        }

        // update posts by post id
        [HttpPut("update-post/{Id}")]

        public async Task<IActionResult> UpdatePostById(int Id, Contents content)
        {
            content.ContentId = Id;
            _context.Update(content);
            await _context.SaveChangesAsync();
            var posts = await _context.Contents.ToListAsync();
            return new JsonResult(posts);
        }

        // Delete the batch by Id
        [HttpDelete("delete-post/{Id}")]
        public async Task<IActionResult> DeletePostById(int Id)
        {
            var post = await _context.Contents.FindAsync(Id);
            if (post != null)
            {
                _context.Contents.Remove(post);
            }
            else
            {
                return new JsonResult("Error: post doesn't exist");
            }

            await _context.SaveChangesAsync();

            return new JsonResult("Deleted Successfully");
        }
    }
}
