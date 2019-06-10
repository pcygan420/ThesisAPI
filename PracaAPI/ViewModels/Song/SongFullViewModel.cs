using PracaAPI.Models.Entity;
using PracaAPI.Models.Song;
using PracaAPI.ViewModels.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PracaAPI.ViewModels
{
    public class SongFullViewModel
    {
        public int EntityId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Performer { get; set; }
        [Required]
        public string Text { get; set; }
        public string Translation { get; set; }
        public string TelediskUrl { get; set; }

        public string Album { get; set; }
        public string PublicationDate { get; set; }
        public string Duration { get; set; }
        public string Curiosities { get; set; }
        public string Tags { get; set; }

        public int? UserRating { get; set; }
        public bool UserFavourite { get; set; }
        public double RateAvg { get; set; }
        public int RatesTotal { get; set; }

        public ICollection<ActionViewModel> Actions { get; set; }
        public IEnumerable<CommentViewModel> Comments { get; set; }

        public int TotalExpectants { get; set; }
        public bool IsUserExpecting { get; set; }

        public static SongFullViewModel MapToViewModel(PracaAPI.Models.Song.Song song, string userId, Rate userRate, Favourite favour) {

            var actions = new List<ActionViewModel>();

            if (song.Actions != null) {

                actions.AddRange(song.Actions.Select(act => ActionViewModel.MapToViewModel(act)));
            }

            var vm = new SongFullViewModel
            {
                EntityId = song.EntityId,
                Title = song.Title,
                Performer = song.Performer,
                Text = song.Text,
                Translation = song.Translation,
                TelediskUrl = song.ClipUrl,
                Album = song.Album,
                PublicationDate = song.PublicationDate,
                Duration = song.Duration,
                Curiosities = song.Curiosities,
                Tags = string.Join(",",song.Tags.Select(tag => tag.Name)),
                Actions = actions,
                RateAvg =  song.Rates.Count() == 0 ? 0 : song.Rates.Average(rate => rate.Rating),
                RatesTotal = song.Rates.Count(),
                
                Comments = song.Comments.Select(com => new CommentViewModel
                                                {
                                                    CommentId = com.CommentId,
                                                    UserName = com.User.UserName,
                                                    Text = com.Text,
                                                    Date = com.AddDate.ToString("MM/dd/yyyy H:mm"),
                                                    Replies = com.Replies.Select(rep => ReplyViewModel.MapToViewModel(rep))
                                                }),
                TotalExpectants = song.Expectants.Count,
                IsUserExpecting = userId == null ? false : song.Expectants.Any(exp => exp.UserId == userId)
            };

            if (userRate != null)
                vm.UserRating = userRate.Rating;
            
            vm.UserFavourite = favour != null ? true : false;

            return vm;
        }
    }
}