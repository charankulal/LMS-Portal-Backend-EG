using LMS.api.Interfaces;
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
        private readonly IEmailSender _emailSender;
        public ContentController(ApplicationDBContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // Create a new post
        [HttpPost("create-post")]
        public async Task<IActionResult> CreateNewPost([FromBody] Contents data)
        {
            var posts = new List<Contents>();

            posts = await _context.Contents.ToListAsync();

            var sprint = await _context.Sprints.FindAsync(data.SprintId);
            var batch = await _context.Batches.FindAsync(sprint.BatchId);
            var instructor = await _context.Users.FindAsync(batch.InstructorId);
            var batchUsers = await _context.BatchUsers.Where(b=>b.BatchId==sprint.BatchId).ToListAsync();

            foreach (var batchUser in batchUsers)
            {
                var user = await _context.Users.FindAsync(batchUser.UserId);
                // notifying the trainee upon adding to the batch
                var receiver = user.Email;
                var subject = "New post in "+ batch.Name;
                var message = "Hi " + user.FullName + ", "+instructor.FullName + "  added new post :  "+data.Title;

                await _emailSender.SendEmailAsync(receiver, subject, message);
            }
            
            
            _context.Add(data);
            await _context.SaveChangesAsync();

            posts = await _context.Contents.ToListAsync();

            return new JsonResult(posts);
        }

        // For Announcements
        [HttpPost("announcement")]
        public async Task<IActionResult> NewAnnounce([FromBody] Announcement data)
        {
            
            var batch = await _context.Batches.FindAsync(data.BatchId);
            var instructor = await _context.Users.FindAsync(batch.InstructorId);
            var batchUsers = await _context.BatchUsers.Where(b => b.BatchId == data.BatchId).ToListAsync();

            foreach (var batchUser in batchUsers)
            {
                var user = await _context.Users.FindAsync(batchUser.UserId);
                // notifying the trainee upon adding to the batch
                var receiver = user.Email;
                var subject = instructor.FullName+" Announced: "+data.Subject;
                var message = "Hi " + user.FullName + ", "+data.Message;

                await _emailSender.SendEmailAsync(receiver, subject, message);
            }
            return new JsonResult("Success");
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
