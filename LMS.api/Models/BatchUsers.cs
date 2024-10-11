using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.api.Models
{
    public class BatchUsers
    {
        
        public int BatchId { get; set; }
        public int UserId { get; set; }

    }
}
