using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracaAPI.Models.Entity
{
    public class Rate
    {
        [Key]
        public int RateId { get; set; }

        public int EntityId { get; set; }
        [ForeignKey("EntityId")]
        public virtual Entity Entity { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public DateTime Date { get; set; }
        [Required]
        public int Rating { get; set; }
    }
}