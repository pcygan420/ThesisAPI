using PracaAPI.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace PracaAPI.ViewModels.Entity
{
    public class ReplyViewModel
    {
        public int ParentId { get; set; }

        [Required]
        public string User { get; set; }
        [Required]
        public string Comment { get; set; }
        public string Date { get; set; }

        public static ReplyViewModel MapToViewModel(Reply reply) {

            return new ReplyViewModel
            {
                User = reply.User.UserName,
                Comment = reply.Text,
                Date = reply.AddDate.ToString("MM/dd/yyyy H:mm")
            };
        }
    }
}