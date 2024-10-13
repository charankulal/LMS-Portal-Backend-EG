using LMS.api.Interfaces;
using LMS.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.api.Controllers
{

    [Route("api/certificate")]
    [ApiController]
    public class CertificateController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IEmailSender _emailSender;
        public CertificateController(ApplicationDBContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // Create a new certificate
        [HttpPost("create-certificate")]
        public async Task<IActionResult> CreateNewCertificate([FromBody] Certificates data)
        {
            var certificates = new List<Certificates>();

            try
            {
                var sprint = await _context.Sprints.FindAsync(data.SprintId);
                var batch = await _context.Batches.FindAsync(sprint.BatchId);
                var instructor = await _context.Users.FindAsync(batch.InstructorId);
                var batchUsers = await _context.BatchUsers.Where(b => b.BatchId == sprint.BatchId).ToListAsync();

                foreach (var batchUser in batchUsers)
                {
                    var user = await _context.Users.FindAsync(batchUser.UserId);
                    // notifying the trainee upon adding to the batch
                    var receiver = user.Email;
                    var subject = "New post in " + batch.Name;
                    var message = "Hi " + user.FullName + ", " + instructor.FullName + "  added new certification :  " + data.Name;

                    await _emailSender.SendEmailAsync(receiver, subject, message);
                }

                certificates = await _context.Certificates.ToListAsync();
                _context.Add(data);
                await _context.SaveChangesAsync();

                certificates = await _context.Certificates.ToListAsync();

                return new JsonResult(certificates);
            }catch(Exception ex)
            {
                ex.ToString();
                return BadRequest(ex.Message);
            }
        }

        // get certiifcates by sprint id
        [HttpGet("get-certificates/{SprintId}")]
        public async Task<IActionResult> GetCertificatesBySprintId(int SprintId)
        {
            var certificates = new List<Certificates>();
            certificates = await _context.Certificates.Where(c => c.SprintId == SprintId).ToListAsync();

            return new JsonResult(certificates);
        }

        // get certificate by cerificate id
        [HttpGet("get-certificate/{Id}")]
        public async Task<IActionResult> GetCertificateById(int Id)
        {
            return new JsonResult(await _context.Certificates.Where(c => c.Id == Id).ToListAsync());
        }

        // Update the certificate by id 
        [HttpPut("update-certificate/{Id}")]

        public async Task<IActionResult> UpdateCertificateById(int Id, Certificates certiifcate)
        {
            try
            {
                certiifcate.Id = Id;
                _context.Update(certiifcate);
                await _context.SaveChangesAsync();
                var certificates = await _context.Certificates.ToListAsync();
                return new JsonResult(certificates);
            }
            catch(Exception ex) { ex.ToString(); return BadRequest(ex.Message); }
        }

        // Delete the certificate by Id
        [HttpDelete("delete-certificate/{Id}")]
        public async Task<IActionResult> DeleteCertificateById(int Id)
        {
            var certificate = await _context.Certificates.FindAsync(Id);
            if (certificate != null)
            {
                _context.Certificates.Remove(certificate);
            }
            else
            {
                return NotFound("Error: Certificate doesn't exist");
            }

            await _context.SaveChangesAsync();

            return new JsonResult("Deleted Successfully");
        }

        // get all certificates
        [HttpGet("all")]
        public async Task<IActionResult> getAllCertificates()
        {
            var certificates = await _context.Certificates.ToListAsync();
            return new JsonResult(certificates);
        }
    }
}
