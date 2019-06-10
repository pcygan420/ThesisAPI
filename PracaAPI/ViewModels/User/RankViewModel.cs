using PracaAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace PracaAPI.ViewModels.User
{
    public class RankViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public int Rank { get; set; }
        [Required]
        public int Points { get; set; }
        [Required]
        public string Joined { get; set; }

        public static RankViewModel MapToViewModel(ApplicationUser user, int index) {

            return new RankViewModel
            {
                UserName = user.UserName,
                Rank = index,
                Points = user.Points,
                Joined = user.JoinDate
            };
        }
    }
}