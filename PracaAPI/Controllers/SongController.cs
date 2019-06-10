using PracaAPI.Models;
using PracaAPI.Models.Added;
using PracaAPI.Models.Entity;
using PracaAPI.Models.Song;
using PracaAPI.ViewModels;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using PracaAPI.Models.Enums;
using Microsoft.AspNet.Identity.EntityFramework;
using PracaAPI.ViewModels.Texts;
using System.Data.Entity;
using System.Collections.Generic;
using PracaAPI.ViewModels.Metrics;
using PracaAPI.Models.Metric;
using PracaAPI.Models.Text;
using PracaAPI.Models.Translations;
using PracaAPI.Models.Clips;
using PracaAPI.ViewModels.Entity;
using PracaAPI.ViewModels.Song.Search;

namespace PracaAPI.Controllers
{
    [RoutePrefix("Song")]
    [EnableCors(origins: "https://zen-allen-12.netlify.com/", headers: "*", methods: "*")]
    public class SongController : ApiController
    {
        #region Private fields

        private ApplicationDbContext _context;
        private ApplicationUserManager _userManager;

        #endregion

        #region Constructor

        public SongController()
        {
            _context = new ApplicationDbContext();
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        #endregion

        #region Add,Approve,Delete song

        [Authorize]
        [HttpPost]
        [Route("AddSong")]
        public IHttpActionResult AddSong([FromBody] SongViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            /* check whether song with given Title and Performer already exists*/
            if (_context.Songs.FirstOrDefault(obj => (obj.Title == viewModel.Title && obj.Performer == viewModel.Performer)) != null)
                return BadRequest("Piosenka o danym tytule i danego wykonawcy jest już w serwisie.");

            /* user associated with THIS request */
            var userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

            /* add song to temporary table */
            AddedSong song = SongViewModel.MapToObject(viewModel, userId);

            _context.AddedSongs.Add(song);
            _context.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("ApproveSong/{id}")]
        public IHttpActionResult Approve(int id)
        {
            /* get approved song*/
            var added_song = _context.AddedSongs.FirstOrDefault(obj => obj.AddedSongId == id);

            /* add points to user */
            var user = _userManager.FindByIdAsync(added_song.UserId).Result;
            user.Points += 10;

            var song = new Song()
            {
                Title = added_song.Title,
                Text = added_song.Text,
                Album = added_song.Album,
                Performer = added_song.Performer,
                Duration = added_song.Duration,
                PublicationDate = added_song.PublicationDate,
                Actions = new List<Models.Song.Action>
                {
                    new Models.Song.Action {
                        UserId = added_song.UserId,
                        Status = Actions.AddText,
                        Date = added_song.AddDate
                    }
                },
                Comments = new List<Comment>(),
                Rates = new List<Rate>(),
                Favourites = new List<Favourite>(),
                Tags = new List<Tag>()
        };


            /* add new song */
            var result = _context.Songs.Add(song);

            _context.AddedSongs.Remove(added_song);

            /* save */
            _userManager.UpdateAsync(user);
            _context.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("DeleteSong/{id}")]
        public IHttpActionResult Delete(int id) {

            /* find song and delete */
            var song = _context.AddedSongs.FirstOrDefault(obj => obj.AddedSongId == id);
            _context.AddedSongs.Remove(song);

            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region Add Text,Translation,Metric,Clip

        [Authorize]
        [HttpPost]
        [Route("AddText")]
        public IHttpActionResult AddText([FromBody] TextViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            /* user associated with THIS request */
            var userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

            /* add song to temporary table */
            _context.SongTexts.Add(new SongText {
                                        UserId = userId,
                                        EntityId = vm.EntityId,
                                        Title = vm.Title,
                                        Performer = vm.Performer,
                                        Text = vm.Text,
                                        AddDate = DateTime.Now
                                    });

            /* save */
            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("AddTran")]
        public IHttpActionResult AddTran([FromBody] TextViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            /* user associated with THIS request */
            var userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

            /* add song translation to temporary table */

            _context.SongTranslations.Add(new SongTranslation {
                                            UserId = userId,
                                            EntityId = vm.EntityId,
                                            Title = vm.Title,
                                            Performer = vm.Performer,
                                            Translation = vm.Text,
                                            AddDate = DateTime.Now
                                        });

            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("AddMetric")]
        public IHttpActionResult AddMetric([FromBody] MetricViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            /* user associated with THIS request */
            var userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

            /* add song to temporary table */
            SongMetric metric = MetricViewModel.MapToMetric(viewModel, userId);
            _context.SongMetrics.Add(metric);

            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("AddClip")]
        public IHttpActionResult AddClip([FromBody] TextViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            /* user associated with THIS request */
            var userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

            /* add song to temporary table */
            _context.SongClips.Add(new SongClipUrl
            {
                UserId = userId,
                EntityId = vm.EntityId,
                Title = vm.Title,
                Performer = vm.Performer,
                ClipUrl = vm.ClipUrl,
                AddDate = DateTime.Now
            });

            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region DoesExist, Search

        [Authorize]
        [Route("Check")]
        public IHttpActionResult DoesExist([FromBody] TitleViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(_context.Songs.FirstOrDefault(obj => obj.Title.ToLower() == viewModel.Title.ToLower() && obj.Performer.ToLower() == viewModel.Performer.ToLower()) == null);
        }

        [HttpGet]
        [Route("Search/{search}/{page}")]
        public IHttpActionResult SearchSong(string search,int page) {

            var userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

            return Ok(_context.Songs.Where(obj => obj.Title.Contains(search))
                                    .OrderBy(obj => obj.EntityId)
                                    .Skip(10 * (page - 1))
                                    .Take(10)
                                    .Include(obj => obj.Tags)
                                    .AsEnumerable()
                                    .Select(obj => SearchResultViewModel.MapToViewModel2(obj)));
        }

        [HttpGet]
        [Route("Search/{search}")]
        public IHttpActionResult SearchSong(string search)
        {

            return Ok(_context.Songs.Where(obj => obj.Title.Contains(search))
                                    .OrderBy(obj => obj.EntityId)
                                    .Take(6)
                                    .Include(obj => obj.Tags)
                                    .AsEnumerable()
                                    .Select(obj => SearchBoxViewModel.MapToViewModel(obj)));
        }

        #endregion

        #region Get song/songs

        [Authorize]
        [HttpGet]
        [Route("GetSong/{id}")]
        public IHttpActionResult GetSongs(int id) {

            return Ok(_context.Songs.Where(obj => obj.EntityId == id)
                                                 .Include(obj => obj.Actions)
                                                 .AsEnumerable()
                                                 .Select(obj => SongViewModel.MapToViewModel(obj))
                                                 .FirstOrDefault());
        }

        [Authorize]
        [HttpGet]
        [Route("GetSongs/{page}/{number}")]
        public IHttpActionResult GetSongsList(int page, int number)
        {
            return Ok(new {
                          Songs = _context.Songs.OrderBy(obj => obj.EntityId)
                                                .Skip(number * (page - 1))
                                                .Take(number)
                                                .Include(obj => obj.Actions)
                                                .Include(obj => obj.Actions.Select(act => act.User))
                                                .AsEnumerable()
                                                .Select(obj => SongViewModel.MapToViewModel(obj)),
                          Total = _context.Songs.Count()
                      });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAddedSong/{id}")]
        public IHttpActionResult Get(int id) {

            return Ok(_context.AddedSongs.Where(obj => obj.AddedSongId == id)
                                                           .Include(obj => obj.User)
                                                           .AsEnumerable()
                                                           .Select(obj => ViewModels.Admin.AddedSongViewModel.MapToViewModel(obj))
                                                           .FirstOrDefault());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAddedSongs/{page}/{number}")]
        public IHttpActionResult GetList(int page, int number) {

            return Ok(new {
                          Songs = _context.AddedSongs.OrderBy(obj => obj.AddedSongId)
                                                      .Skip(number * (page - 1))
                                                      .Take(number)
                                                      .Include(obj => obj.User)
                                                      .AsEnumerable()
                                                      .Select(obj => ViewModels.Admin.AddedSongViewModel.MapToViewModel(obj)),
                          Total = _context.AddedSongs.Count()
                      });
        }

        [HttpGet]
        [Route("GetFullSong/{id}")]
        public IHttpActionResult GetSong(int id) {

            string current = null;
            Rate rate = null;
            Favourite favour = null;

            if (User.Identity.Name != null)
            {
                current = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;
                rate = _context.Rates.FirstOrDefault(obj => obj.UserId == current && obj.EntityId == id);
                favour = _context.Favourites.FirstOrDefault(obj => obj.UserId == current && obj.EntityId == id);
            }
            
            var song = _context.Songs.Where(obj => obj.EntityId == id)
                                     .Include(obj => obj.Tags)
                                     .Include(obj => obj.Comments)
                                     .Include(obj => obj.Comments.Select(com => com.Replies))
                                     .Include(obj => obj.Actions)
                                     .Include(obj => obj.Rates)
                                     .Include(obj => obj.Actions.Select(act => act.User))
                                     .Include(obj => obj.Expectants)
                                     .AsEnumerable()
                                     .Select(obj => SongFullViewModel.MapToViewModel(obj, current, rate, favour))
                                     .FirstOrDefault();

            return Ok(song);
        }

        [HttpGet]
        [Route("GetViewModel/{id}")]
        public IHttpActionResult GetViewModel(int id) {

            return Ok(_context.Songs.Where(obj => obj.EntityId == id)
                                     .AsEnumerable()
                                     .Select(obj => TextViewModel.MapToViewModel(obj)));
        }

        [HttpGet]
        [Route("GetSongs/{page}/{sort?}/{year?}/{genre?}/{hidden?}")]
        public IHttpActionResult GetSearchedSongs(int page, string sort = "", string year = "", string genre = "", string hidden = "") {

            IQueryable<Song> songs = _context.Songs.Include(obj => obj.Tags)
                                                   .Include(obj => obj.Rates);

            
            string userId = "";

            if (User.Identity.IsAuthenticated)
                userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

            if (year != "")
            {
                string[] years = year.Split(',');
                songs = songs.Where(song => years.Contains(song.PublicationDate));
            }

            if (genre != "") {

                string[] genres = genre.Split(',');
                
                songs = songs.Where(song => genres.All(ge => song.Tags.Any(tag => tag.Name == ge)) );
            }

            if(hidden != "")
            {
                if(User.Identity.IsAuthenticated)
                    songs = songs.Where(song => !song.Rates.Any(rate => rate.UserId == userId));
            }

            switch(sort) {
                
                case "rate-ascending":
                    songs = songs.OrderBy(song => song.Rates.Average(rate => rate.Rating));
                    break;
                case "rate-descending":
                    songs = songs.OrderByDescending(song => song.Rates.Average(rate => rate.Rating));
                    break;
                case "votes-ascending":
                    songs = songs.OrderBy(song => song.Rates.Count());
                    break;
                case "votes-descending":
                    songs = songs.OrderByDescending(song => song.Rates.Count());
                    break;
                case "oldest":
                    songs = songs.OrderBy(song => song.EntityId);
                    break;
                case "newest":
                    songs = songs.OrderByDescending(song => song.EntityId);
                    break;

                default:
                    break;
            }

            return Ok(new {
                          Results = songs.Skip(10 * (page - 1))
                                         .Take(10)
                                         .AsEnumerable()
                                         .Select(obj => SearchResultViewModel.MapToViewModel(obj,userId)),
                          Total = songs.Count()
                      });
        }

        [HttpGet]
        [Route("GetMetric/{id}")]
        public IHttpActionResult GetMetric(int id)
        {

            var metric = _context.Songs.Where(obj => obj.EntityId == id)
                                       .Include(obj => obj.Tags)
                                       .AsEnumerable()
                                       .Select(obj => MetricViewModel.MapToViewModel2(obj))
                                       .FirstOrDefault();

            return Ok(metric);
        }

        #endregion

        #region Get recent data
        [HttpGet]
        [Route("GetRecent")]
        public IHttpActionResult GetRecentSongs() {
            
            var song = _context.Songs.OrderByDescending(obj => obj.EntityId)
                                     .Include(obj => obj.Actions)
                                     .Include(obj => obj.Actions.Select(act => act.User))
                                     .Take(12)
                                     .AsEnumerable()
                                     .Select(obj => new TextViewModel {
                                         Id = obj.EntityId,
                                         Title = obj.Title,
                                         Performer = obj.Performer,
                                         Text = string.Join( "\n", obj.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Take(6) ),
                                         User = obj.Actions.FirstOrDefault(act => act.Status == Actions.AddText).User.UserName,
                                         AddDate = ConvertDateToTimeLast(obj.Actions.FirstOrDefault(act => act.Status == Actions.AddText).Date)
                                     });

            return Ok(song);
        }

        public string ConvertDateToTimeLast(DateTime date) {

            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.UtcNow.AddHours(1).Ticks - date.Ticks);

            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
            {
                if (ts.Seconds == 1)
                    return "sekunde temu";
                else if (ts.Seconds > 1 && ts.Seconds < 5)
                    return ts.Seconds + " sekundy temu";
                else
                    return ts.Seconds + " sekund temu";
            }

            if (delta < 2 * MINUTE)
                return "minute temu";

            if (delta < 45 * MINUTE)
            {
                if (ts.Minutes > 2 * MINUTE && ts.Minutes < 5 * MINUTE)
                    return ts.Minutes + " minuty temu";
                else
                    return ts.Minutes + " minut temu";
            }

            if (delta < 90 * MINUTE)
                return "godzine temu";

            if (delta < 24 * HOUR)
            {
                if (ts.Hours > 2 * HOUR && ts.Hours < 5 * HOUR)
                    return ts.Hours + " godziny temu";
                else
                    return ts.Hours + " godzin temu";
            }

            if (delta < 48 * HOUR)
                return "wczoraj";

            if (delta < 30 * DAY)
                return ts.Days + " dni temu";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                if (months <= 1)
                    return "jeden miesiąc temu";
                else if (months > 2 && months < 5)
                    return months + " miesiące temu";
                else
                    return months + " miesięcy temu";
            }

            else {
                return "rok temu";
            }
        }

        [HttpGet]
        [Route("GetExpectants")]
        public IHttpActionResult GetExpectants() {

            return Ok(_context.Songs.Where(song => song.Expectants.Count() > 0)
                                       .OrderByDescending(song => song.Expectants.Count())
                                       .Include(song => song.Expectants)
                                       .AsEnumerable()
                                       .Select(song => ExpectantViewModel.MapToViewModel(song))
                                       .Take(12));
        }

        [HttpGet]
        [Route("GetClips")]
        public IHttpActionResult GetClips() {

            return Ok(_context.Actions.OrderByDescending(act => act.ActionId)
                                      .Where(act => act.Status == Actions.AddUrl)
                                      .Include(act => act.Song)
                                      .Take(8)
                                      .Select(act => new {
                                          Url = act.Song.ClipUrl.Replace("watch?v=", "embed/"),
                                          Title = act.Song.Title,
                                          Performer = act.Song.Performer
                                      }));
        }

        #endregion

        #region Add comment, rate, heart

        [Authorize]
        [HttpPost]
        [Route("AddExpectant/{songId}")]
        public IHttpActionResult AddExpectant(int songId) {

            var userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

            _context.Songs.FirstOrDefault(song => song.EntityId == songId).Expectants.Add(new Expectant
            {
                UserId = userId
            });

            _context.SaveChanges();

            return Ok();
        }
  
        [Authorize]
        [HttpPost]
        [Route("AddComment")]
        public IHttpActionResult AddComment([FromBody] CommentViewModel viewModel) {

            if(!ModelState.IsValid)
                return BadRequest();

            /* get user and entity */
            var userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;
            var entity = _context.Entity.FirstOrDefault(ent => ent.EntityId == viewModel.EntityId);
   
            /* add comment and save */
            entity.Comments.Add(CommentViewModel.MapToObject(viewModel, userId));
            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("AddReply")]
        public IHttpActionResult AddReply([FromBody] ReplyViewModel viewModel) {

            if (!ModelState.IsValid)
                return BadRequest();

            /* get user and entity */
            var userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;
            var comment = _context.Comments.FirstOrDefault(com => com.CommentId == viewModel.ParentId);

            /* add reply and save */
            comment.Replies.Add(new Reply
            {
                UserId = userId,
                AddDate = DateTime.Now,
                Text = viewModel.Comment
            });
      
            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("AddRate")]
        public IHttpActionResult AddRate([FromBody] RateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            /* get user and current rate (if exists) */
            var userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;
            var current_rate = _context.Rates.FirstOrDefault(obj => obj.EntityId == viewModel.EntityId && obj.UserId == userId);

            /* update if exists */
            if (current_rate != null) {
                current_rate.Rating = viewModel.Rate;
                _context.SaveChanges();
                return Ok();
            }

            /* add new rate */
            var entity = _context.Entity.FirstOrDefault(ent => ent.EntityId == viewModel.EntityId);
            entity.Rates.Add(RateViewModel.MapToObject(viewModel,userId));

            /* save */
            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("AddHeart/{entityId}")]
        public IHttpActionResult AddHeart(int entityId)
        {
            /* user and entity */
            var userId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;
            var entity = _context.Entity.FirstOrDefault(ent => ent.EntityId == entityId);

            /* has user added heart already */
            var current_heart = _context.Favourites.FirstOrDefault(obj => obj.EntityId == entityId && obj.UserId == userId);

            /* heart exists => remove */
            if (current_heart != null)
            {
                _context.Favourites.Remove(current_heart);
                _context.SaveChanges();
                return Ok();
            }

            entity.Favourites.Add(new Favourite
            {
                EntityId = entity.EntityId,
                UserId = userId
            });

            _context.SaveChanges();

            return Ok();
        }

        #endregion
    }
}
