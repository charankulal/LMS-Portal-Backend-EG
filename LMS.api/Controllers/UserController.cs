﻿using LMS.api.Interfaces;
using LMS.api.Models;
using LMS.api.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace LMS.api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IEmailSender _emailSender;

        public UserController(ApplicationDBContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
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

        // get all trainees
        [HttpGet("trainees")]
        public async Task<IActionResult> GetTrainees()
        {
            try
            {
                var trainees = await _context.Users
                                 .Where(u => u.Role == "Trainee")
                                 .ToListAsync();
                return new JsonResult(trainees);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return NotFound(ex.Message);
            }
        }

        // get all instructors
        [HttpGet("instructors")]
        public async Task<IActionResult> GetInstructors()
        {
            try
            {
                var instructors = await _context.Users
                                     .Where(u => u.Role == "Instructor")
                                     .ToListAsync();
                return new JsonResult(instructors);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return NotFound(ex.Message);
            }
        }

        // Post: Create a User {Instructor,Trainees}
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([Bind("FullName,Password,Email,Role,Points")][FromBody] Users myData)
        {
            try
            {
                PasswordHasher hasher = new PasswordHasher();
                var password = myData.Password;
                var users = new List<Users>();
                myData.Password = hasher.ComputeHash(myData.Password, SHA256.Create(), Encoding.UTF8.GetBytes("lms"));
                _context.Add(myData);
                await _context.SaveChangesAsync();

                // notifying the trainee upon adding to the batch
                var receiver = myData.Email;
                var subject = "Welcome to LMS";
                var message = $@"
                            Hi {myData.FullName},

                            You've been given access to LMS.

                            The credentials are:
                            - **Email**: {myData.Email}
                            - **Password**: {password}

                            Best regards,
                            Your LMS Team
                            ";


                await _emailSender.SendEmailAsync(receiver, subject, message);
                users = await _context.Users.ToListAsync();

                return new JsonResult(users);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return NotFound(ex.Message);
            }
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
                return NotFound("Error: User doesn't exist");
            }

            await _context.SaveChangesAsync();
            
            return new JsonResult("Deleted Successfully");
        }

        // PUT: update the user details
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateUsers(int Id, [FromBody] Users data)
        {
            PasswordHasher hasher = new PasswordHasher();
            var password = data.Password;
            var users = new List<Users>();
            data.Password = hasher.ComputeHash(data.Password, SHA256.Create(), Encoding.UTF8.GetBytes("lms"));
            
            data.Id = Id;
            _context.Update(data);
            await _context.SaveChangesAsync();
            users = await _context.Users.ToListAsync();
            // notifying the trainee upon adding to the batch
            var receiver = data.Email;
            var subject = "Your Password Updated";
            var message = $@"
                            Hi {data.FullName},

                            

                            The updated credentials are:
                            - **Email**: {data.Email}
                            - **Password**: {password}

                            Best regards,
                            Your LMS Team
                            ";


            await _emailSender.SendEmailAsync(receiver, subject, message);
            users = await _context.Users.ToListAsync();
            return new JsonResult(users);
        }

        

    }
}
