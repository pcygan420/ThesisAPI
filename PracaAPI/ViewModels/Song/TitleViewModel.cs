using System.ComponentModel.DataAnnotations;

namespace PracaAPI.ViewModels
{
    public class TitleViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Performer { get; set; }
    }
}