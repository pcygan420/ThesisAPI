﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracaAPI.Models.Metric
{
    public class SongMetric
    {
        [Key]
        public int MetricId { get; set; }
        public int EntityId { get; set; }

        [ForeignKey("EntityId")]
        public virtual Entity.Entity Entity { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        [MaxLength(128)]
        public string Title { get; set; }
        [Required]
        public string Performer { get; set; }
        public DateTime AddDate { get; set; }

        public string Album { get; set; }
        public string PublicationDate { get; set; }
        public string Duration { get; set; }
        public string Curosities { get; set; }

        public virtual string Tags { get; set; }
    }
}