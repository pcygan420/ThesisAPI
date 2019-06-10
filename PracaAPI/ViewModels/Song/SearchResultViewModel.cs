using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace PracaAPI.ViewModels
{
    public class SearchResultViewModel
    {
        public int id { get; set; }

        [Required]
        public string title { get; set; }
        [Required]
        public string description { get; set; }

        public string Album { get; set; }
        public string Publication { get; set; }
        public string Duration { get; set; }
        public string Genres { get; set; }

        public double Rate { get; set; }
        public int? UserRate { get; set; }
        [Required]
        public int Votes { get; set; }

        public static SearchResultViewModel MapToViewModel(PracaAPI.Models.Song.Song song, string userId) {

            var userRate = song.Rates.FirstOrDefault(rate => rate.UserId == userId);

            return new SearchResultViewModel()
            {
                id = song.EntityId,
                title = song.Title,
                description = song.Performer,
                Album = song.Album,
                Publication = song.PublicationDate,
                Duration = song.Duration,
                Genres = string.Join(", ", song.Tags.Select(tag => tag.Name).ToArray()),
                Rate = song.Rates.Average(rate => rate.Rating),
                UserRate = userRate != null ? userRate.Rating : (int?)null,
                Votes = song.Rates.Count()
            };
        }

        public static SearchResultViewModel MapToViewModel2(PracaAPI.Models.Song.Song song)
        {

            return new SearchResultViewModel()
            {
                id = song.EntityId,
                title = song.Title,
                description = song.Performer,
                Album = song.Album,
                Publication = song.PublicationDate,
                Duration = song.Duration,
                Genres = string.Join(", ", song.Tags.Select(tag => tag.Name).ToArray()),
                Rate = song.Rates.Average(rate => rate.Rating),
                Votes = song.Rates.Count()
            };
        }
    }
}