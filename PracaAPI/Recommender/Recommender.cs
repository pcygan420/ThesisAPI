using PracaAPI.Models;
using PracaAPI.Models.Entity;
using PracaAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace PracaAPI.Recommender
{
    public class Recommender
    {
        #region Private fields

        private static ApplicationDbContext _context;

        #endregion

        #region Constructor

        public Recommender()
        {
            _context = new ApplicationDbContext();
        }

        #endregion

        #region Find items

        public IEnumerable<RecommendViewModel> FindItems(List<ApplicationUser> users, List<Rate> current_rates, string userId) {

            /* items with ratings in the DataTable form */
            var items = FindClosestNeighbors(users, current_rates, userId);

            /* result list */
            List<RecommendViewModel> result = new List<RecommendViewModel>();

            /* helper variables */
            double k = 0;
            double counter = 0;

            /* calculate predictions for items */
            /* each column is an item with it's ratings (user rates are stored in a single row) */
            foreach (DataColumn col in items.Columns)
            {
                /* skip last column */
                if (col.ColumnName == "similarity")
                    continue;

                foreach (DataRow row in items.Rows)
                {
                    /* if user didnt rate an item => skip */
                    if (string.IsNullOrEmpty(row[col].ToString()))
                        continue;

                    counter += double.Parse(row[col].ToString()) * double.Parse(row["similarity"].ToString());
                    k += double.Parse(row["similarity"].ToString());
                }

                double predicted_rate = Math.Ceiling(1 / k * counter * 100) / 100;

                int entityId = int.Parse(col.ColumnName);

                /* find a song by EntityId, then project it to view model (with predicted rate and other extra information) */
                result.Add(_context.Songs.Where(song => song.EntityId == entityId)
                                         .Include(song => song.Tags)
                                         .Include(song => song.Rates)
                                         .AsEnumerable()
                                         .Select(song => RecommendViewModel.MapToViewModel(song, predicted_rate))
                                         .FirstOrDefault());

                k = 0;
                counter = 0;
            }

            return result;
        }

        #endregion

        #region Find closest neighbors

        public DataTable FindClosestNeighbors(List<ApplicationUser> users, List<Rate> current_rates, string userId) {

            /*  rates (of similar user) - similarity */
            Dictionary<IQueryable<Rate>, double> closestNeighbours = new Dictionary<IQueryable<Rate>, double>();

            foreach (var user in users)
            {
                /* skip current user */
                if (user.Id == userId)
                    continue;

                /* get user rates */
                var rates = _context.Rates.Where(rate => rate.UserId == user.Id)
                                          .Include(rate => rate.Entity)
                                          .Include(rate => rate.Entity.Tags)
                                          .OrderBy(rate => rate.EntityId)
                                          .ToList()
                                          .AsQueryable();

                /* if user rated less than 5 items => skip */
                if (rates.Count() < 5)
                    continue;

                /* if there arent common rates => skip */
                if (!rates.Any(rate => current_rates.Any(r => r.EntityId == rate.EntityId)))
                    continue;

                /* get common elements ratings - items that current user and another user have rated */
                var current_commonRates = current_rates.Where(rate => rates.Any(r => rate.EntityId == r.EntityId));
                var rates_common = rates.Where(rate => current_commonRates.Any(r => r.EntityId == rate.EntityId));

                var similarity = CalculateSimilarity(current_commonRates, rates_common);

                /* add found user to dictionary */
                closestNeighbours.Add(rates.Except(rates_common), similarity);
            }

            /* sort the dict and take 5 most similar users */
            var sortedDict = (from entry in closestNeighbours orderby entry.Value descending select entry).Take(5);


            DataTable dt = new DataTable();
            /* rows = users ; columns = items */
            dt.Columns.Add("similarity");
            


            foreach (var user in sortedDict) {

                /* create new user row */
                DataRow _ravi = dt.NewRow();

                /* add items that logged in user didnt rate */
                foreach (var rate in user.Key)
                {
                    /* add column if an item is new */
                    if (!dt.Columns.Contains(rate.EntityId.ToString()))
                        dt.Columns.Add(rate.EntityId.ToString());

                    /* add rating */
                    _ravi[rate.EntityId.ToString()] = rate.Rating;
                }

                /* store similarity */
                _ravi["similarity"] = user.Value;

                /* add row */
                dt.Rows.Add(_ravi);
            }

            return dt;
        }

        #endregion

        #region Calculate similarity

        /* method calculates the similarity between logged in user and found user */
        /* rates1 - rates of logged in user   rates2 - rates of found user */
        public static double CalculateSimilarity(IEnumerable<Rate> rates1, IEnumerable<Rate> rates2) {

            var avg1 = rates1.Average(rate => rate.Rating);
            var avg2 = rates2.Average(rate => rate.Rating);

            double counter = 0;
            double nomi1 = 0;
            double nomi2 = 0;

            for (var i = 0; i < rates2.Count(); i++)
            {
                counter += (rates1.ElementAt(i).Rating - avg1) * (rates2.ElementAt(i).Rating - avg2);
                nomi1 += Math.Pow((rates1.ElementAt(i).Rating - avg1), 2);
                nomi2 += Math.Pow((rates2.ElementAt(i).Rating - avg2), 2);
            }
            
            if(nomi1 * nomi2 == 0)
                return 0;

            return counter / Math.Sqrt(nomi1 * nomi2);
        }

        #endregion
    }
}