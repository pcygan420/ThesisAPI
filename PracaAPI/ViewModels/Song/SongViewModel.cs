using PracaAPI.Models.Added;
using PracaAPI.Models.Enums;
using PracaAPI.Models.Song;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PracaAPI.ViewModels
{
    public class SongViewModel
    {
        public int EntityId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Performer { get; set; }
        [Required]
        public string Text { get; set; }
        public string Translation { get; set; }

        public string Album { get; set; }
        public string PublicationDate { get; set; }
        public string Duration { get; set; }
        public ActionViewModel TextUser { get; set; }
        public ActionViewModel TransUser { get; set; }
        public ActionViewModel ClipUser { get; set; }

        public static SongViewModel MapToViewModel(PracaAPI.Models.Song.Song song) {

            var vm = new SongViewModel
            {
                EntityId = song.EntityId,
                Title = song.Title,
                Performer = song.Performer,
                Album = song.Album,
                PublicationDate = song.PublicationDate,
                Duration = song.Duration,
                Text = song.Text,
                Translation = song.Translation,
                TextUser = song.Actions.Select(act => ActionViewModel.MapToViewModel(act))
                                       .FirstOrDefault(act => act.Status == Actions.AddText),
                TransUser = song.Actions.Select(act => ActionViewModel.MapToViewModel(act))
                                        .FirstOrDefault(act => act.Status == Actions.AddTranslation),
                ClipUser = song.Actions.Select(act => ActionViewModel.MapToViewModel(act))
                                       .FirstOrDefault(act => act.Status == Actions.AddUrl),
            };

            return vm;
        }

        public static AddedSong MapToObject(SongViewModel vm, string userId) {

            return new AddedSong
            {
                UserId = userId,
                Title = vm.Title,
                Performer = vm.Performer,
                Text = vm.Text,
                Album = vm.Album,
                Duration = vm.Duration,
                PublicationDate = vm.PublicationDate,
                AddDate = DateTime.Now
            };
        }
    }
}