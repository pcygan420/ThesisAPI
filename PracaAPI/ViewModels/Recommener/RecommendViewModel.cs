using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PracaAPI.ViewModels
{
    public class RecommendViewModel
    {
        public int id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Performer { get; set; }
        [Required]
        public double PredictedRate { get; set; }

        public string Tags { get; set; }
        public string Album { get; set; }
        public string Duration { get; set; }
        public string Publication { get; set; }

        public double Rate { get; set; }
        public int Votes { get; set; }


        public static RecommendViewModel MapToViewModel(PracaAPI.Models.Song.Song song, double predicted_rate) {

            return new RecommendViewModel
            {
                id = song.EntityId,
                Title = song.Title,
                Performer = song.Performer,
                PredictedRate = predicted_rate,
                Tags = string.Join(", ",song.Tags.Select(t => t.Name)),
                Album = song.Album,
                Duration = song.Duration,
                Publication = song.PublicationDate,
                Rate = song.Rates.Average(rate => rate.Rating),
                Votes = song.Rates.Count()
            };
        }
    }
}