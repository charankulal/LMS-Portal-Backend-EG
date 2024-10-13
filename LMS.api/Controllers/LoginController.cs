using LMS.api.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using LMS.api.Utilities;
using System.Text;

namespace LMS.api.Controllers
{

    [Route("api/authorize")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ApplicationDBContext _context;
        public LoginController(ApplicationDBContext context)
        {
            _context = context;
        }

        // POST: api/User/Login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            PasswordHasher hasher = new PasswordHasher();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginModel.Email);
            loginModel.Password  = hasher.ComputeHash(loginModel.Password, SHA256.Create(), Encoding.UTF8.GetBytes("lms"));

            Console.WriteLine(loginModel.Password);
            Console.WriteLine(user.Password);
            if (user == null || loginModel.Password != user.Password)
            {
                return Unauthorized("Invalid login attempt.");
            }

            return new JsonResult(Ok("Login successful"));
        }

        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out successfully");
        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<IActionResult> GetUserData()
        {
            var usersList = await _context.Users.ToListAsync();
            return Ok(usersList);
        }
    }
}
