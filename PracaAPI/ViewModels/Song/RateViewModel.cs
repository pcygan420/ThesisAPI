using PracaAPI.Models.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PracaAPI.ViewModels
{
    public class RateViewModel
    {
        public int RateId { get; set; }
        public int EntityId { get; set; }
        [Required]
        public int Rate { get; set; }
        public string Date { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Performer { get; set; }
        public string Tags { get; set; }

        public static Rate MapToObject(RateViewModel vm, string userId) {

            return new Rate
            {
                EntityId = vm.EntityId,
                UserId = userId,
                Rating = vm.Rate,
                Date = DateTime.Now
            };
        }

        public static RateViewModel MapToViewModel(Rate rate) {

            var song = rate.Entity as PracaAPI.Models.Song.Song;

            return new RateViewModel
            {
                EntityId = rate.EntityId,
                Rate = rate.Rating,
                Date = rate.Date.ToString("MM/dd/yyyy H:mm"),
                Title = song.Title,
                Performer = song.Performer,
                Tags = string.Join(",", rate.Entity.Tags.Select(tag => tag.Name).ToArray())
            };
        }

        public static RateViewModel MapToViewModel2(Favourite fav, string userId)
        {
            var song = fav.Entity as PracaAPI.Models.Song.Song;

            return new RateViewModel
            {
                EntityId = song.EntityId,
                Title = song.Title,
                Performer = song.Performer,
                Rate = fav.Entity.Rates.FirstOrDefault(r => r.UserId == userId).Rating,
                Tags = string.Join(",", fav.Entity.Tags.Select(tag => tag.Name).ToArray())
            };
        }
    }
}