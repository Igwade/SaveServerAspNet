using System;
using System.ComponentModel.DataAnnotations;

namespace SaveLoadApp.Models
{
    public class Record
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Version { get; set; }
        
        [Required]
        public string Content { get; set; }
    }
}