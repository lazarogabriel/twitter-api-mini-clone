using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using twitter.api.application.Models.Follow;
using twitter.api.application.Services.Abstractions;
using twitter.api.data.DbContexts;
using twitter.api.domain.Constants;
using twitter.api.domain.Exceptions;
using twitter.api.domain.Models;

namespace twitter.api.application.Services
{
    public class FollowService : IFollowService
    {
        #region Fields

        private readonly ITwitterApiDbContext _dbContext;

        #endregion

        #region Constructor

        public FollowService(ITwitterApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public async Task<FollowRelationship> FollowUser(CreateFollowerCommand command)
        {
            if (command.FollowerId == command.UserToFollowId)
            {
                throw new ValidationException(Errors.CannotFollowYourself);
            }

            var follower = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == command.FollowerId);

            if (follower is null)
            {
                throw new NotFoundException(Errors.FollowerUserNotFound);
            }
            
            var userToFollow = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == command.UserToFollowId);

            if (userToFollow is null)
            {
                throw new NotFoundException(Errors.UserToFollowNotFound);
            }

            var alreadyFollowing = await _dbContext.FollowRelationships.AnyAsync(r => 
                r.FollowerId == command.FollowerId && 
                r.FollowedId == command.UserToFollowId);

            if (alreadyFollowing)
            {
                throw new ValidationException(Errors.AlreadyFollowingUser);
            }

            var followRelationship = follower.Follow(userToFollow);

            await _dbContext.FollowRelationships.AddAsync(followRelationship);

            await _dbContext.CommitAsync();

            return followRelationship;
        }

        /// <inheritdoc/>
        public async Task UnfollowUser(DeleteFollowerCommand command)
        {
            var followRelationship = await _dbContext.FollowRelationships
                .Include(f => f.Followed)
                .Include(f => f.Follower)
                .FirstOrDefaultAsync(r =>
                    r.FollowerId == command.UnfollowerId &&
                    r.FollowedId == command.UserToUnfollowId);

            if (followRelationship is null)
            {
                throw new NotFoundException(Errors.FollowRelationshipNotFound);
            }

            var unfollower = followRelationship.Follower;
            var userToUnfollow = followRelationship.Followed;

            unfollower.Unfollow(userToUnfollow, followRelationship);

            await _dbContext.CommitAsync();
        }

        #endregion
    }
}
