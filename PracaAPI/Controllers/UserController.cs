using Microsoft.AspNet.Identity.EntityFramework;
using PracaAPI.Models;
using PracaAPI.Models.Enums;
using PracaAPI.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using PracaAPI.ViewModels.User;

namespace PracaAPI.Controllers
{
    [RoutePrefix("User")]
    [EnableCors(origins: "https://zen-allen-12.netlify.com/", headers: "*", methods: "*")]
    public class UserController : ApiController
    {

        #region Private fields

        private ApplicationDbContext _context;
        private ApplicationUserManager _userManager;

        #endregion

        #region Constructor

        public UserController()
        {
            _context = new ApplicationDbContext();
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        #endregion

        #region Get profil data

        [HttpGet]
        [Route("GetProfil")]
        public IHttpActionResult GetProfil()
        {

            ApplicationUser current = _userManager.FindByNameAsync(User.Identity.Name).Result;

            var favTotal = 0;
            var ratesTotal = 0;
            var songsTotal = 0;
            var transTotal = 0;
            var clipsTotal = 0;

            foreach (var action in _context.Actions)
            {
                if(action.UserId == current.Id)
                {
                    if (action.Status == Actions.AddText)
                        songsTotal++;

                    if (action.Status == Actions.AddTranslation)
                        transTotal++;

                    if (action.Status == Actions.AddUrl)
                        clipsTotal++;
                }
            }

            foreach (var fav in _context.Favourites)
            {
                if (fav.UserId == current.Id)
                    favTotal++;
            }

            foreach (var rate in _context.Rates)
            {
                if (rate.UserId == current.Id)
                    ratesTotal++;
            }

            var result = _context.Users.OrderByDescending(user => user.Points)
                                       .AsEnumerable()
                                       .Select((entry, index) => new {
                                           Index = index,
                                           User = entry.Id
                                       }).FirstOrDefault(res => res.User == current.Id);



            return Ok( ProfilViewModel.MapToViewModel(current,ratesTotal,favTotal,songsTotal,transTotal,clipsTotal,current.Points,result.Index + 1) );
        }

        [HttpGet]
        [Route("Get/{page}/{number}")]
        public IHttpActionResult GetAdded(int page, int number) {

            string userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

            var action = Actions.AddText;

            switch (number)
            {
                case 1:
                    action = Actions.AddTranslation;
                    break;
                case 2:
                    action = Actions.AddUrl;
                    break;

                default:
                    break;
            }

            var total = _context.Actions.Where(act => act.Status == action && act.UserId == userId).Count();

            var result = _context.Actions.Where(act => act.Status == action && act.UserId == userId)
                                         .OrderBy(act => act.ActionId)
                                         .Skip(5 * (page - 1))
                                         .Take(5)
                                         .Include(act => act.Song)
                                         .Include(act => act.User)
                                         .AsEnumerable()
                                         .Select(act => new ActionViewModel
                                         {
                                             Id = act.Song.EntityId,
                                             Title = act.Song.Title,
                                             Performer = act.Song.Performer,
                                             User = act.User.UserName,
                                             AddDate = act.Date.ToString("MM/dd/yyyy H:mm"),
                                             Status = act.Status

                                         });

            var returnObj = new { Actions = result, Total = total };

            return Ok(returnObj);
        }

        [HttpGet]
        [Route("GetRated/{page}/{number}")]
        public IHttpActionResult GetRated(int page, int number)
        {

            string userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

            var rated = _context.Rates.Where(obj => obj.UserId == userId)
                                      .OrderBy(obj => obj.Rating)
                                      .Skip(5 * (page - 1))
                                      .Take(5)
                                      .Include(obj => obj.Entity)
                                      .Include(obj => obj.Entity.Tags)
                                      .AsEnumerable()
                                      .Select(obj => RateViewModel.MapToViewModel(obj));

            var total = _context.Rates.Where(obj => obj.UserId == userId).Count();

            return Ok(new { Rates = rated, Total = total });
        }

        [HttpGet]
        [Route("GetFavourite/{page}/{number}")]
        public IHttpActionResult GetFavourite(int page, int number)
        {

            string userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

            var rated = _context.Favourites.Where(obj => obj.UserId == userId)
                                           .OrderBy(obj => obj.FavouriteId)
                                           .Skip(5 * (page - 1))
                                           .Take(5)
                                           .Include(obj => obj.Entity)
                                           .Include(obj => obj.Entity.Tags)
                                           .Include(obj => obj.Entity.Rates)
                                           .AsEnumerable()
                                           .Select(obj => RateViewModel.MapToViewModel2(obj,userId));

            var total = _context.Favourites.Where(obj => obj.UserId == userId).Count();

            return Ok(new { Rates = rated, Total = total });
        }

        #endregion

        #region Get users

        [HttpGet]
        [Route("GetUsers/{page}")]
        public IHttpActionResult GetUsers(int page) {

            return Ok( new {
                        Users = _context.Users.OrderByDescending(user => user.Points)
                                              .Skip(20 * (page - 1))
                                              .Take(20)
                                              .AsEnumerable()
                                              .Select((user, index) => RankViewModel.MapToViewModel(user, index+1)),
                        Total = _context.Users.Count()
                    });

        }

        #endregion
    }
}
