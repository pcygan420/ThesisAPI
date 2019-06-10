using PracaAPI.Models.Metric;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;

namespace PracaAPI.ViewModels.Metrics
{
    public class MetricViewModel
    {
        public int MetricId { get; set; }
        public int EntityId { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Performer { get; set; }
        public string AddDate { get; set; }

        public string Album { get; set; }
        public string PublicationDate { get; set; }
        public string Duration { get; set; }
        public string Curiosities { get; set; }

        public string Tags { get; set; }

        public static SongMetric MapToMetric(MetricViewModel vm, string userId) {

            return new SongMetric
            {
                EntityId = vm.EntityId,
                UserId = userId,
                Title = vm.Title,
                Performer = vm.Performer,
                AddDate = DateTime.Now,
                Album = vm.Album,
                Duration = vm.Duration,
                Curosities = vm.Curiosities,
                Tags = vm.Tags,
                PublicationDate = vm.PublicationDate
            };
        }

        public static MetricViewModel MapToViewModel(SongMetric met) {

            return new MetricViewModel
            {
                MetricId = met.MetricId,
                User = met.User.UserName,
                Title = met.Title,
                Performer = met.Performer,
                Album = met.Album,
                Duration = met.Duration,
                PublicationDate = met.PublicationDate,
                Curiosities = met.Curosities,
                Tags = met.Tags,
                AddDate = met.AddDate.ToString("MM/dd/yyyy H:mm")
            };
        }

        public static MetricViewModel MapToViewModel2(PracaAPI.Models.Song.Song song)
        {
            
            return new MetricViewModel
            {
                EntityId = song.EntityId,
                Title = song.Title,
                Performer = song.Performer,
                Album = song.Album,
                Duration = song.Duration,
                PublicationDate = song.PublicationDate,
                Curiosities = song.Curiosities,
                Tags = string.Join(",", song.Tags.Select(tag => tag.Name).ToArray() )
            };
        }
    }
}