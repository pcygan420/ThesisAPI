using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracaAPI.Models.Text
{
    public class SongText
    {
        [Key]
        public int SongTextId { get; set; }

        public int EntityId { get; set; }
        [ForeignKey("EntityId")]
        public virtual Entity.Entity Entity { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        [MaxLength(128)]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }

        public string Performer { get; set; }
        public DateTime AddDate { get; set; }
    }
}