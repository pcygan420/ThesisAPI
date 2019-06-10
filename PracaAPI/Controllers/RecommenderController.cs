using Microsoft.AspNet.Identity.EntityFramework;
using PracaAPI.Models;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PracaAPI.Controllers
{
    [RoutePrefix("Recommender")]
    [EnableCors(origins: "https://zen-allen-12.netlify.com/", headers: "*", methods: "*")]
    public class RecommenderController : ApiController
    {

        #region Private Fields

        private ApplicationDbContext _context;
        private ApplicationUserManager _userManager;

        #endregion

        #region Constructor

        public RecommenderController()
        {
            _context = new ApplicationDbContext();
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        #endregion

        #region Recommender

        [Authorize]
        [HttpGet]
        [Route("Get/{page}")]
        public IHttpActionResult Get(int page)
        {

            /* current user Id */
            var userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

            /* current user rates */
            var current_rates = _context.Rates.OrderBy(rate => rate.EntityId)
                                              .Where(rate => rate.UserId == userId)
                                              .ToList();

            /* return if user has less then 5 rates */
            if (current_rates.Count() < 5)
                return Ok( new { Message = "Oceń co najmniej 5 piosenek by korzystać z rekomendera." } );

            /* recommender */
            var recommender = new Recommender.Recommender();
            var items = recommender.FindItems(_context.Users.ToList(), current_rates, userId);

            /* if recommended items length is 0 */
            if (items.Count() == 0)
                return Ok( new { Message = "Brak rekomendowanych piosenek dla Ciebie :/" } );

            return Ok(items);
        }

        #endregion
    }
}
