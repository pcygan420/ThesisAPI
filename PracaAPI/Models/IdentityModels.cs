using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PracaAPI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(64)]
        public string FirstName { get; set; }
        [MaxLength(256)]
        public string LastName { get; set; }       
        public string Sex { get; set; }
        [MaxLength(512)]
        public string About { get; set; }
        public string JoinDate { get; set; }
        public string BirthDate { get; set; }
        public int Points { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        
        /* entity */
        public DbSet<Entity.Entity> Entity { get; set; }
        public DbSet<Entity.Comment> Comments { get; set; }
        public DbSet<Entity.Favourite> Favourites { get; set; }
        public DbSet<Entity.Rate> Rates { get; set; }
        public DbSet<Entity.Reply> Replies { get; set; }

        /* data */
        public DbSet<Song.Song> Songs { get; set; }
        public DbSet<Song.Action> Actions { get; set; }
        public DbSet<Song.Expectant> Expectants { get; set; }

        /* users added data */
        public DbSet<Added.AddedSong> AddedSongs { get; set; }
        public DbSet<Text.SongText> SongTexts { get; set; }
        public DbSet<Translations.SongTranslation> SongTranslations { get; set; }
        public DbSet<Clips.SongClipUrl> SongClips { get; set; }
        public DbSet<Metric.SongMetric> SongMetrics { get; set; }

        public ApplicationDbContext()
            : base("MyDbConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //change tables name 
            modelBuilder.Entity<ApplicationUser>().ToTable("User");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<IdentityRole>().ToTable("Role");


            //delete phone fields from database
            modelBuilder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumber)
                                                  .Ignore(u => u.PhoneNumberConfirmed);

            //change columns name
            modelBuilder.Entity<ApplicationUser>().Property(p => p.Id).HasColumnName("UserId");
        }
        
    }
}