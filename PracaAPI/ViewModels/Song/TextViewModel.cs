using PracaAPI.Models.Clips;
using PracaAPI.Models.Text;
using PracaAPI.Models.Translations;
using System;
using System.ComponentModel.DataAnnotations;

namespace PracaAPI.ViewModels.Texts
{
    public class TextViewModel
    {
        public int Id { get; set; }
        public int EntityId { get; set; }

        public string User { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Performer { get; set; }
        public string AddDate { get; set; }
        public string Text { get; set; }
        public string Translation { get; set; }
        public string ClipUrl { get; set; }

        public static TextViewModel MapToViewModel(PracaAPI.Models.Song.Song song) {

            return new TextViewModel
            {
                EntityId = song.EntityId,
                Title = song.Title,
                Performer = song.Performer,
                Text = song.Text,
                Translation = song.Translation
            };
        }

        public static TextViewModel MapTextToViewModel(SongText song) {

            return new TextViewModel
            {
                Id = song.SongTextId,
                User = song.User.UserName,
                Title = song.Title,
                Performer = song.Performer,
                AddDate = song.AddDate.ToString("MM/dd/yyyy H:mm"),
                Text = song.Text
            };
        }

        public static TextViewModel MapTranToViewModel(SongTranslation tran) {

            return new TextViewModel
            {
                Id = tran.TranslationId,
                User = tran.User.UserName,
                Title = tran.Title,
                Performer = tran.Performer,
                AddDate = tran.AddDate.ToString("MM/dd/yyyy H:mm"),
                Translation = tran.Translation
            };
        }

        public static TextViewModel MapClipToViewModel(SongClipUrl clip) {

            return new TextViewModel
            {
                Id = clip.ClipId,
                User = clip.User.UserName,
                Title = clip.Title,
                Performer = clip.Performer,
                AddDate = clip.AddDate.ToString("MM/dd/yyyy H:mm"),
                ClipUrl = clip.ClipUrl
            };
        }
    }
}