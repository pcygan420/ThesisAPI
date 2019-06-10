using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracaAPI.Models.Translations
{
    public class SongTranslation
    {
        [Key]
        public int TranslationId { get; set; }

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
        public string Performer { get; set; }
        [Required]
        public string Translation { get; set; }

        public DateTime AddDate { get; set; }
    }
}