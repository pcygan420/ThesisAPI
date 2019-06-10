using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracaAPI.Models.Song
{
    [Table("Songs")]
    public class Song : Entity.Entity
    {
        [Required]
        [MaxLength(128)]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }

        public string Translation { get; set; }
        public string ClipUrl { get; set; }

        public string Album { get; set; }
        public string Performer { get; set; }
        public string PublicationDate { get; set; }
        public string Duration { get; set; }
        public string Curiosities { get; set; }

        public virtual ICollection<Action> Actions { get; set; }
        public virtual ICollection<Expectant> Expectants { get; set; }
    }
}