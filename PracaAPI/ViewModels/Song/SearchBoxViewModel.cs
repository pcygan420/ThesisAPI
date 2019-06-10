using System.ComponentModel.DataAnnotations;

namespace PracaAPI.ViewModels.Song.Search
{
    public class SearchBoxViewModel
    {
        public int id { get; set; }

        [Required]
        public string title { get; set; }
        [Required]
        public string description { get; set; }

        public static SearchBoxViewModel MapToViewModel(PracaAPI.Models.Song.Song song)
        {

            return new SearchBoxViewModel()
            {
                id = song.EntityId,
                title = song.Title,
                description = song.Performer
            };
        }
    }
}