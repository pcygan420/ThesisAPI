using System.ComponentModel.DataAnnotations;

namespace PracaAPI.Models.Entity
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}