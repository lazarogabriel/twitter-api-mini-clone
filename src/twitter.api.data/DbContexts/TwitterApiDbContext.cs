using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using twitter.api.data.EntityConfigurations;
using twitter.api.domain.Models;

namespace twitter.api.data.DbContexts
{
    public class TwitterApiDbContext : DbContext, ITwitterApiDbContext
    {
        #region Constructor

        public TwitterApiDbContext(DbContextOptions<TwitterApiDbContext> options) : base(options)
        {
        }

        #endregion

        #region Props

        public DbSet<Tweet> Tweets { get ; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<FollowRelationship> FollowRelationships { get ; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
               .ApplyConfiguration(new UserEntityTypeConfig())
               .ApplyConfiguration(new FollowRelationshipEntityTypeConfig())
               .ApplyConfiguration(new TweetEntityTypeConfig());
        }

        #region Public Methods

        /// <inheritdoc/>
        public async Task CommitAsync() => await SaveChangesAsync();

        /// <inheritdoc/>
        public async Task TryCommitAsync(Action action)
        {
            var commited = false;
            var attempts = 0;
            while (!commited && attempts <= 3)
            {
                try
                {
                    // Attempt to commit changes to the database
                    attempts++;
                    await CommitAsync();
                    commited = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    action();
                }
            }
        }

        #endregion
    }
}
