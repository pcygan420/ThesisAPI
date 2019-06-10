using PracaAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace PracaAPI.ViewModels
{
    public class ProfilViewModel
    {
        public string Sex { get; set; }
        public string JoinDate { get; set; }

        public int Rank { get; set; }
        public int Points { get; set; }

        public int RatesTotal { get; set; }
        public int FavTotal { get; set; }
        public int SongsTotal { get; set; }
        public int TransTotal { get; set; }
        public int ClipsTotal { get; set; }

        public static ProfilViewModel MapToViewModel(ApplicationUser user, int ratesTotal, int favTotal, int songsTotal, int transTotal, int clipsTotal, int points, int rank) {

            var vm = new ProfilViewModel
            {
                JoinDate = user.JoinDate,
                Sex = user.Sex,
                RatesTotal = ratesTotal,
                FavTotal = favTotal,
                SongsTotal = songsTotal,
                TransTotal = transTotal,
                ClipsTotal = clipsTotal,
                Points = points,
                Rank = rank
            };

            return vm; 
        }
    }
}