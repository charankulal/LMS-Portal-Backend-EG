using LMS.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _context;

        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: get all users

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = new List<Users>();
            users = await _context.Users.ToListAsync();

            return new JsonResult(users);
        }



        //Get: get user by email
        [HttpGet("email")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Email==email);
            return new JsonResult(user);
        }

        //Get: get user by email
        [HttpGet("get-user/{Id}")]
        public async Task<IActionResult> GetUserById(int Id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Id);
            return new JsonResult(user);
        }

        [HttpGet("trainees")]
        public async Task<IActionResult> GetTrainees()
        {
            var trainees = await _context.Users
                                 .Where(u => u.Role == "Trainee")
                                 .ToListAsync();
            return new JsonResult(trainees);
        }

        [HttpGet("instructors")]
        public async Task<IActionResult> GetInstructors()
        {
            var instructors = await _context.Users
                                 .Where(u => u.Role == "Instructor")
                                 .ToListAsync();
            return new JsonResult(instructors);
        }

        // Post: Create a User {Instructor,Trainees}
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([Bind("FullName,Password,Email,Role,Points")][FromBody] Users myData)
        {
            var users = new List<Users>();

            users = await _context.Users.ToListAsync();
            //myData.Id = booking.LastOrDefault().Id + 1;


            _context.Add(myData);
            await _context.SaveChangesAsync();

            users = await _context.Users.ToListAsync();

            return new JsonResult(users);
        }

        // Delete: delete the user using Id
        [HttpDelete]
        [Route("{Id}")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            var user = await _context.Users.FindAsync(Id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            else
            {
                return new JsonResult("Error: User doesn't exist");
            }

            await _context.SaveChangesAsync();
            
            return new JsonResult("Deleted Successfully");
        }

        // PUT: update the user details
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateUsers(int Id, [FromBody] Users data)
        {
            var users = new List<Users>();
            data.Id = Id;
            _context.Update(data);
            await _context.SaveChangesAsync();
            users = await _context.Users.ToListAsync();
            return new JsonResult(users);
        }

    }
}
