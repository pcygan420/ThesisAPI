using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracaAPI.Models.Added
{
    public class AddedSong
    {
        [Key]
        public int AddedSongId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        [MaxLength(128)]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }

        [MaxLength(64)]
        public string Album { get; set; }
        [MaxLength(64)]
        public string Performer { get; set; }
        public string PublicationDate { get; set; }
        public string Duration { get; set; }
        public DateTime AddDate { get; set; }

    }
}