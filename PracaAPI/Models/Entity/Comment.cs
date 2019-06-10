using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracaAPI.Models.Entity
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        public int EntityId { get; set; }
        [ForeignKey("EntityId")]
        public virtual Entity Entity { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public DateTime AddDate { get; set; }
        [Required]
        [MaxLength(500)]
        public string Text { get; set; }

        public virtual ICollection<Reply> Replies { get; set; }
    }
}