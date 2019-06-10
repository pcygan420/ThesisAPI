using PracaAPI.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace PracaAPI.ViewModels
{
    public class TagViewModel
    {
        [Required]
        public string Name { get; set; }

        public static TagViewModel MapToViewModel(Tag tag) {

            return new TagViewModel
            {
                Name = tag.Name
            };
        }
    }
}