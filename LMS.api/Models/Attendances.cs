using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.api.Models
{
    public class Attendances
    {
        
        public int BatchId { get; set; }
        

        
        public int UserId { get; set; }  

        public DateOnly Date {  get; set; } 

        public string Remarks { get; set; }

    }
}
