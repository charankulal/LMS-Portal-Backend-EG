using LMS.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace LMS.api.Controllers
{
    [Route("api/batches")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public BatchController(ApplicationDBContext context)
        {
            _context = context;
        }

        // Get all batches
        [HttpGet]
        public async Task<IActionResult> GetBatches()
        {
            var batches = new List<Batches>();
            batches = await _context.Batches.ToListAsync();

            return new JsonResult(batches);
        }


        // Get Batch by Id
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetBatchById(int Id)
        {
            var batch = await _context.Batches.FindAsync(Id);
            return new JsonResult(batch);
        }

        // Create a new batch
        [HttpPost("create-batch")]
        public async Task<IActionResult> CreateNewBatch([FromBody] Batches batchData)
        {
            _context.Add(batchData);
            await _context.SaveChangesAsync();
            return new JsonResult(new { id = batchData.Id });
        }

        // Delete the batch by Id
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteBatchById(int Id)
        {
            var batch = await _context.Batches.FindAsync(Id);
            if (batch != null)
            {
                _context.Batches.Remove(batch);
            }
            else
            {
                return new JsonResult("Error: batch doesn't exist");
            }

            await _context.SaveChangesAsync();

            return new JsonResult("Deleted Successfully");
        }

        // Update the batch by id 
        [HttpPut("{Id}")]

        public async Task<IActionResult> UpdateBatchById(int Id, Batches batch)
        {
            batch.Id = Id;
            _context.Update(batch);
            await _context.SaveChangesAsync();
            var batches = await _context.Batches.ToListAsync();
            return new JsonResult(batches);
        }

        // Get all batches created by particular instructor
        [HttpGet("{instructorId}/batches")]
        public async Task<IActionResult> GetBatchesByInstructorId(int instructorId)
        {
            var batches = await _context.Batches.Where(b => b.InstructorId == instructorId).ToListAsync();
            return new JsonResult(batches);
        }

        // Add a trainee to the batch
        [HttpPost("add-trainee/{UserId}/batch/{BatchId}")]
        public async Task<IActionResult> AddTraineeToBatch(int UserId, int BatchId)
        {
            var batchUser = new BatchUsers { UserId = UserId, BatchId = BatchId };
            _context.BatchUsers.Add(batchUser);
            await _context.SaveChangesAsync();
            return new JsonResult(batchUser);
        }

        // Viewing all trainees in the batches
        [HttpGet("view-trainee/batch/{BatchId}")]
        public async Task<IActionResult> GetAllTraineesInBatch(int BatchId)
        {
            var usersList = new List<Users>();
            var userIdList= await _context.BatchUsers.Where(b => b.BatchId == BatchId).ToListAsync();
            foreach (var user in userIdList)
            {
                usersList.Add(await _context.Users.FindAsync(user.UserId));
            }
            
            return new JsonResult(usersList);
        }

        // fetching all trainees who are not in current batch
        [HttpGet("fetch-trainee/batch/{BatchId}")]
        public async Task<IActionResult> GetAllTraineesNotInBatch(int BatchId)
        {
            var usersList = new List<Users>();
            var userIdListInBatch = await _context.BatchUsers
                                                  .Where(b => b.BatchId == BatchId)
                                                  .Select(b => b.UserId)
                                                  .ToListAsync();

            usersList = await _context.Users
                          .Where(u => !userIdListInBatch.Contains(u.Id) && u.Role == "Trainee")
                          .ToListAsync();


            return new JsonResult(usersList);
        }

        // remove trainee from the batch 
        [HttpDelete("remove-trainee/batch/{BatchId}/trainee/{UserId}")]
        public async Task<IActionResult> GetAllTraineesNotInBatch(int BatchId, int UserId)
        {
            var trainee = await _context.BatchUsers.Where(b=>b.BatchId == BatchId).Where(b=>b.UserId == UserId).FirstOrDefaultAsync();

            if (trainee == null)
            {
                return NotFound(new { message = "Trainee not found." });
            }

           

            _context.BatchUsers.Remove(trainee);
            await _context.SaveChangesAsync();

            return new JsonResult("Trainee removed successfully from the batch." );
        }

    }
}
