using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PracaAPI.Models.Entity
{
    public abstract class Entity
    {
        [Key]
        public int EntityId { get; set; }
        public ICollection<Tag> Tags { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Rate> Rates { get; set; }
        public virtual ICollection<Favourite> Favourites { get; set; }
    }
}