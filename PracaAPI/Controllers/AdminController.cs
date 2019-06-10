using EntityFramework.Extensions;
using Microsoft.AspNet.Identity.EntityFramework;
using PracaAPI.Models;
using PracaAPI.Models.Entity;
using PracaAPI.Models.Enums;
using PracaAPI.ViewModels.Admin;
using PracaAPI.ViewModels.Metrics;
using PracaAPI.ViewModels.Texts;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PracaAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("Admin")]
    [EnableCors(origins: "https://zen-allen-12.netlify.com/", headers: "*", methods: "*")]
    public class AdminController : ApiController
    {
        #region Private Fields

        private ApplicationDbContext _context;
        private ApplicationUserManager _userManager;

        #endregion

        #region Constructor

        public AdminController()
        {
            _context = new ApplicationDbContext();
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        #endregion

        #region Home data

        [HttpGet]
        [Route("Global")]
        public IHttpActionResult GetGlobal()
        {

            var result = new
            {
                Songs = _context.AddedSongs.OrderBy(obj => obj.AddedSongId)
                                           .Take(20)
                                           .Include(obj => obj.User)
                                           .AsEnumerable()
                                           .Select(obj => AddedSongViewModel.MapToViewModel(obj)),
                TotalSongs = _context.AddedSongs.Count(),
                TotalTrans = _context.SongTranslations.Count(),
                TotalClips = _context.SongClips.Count(),
                TotalMetrics= _context.SongMetrics.Count()
            };

            return Ok(result);
        }

        #endregion

        #region Get,Edit Song
        
        [HttpGet]
        [Route("Get/{id}")]
        public IHttpActionResult Get(int id) {

            var result = _context.Songs.Where(song => song.EntityId == id)
                                       .Include(song => song.Tags)
                                       .FirstOrDefault();

            return Ok(SongViewModel.MapToViewModel(result));
        }
        
        [HttpPost]
        [Route("Edit/{id}")]
        public IHttpActionResult Edit(SongViewModel vm, int id) {

            if (!ModelState.IsValid)
                return BadRequest();

            var song = _context.Songs.Where(s => s.EntityId == id)
                                     .Include(s => s.Tags)
                                     .FirstOrDefault();

            song.Title = vm.Title;
            song.Performer = vm.Performer;
            song.Text = vm.Text;
            song.Translation = vm.Translation;
            song.ClipUrl = vm.ClipUrl;

            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region Get,Approve,Delete text

        [HttpGet]
        [Route("GetText/{id}")]
        public IHttpActionResult GetText(int id) { 

            return Ok(_context.SongTexts.Where(obj => obj.SongTextId == id)
                                        .Include(obj => obj.User)
                                        .AsEnumerable()
                                        .Select(obj => TextViewModel.MapTextToViewModel(obj))
                                        .FirstOrDefault());
        }

        [HttpGet]
        [Route("GetTexts/{page}/{number}")]
        public IHttpActionResult GetTexts(int page, int number)
        {
            return Ok(new {
                Texts = _context.SongTexts.OrderBy(obj => obj.SongTextId)
                                          .Skip(number * (page - 1))
                                          .Take(number)
                                          .Include(obj => obj.User)
                                          .AsEnumerable()
                                          .Select(obj => TextViewModel.MapTextToViewModel(obj)),
                Total = _context.SongTexts.Count()
            });
        }
        
        [HttpPost]
        [Route("ApproveText/{id}")]
        public IHttpActionResult ApproveText(int id)
        {
            /* get approved text and song to update */
            var text = _context.SongTexts.FirstOrDefault(obj => obj.SongTextId == id);
            var song = _context.Songs.FirstOrDefault(obj => obj.EntityId == text.EntityId);

            /* add points to user */
            var user = _userManager.FindByIdAsync(text.UserId).Result;
            user.Points += 1;

            /* update text and add actions */
            song.Text = text.Text;
            song.Actions.Add(new Models.Song.Action()
            {
                UserId = text.UserId,
                Date = text.AddDate,
                Status = Actions.UpdateText
            });

            /* remove text and save */
            _context.SongTexts.Remove(text);
            _userManager.UpdateAsync(user);
            _context.SaveChanges();

            return Ok();
        }
        
        [HttpDelete]
        [Route("DeleteText/{id}")]
        public IHttpActionResult DeleteText(int id)
        {
            /* find text and remove */
            var text = _context.SongTexts.FirstOrDefault(obj => obj.SongTextId == id);
            _context.SongTexts.Remove(text);

            /* save */
            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region Get,Approve,Delete translation

        [HttpGet]
        [Route("GetTran/{id}")]
        public IHttpActionResult GetTran(int id)
        {

            var result = _context.SongTranslations.Where(obj => obj.TranslationId == id)
                                                  .Include(obj => obj.User)
                                                  .AsEnumerable()
                                                  .Select(obj => TextViewModel.MapTranToViewModel(obj));

            return Ok(result);
        }

        [HttpGet]
        [Route("GetTrans/{page}/{number}")]
        public IHttpActionResult GetTrans(int page, int number)
        {
            return Ok(new {
                Texts = _context.SongTranslations.OrderBy(obj => obj.TranslationId)
                                                        .Skip(number * (page - 1))
                                                        .Take(number)
                                                        .Include(obj => obj.User)
                                                        .AsEnumerable()
                                                        .Select(obj => TextViewModel.MapTranToViewModel(obj)),

                Total = _context.SongTranslations.Count()
            });
        }
        
        [HttpPost]
        [Route("ApproveTran/{id}")]
        public IHttpActionResult ApproveTran(int id)
        {
            /* get approved translation and song to upadte */
            var tran = _context.SongTranslations.FirstOrDefault(obj => obj.TranslationId == id);
            var song = _context.Songs.FirstOrDefault(obj => obj.EntityId == tran.EntityId);

            /* add points to user */
            var user = _userManager.FindByIdAsync(tran.UserId).Result;

            if (song.Translation == null)
                user.Points += 5;
            else
                user.Points += 1;

            song.Actions.Add(new Models.Song.Action()
            {
                UserId = tran.UserId,
                Date = tran.AddDate,
                Status = (song.Translation == null ? Actions.AddTranslation : Actions.UpdateTranslation)
            });

            /* update text and add actions */
            song.Translation = tran.Translation;

            /* remove text and delete all expectants */
            _context.SongTranslations.Remove(tran);
            _context.Expectants.Where(exp => exp.SongId == song.EntityId).Delete();

            /* save */
            _userManager.UpdateAsync(user);
            _context.SaveChanges();

            return Ok();
        }
        
        [HttpDelete]
        [Route("DeleteTran/{id}")]
        public IHttpActionResult DeleteTran(int id)
        {
            /* find tran and remove */
            var tran = _context.SongTranslations.FirstOrDefault(obj => obj.TranslationId == id);
            _context.SongTranslations.Remove(tran);

            /* save */
            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region Get,Approve,Delete metric

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

        [HttpGet]
        [Route("GetAddedMetric/{id}")]
        public IHttpActionResult Get2(int id) {
            
            return Ok(_context.SongMetrics.Where(obj => obj.MetricId == id)
                                          .Include(obj => obj.User)
                                          .AsEnumerable()
                                          .Select(obj => MetricViewModel.MapToViewModel(obj)));
        }

        [HttpGet]
        [Route("GetMetrics/{page}/{number}")]
        public IHttpActionResult GetMetrics(int page, int number)
        {
            return Ok( new {
                Metrics = _context.SongMetrics.OrderBy(obj => obj.MetricId)
                                              .Skip(number * (page - 1))
                                              .Take(number)
                                              .Include(obj => obj.User)
                                              .AsEnumerable()
                                              .Select(obj => MetricViewModel.MapToViewModel(obj)),

                Total = _context.SongMetrics.Count()
            });
        }
        
        [HttpPost]
        [Route("ApproveMetric/{id}")]
        public IHttpActionResult ApproveMetric(int id)
        {
            /* get approved metric and song to update */
            var metric = _context.SongMetrics.FirstOrDefault(obj => obj.MetricId == id);
            var song = _context.Songs.FirstOrDefault(obj => obj.EntityId == metric.EntityId);

            /* add points to user */
            var user = _userManager.FindByIdAsync(metric.UserId).Result;
            user.Points += 3;

            /* update song and add action */
            song.Album = metric.Album;
            song.PublicationDate = metric.PublicationDate;
            song.Duration = metric.Duration;
            song.Curiosities = metric.Curosities;
            song.Tags = new List<Tag>();
            song.Curiosities = metric.Curosities;

            foreach (var tag in metric.Tags.Split(','))
            {
                song.Tags.Add(new Tag
                {
                    Name = tag
                });
            }

            song.Actions.Add(new Models.Song.Action()
            {
                UserId = metric.UserId,
                Date = metric.AddDate,
                Status = Actions.UpdateMetric
            });

            /* remove text and save */
            _context.SongMetrics.Remove(metric);
            _userManager.UpdateAsync(user);
            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete]
        [Route("DeleteMetric/{id}")]
        public IHttpActionResult DeleteMetric(int id) {

            /* find metric and remove */
            var metric = _context.SongMetrics.FirstOrDefault(obj => obj.MetricId == id);
            _context.SongMetrics.Remove(metric);

            /* save */
            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region Get,Approve,Delete clip

        [HttpGet]
        [Route("GetClip/{id}")]
        public IHttpActionResult GetClip(int id)
        {

            var clip = _context.Songs.Where(obj => obj.EntityId == id)
                                     .Select(obj => new TextViewModel
                                     {
                                         EntityId = obj.EntityId,
                                         Title = obj.Title,
                                         Performer = obj.Performer,
                                         ClipUrl = obj.ClipUrl
                                     });

            return Ok(clip);
        }

        [HttpGet]
        [Route("GetClips/{page}/{number}")]
        public IHttpActionResult GetClips(int page, int number)
        {
            return Ok(new {
                Texts = _context.SongClips.OrderBy(obj => obj.ClipId)
                                          .Skip(number * (page - 1))
                                          .Take(number)
                                          .Include(obj => obj.User)
                                          .AsEnumerable()
                                          .Select(obj => TextViewModel.MapClipToViewModel(obj)),
                Total = _context.SongClips.Count()
            });
        }
        
        [HttpPost]
        [Route("ApproveClip/{id}")]
        public IHttpActionResult ApproveClip(int id)
        {
            /* get approved clip and song to update */
            var clip = _context.SongClips.FirstOrDefault(obj => obj.ClipId == id);
            var song = _context.Songs.FirstOrDefault(obj => obj.EntityId == clip.EntityId);

            /* add points to user */
            var user = _userManager.FindByIdAsync(clip.UserId).Result;
            user.Points += 3;

            /* update song and add action */
            song.ClipUrl = clip.ClipUrl;

            song.Actions.Add(new Models.Song.Action()
            {
                UserId = clip.UserId,
                Date = clip.AddDate,
                Status = Actions.AddUrl
            });

            /* remove clip and save */
            _context.SongClips.Remove(clip);
            _userManager.UpdateAsync(user);
            _context.SaveChanges();

            return Ok();
        }
        
        [HttpDelete]
        [Route("DeleteClip/{id}")]
        public IHttpActionResult DeleteClip(int id) {

            /* find and remove clip */
            var clip = _context.SongClips.FirstOrDefault(obj => obj.ClipId == id);
            _context.SongClips.Remove(clip);

            /* save */
            _context.SaveChanges();

            return Ok();
        }

        #endregion  
    }
}
