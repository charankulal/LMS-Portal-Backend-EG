﻿using System.ComponentModel.DataAnnotations;

namespace LMS.api.Models
{
    public class Certificates
    {
        [Key]
        public int Id { get; set; }

        public int SprintId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Points { get; set; }

      
    }
}
