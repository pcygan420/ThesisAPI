using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracaAPI.Models.Song
{
    public class Expectant
    {
        [Key]
        public int ExpectantId { get; set; }
        
        public int SongId { get; set; }
        [ForeignKey("SongId")]
        public virtual Song Song { get; set; }

        public string UserId { get; set; }
    }
}