using PracaAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PracaAPI.ViewModels
{
    public class ActionViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Performer { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        public string AddDate { get; set; }
        [Required]
        public Actions Status { get; set; }

        public static ActionViewModel MapToViewModel(Models.Song.Action action) {

            var vm = new ActionViewModel
            {
                User = action.User.UserName,
                AddDate = action.Date.ToString("MM/dd/yyyy H:mm"),
                Status = action.Status
            };

            return vm;
        }
    }
}