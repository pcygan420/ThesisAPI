using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracaAPI.Models.Entity
{
    public class Reply
    {
        [Key]
        public int ReplyId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public DateTime AddDate { get; set; }

        [MaxLength(500)]
        public string Text { get; set; }
    }
}