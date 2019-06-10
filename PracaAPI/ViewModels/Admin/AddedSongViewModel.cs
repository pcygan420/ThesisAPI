using PracaAPI.Models.Added;
using System.ComponentModel.DataAnnotations;

namespace PracaAPI.ViewModels.Admin
{
    public class AddedSongViewModel
    {
        [Required]
        public int AddedSongId { get; set; }
        [Required]
        public int EntityId { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Performer { get; set; }
        public string AddDate { get; set; }
        [Required]
        public string Text { get; set; }

        public static AddedSongViewModel MapToViewModel(AddedSong song) {

            var vm = new AddedSongViewModel
            {
                User = song.User.UserName,
                AddedSongId = song.AddedSongId,
                Title = song.Title,
                Performer = song.Performer,
                AddDate = song.AddDate.ToString("MM/dd/yyyy H:mm"),
                Text = song.Text
            };

            return vm;
        }
    }
}