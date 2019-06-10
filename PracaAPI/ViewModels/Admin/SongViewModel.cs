using System.Linq;

namespace PracaAPI.ViewModels.Admin
{
    public class SongViewModel
    {
        public int EntityId { get; set; }
        public string Title { get; set; }
        public string Performer { get; set; }

        public string Text { get; set; }
        public string Translation { get; set; }
        public string ClipUrl { get; set; }

        public string Genres { get; set; }
        public string Album { get; set; }
        public string Publication { get; set; }
        public string Duration { get; set; }
        public string Curiosites { get; set; }

        public static SongViewModel MapToViewModel(PracaAPI.Models.Song.Song song) {

            return new SongViewModel
            {
                EntityId = song.EntityId,
                Title = song.Title,
                Performer = song.Performer,
                Text = song.Text,
                Translation = song.Translation,
                ClipUrl = song.ClipUrl,
                Genres = string.Join(",",song.Tags.Select(tag => tag.Name)),
                Album = song.Album,
                Publication = song.PublicationDate,
                Duration = song.Duration,
                Curiosites = song.Curiosities
            };
        }
    }
}