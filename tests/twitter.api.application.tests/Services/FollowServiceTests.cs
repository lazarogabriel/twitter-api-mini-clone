using Microsoft.EntityFrameworkCore;
using twitter.api.application.Models.Follow;
using twitter.api.application.Services;
using twitter.api.data.DbContexts;
using twitter.api.domain.Constants;
using twitter.api.domain.Exceptions;
using twitter.api.domain.Models;

namespace twitter.api.application.tests.Services
{
    public class FollowServiceTests
    {
        #region FollowUser Tests

        [Fact]
        public async Task FollowUser_ShouldThrowValidationException_WhenTryingToFollowSelf()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var user = new User(Guid.NewGuid(), "testuser");

            db.Users.Add(user);
            db.SaveChanges();

            var service = new FollowService(db);

            var command = new CreateFollowerCommand(followerId: user.Id, userToFollowId: user.Id);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => service.FollowUser(command));

            Assert.Equal(nameof(Errors.CannotFollowYourself), exception.ErrorType);
        }

        [Fact]
        public async Task FollowUser_ShouldThrowNotFoundException_WhenFollowerDoesNotExist()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var (follower, followed) = SeedUsers(db);
            db.Users.Remove(follower);
            db.SaveChanges();

            var service = new FollowService(db);
            var command = new CreateFollowerCommand(followerId: follower.Id, userToFollowId: followed.Id);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => service.FollowUser(command));

            Assert.Equal(nameof(Errors.FollowerUserNotFound), exception.ErrorType);
        }

        [Fact]
        public async Task FollowUser_ShouldThrowNotFoundException_WhenUserToFollowDoesNotExist()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var (follower, followed) = SeedUsers(db);
            db.Users.Remove(followed);
            db.SaveChanges();

            var service = new FollowService(db);
            var command = new CreateFollowerCommand(followerId: follower.Id, userToFollowId: followed.Id);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => service.FollowUser(command));
            
            Assert.Equal(nameof(Errors.UserToFollowNotFound), exception.ErrorType);
        }

        [Fact]
        public async Task FollowUser_ShouldThrowValidationException_WhenAlreadyFollowing()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var (follower, followed) = SeedUsers(db);

            db.FollowRelationships.Add(new FollowRelationship(follower, followed));
            db.SaveChanges();

            var service = new FollowService(db);
            var command = new CreateFollowerCommand(followerId: follower.Id, userToFollowId: followed.Id);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => service.FollowUser(command));
        
            Assert.Equal(nameof(Errors.AlreadyFollowingUser), exception.ErrorType);
        }

        [Fact]
        public async Task FollowUser_ShouldReturnRelationship_WhenSuccess()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var (follower, followed) = SeedUsers(db);

            var service = new FollowService(db);
            var command = new CreateFollowerCommand(followerId: follower.Id, userToFollowId: followed.Id);

            // Act
            var result = await service.FollowUser(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(follower.Id, result.Follower.Id);
            Assert.Equal(followed.Id, result.Followed.Id);
            Assert.True(db.FollowRelationships.AnyAsync(r =>
                r.FollowerId == follower.Id &&
                r.FollowedId == followed.Id).Result);
        }
        
        #endregion

        #region UnfollowUser Tests

        [Fact]
        public async Task UnfollowUser_ShouldThrowNotFoundException_WhenRelationshipDoesNotExist()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var (follower, followed) = SeedUsers(db);

            var service = new FollowService(db);
            var command = new DeleteFollowerCommand(unfollowerId: follower.Id, userToUnfollowId: followed.Id);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => service.UnfollowUser(command));
        
            Assert.Equal(nameof(Errors.FollowRelationshipNotFound), exception.ErrorType);
        }

        [Fact]
        public async Task UnfollowUser_ShouldRemoveRelationship_WhenExists()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var (follower, followed) = SeedUsers(db);

            // Seed follow relationship
            var relationship = new FollowRelationship(follower, followed);
            db.FollowRelationships.Add(relationship);
            db.SaveChanges();

            var service = new FollowService(db);
            var command = new DeleteFollowerCommand(unfollowerId: follower.Id, userToUnfollowId: followed.Id);

            // Act
            await service.UnfollowUser(command);

            // Assert
            var exists = await db.FollowRelationships
                .AnyAsync(r => r.FollowerId == follower.Id && r.FollowedId == followed.Id);

            Assert.False(exists);
        }
        
        #endregion

        #region Private Methods
        private TwitterApiDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TwitterApiDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TwitterApiDbContext(options);
        }

        private (User follower, User followed) SeedUsers(TwitterApiDbContext db)
        {
            var follower = new User(Guid.NewGuid(), "followerUser");
            var followed = new User(Guid.NewGuid(), "followedUser");

            db.Users.AddRange(follower, followed);
            db.SaveChanges();

            return (follower, followed);
        }

        #endregion
    }
}
