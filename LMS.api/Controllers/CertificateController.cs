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
        public CertificateController(ApplicationDBContext context)
        {
            _context = context;
        }

        // Create a new certificate
        [HttpPost("create-certificate")]
        public async Task<IActionResult> CreateNewCertificate([FromBody] Certificates data)
        {
            var certificates = new List<Certificates>();

            certificates = await _context.Certificates.ToListAsync();
            _context.Add(data);
            await _context.SaveChangesAsync();

            certificates = await _context.Certificates.ToListAsync();

            return new JsonResult(certificates);
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
            certiifcate.Id = Id;
            _context.Update(certiifcate);
            await _context.SaveChangesAsync();
            var certificates = await _context.Certificates.ToListAsync();
            return new JsonResult(certificates);
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
                return new JsonResult("Error: Certificate doesn't exist");
            }

            await _context.SaveChangesAsync();

            return new JsonResult("Deleted Successfully");
        }
    }
}
