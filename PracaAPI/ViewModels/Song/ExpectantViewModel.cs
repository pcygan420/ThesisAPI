using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace PracaAPI.ViewModels
{
    public class ExpectantViewModel
    {
        public int SongId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Performer { get; set; }
        [Required]
        public int Total { get; set; }

        public static ExpectantViewModel MapToViewModel(PracaAPI.Models.Song.Song song) {

            return new ExpectantViewModel
            {
                SongId = song.EntityId,
                Title = song.Title,
                Performer = song.Performer,
                Total = song.Expectants.Count()
            };
        }
    }
}