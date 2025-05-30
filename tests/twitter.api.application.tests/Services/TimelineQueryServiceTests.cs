using Microsoft.EntityFrameworkCore;
using twitter.api.application.Models.Timeline;
using twitter.api.application.Services;
using twitter.api.data.DbContexts;
using twitter.api.domain.Models;

namespace twitter.api.application.tests.Services
{
    public class TimelineQueryServiceTests
    {
        #region GetTimeline Tests

        [Fact]
        public async Task GetTimeline_ShouldReturnEmpty_WhenUserFollowsNoOne()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var user = new User(Guid.NewGuid(), "followerUser");

            db.Users.Add(user);
            db.SaveChanges();

            var service = new TimelineQueryService(db);

            var query = new GetTimelineQuery(
                userId: user.Id,
                page: 1,
                pageSize: 10);

            // Act
            var result = await service.GetTimeline(query);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalItems);
        }

        [Fact]
        public async Task GetTimeline_ShouldReturnAllTweetsOfFollowedUsers_WhenUnderPageSize()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var (follower, followedUsers, tweets) = SeedUsersWithTweets(db, follows: 2, tweetsPerUser: 2);

            var service = new TimelineQueryService(db);

            var query = new GetTimelineQuery(
                userId: follower.Id,
                page: 1,
                pageSize: 10);

            // Act
            var result = await service.GetTimeline(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.TotalItems);
            Assert.Equal(4, result.Items.Count);
            // All tweets are from followed users
            Assert.All(result.Items, t => Assert.Contains(followedUsers.Select(u => u.Id), id => id == t.AuthorId));
        }

        [Fact]
        public async Task GetTimeline_ShouldOrderTweetsDescendingByCreatedAt()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var (follower, followedUsers, tweets) = SeedUsersWithTweets(db, follows: 2, tweetsPerUser: 3);

            var service = new TimelineQueryService(db);

            var query = new GetTimelineQuery(
                userId: follower.Id,
                page: 1,
                pageSize: 10);

            // Act
            var result = await service.GetTimeline(query);

            // Assert
            var ordered = result.Items.OrderByDescending(t => t.CreatedAt).Select(t => t.TweetId);
            Assert.Equal(ordered, result.Items.Select(t => t.TweetId));
        }

        [Fact]
        public async Task GetTimeline_ShouldPaginateResultsCorrectly()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var (follower, followedUsers, tweets) = SeedUsersWithTweets(db, follows: 2, tweetsPerUser: 5); // 10 tweets

            var service = new TimelineQueryService(db);

            // First page
            var query1 = new GetTimelineQuery(
                userId: follower.Id,
                page: 1,
                pageSize: 3);

            var result1 = await service.GetTimeline(query1);

            // Second page
            var query2 = new GetTimelineQuery(
                userId: follower.Id,
                page: 2,
                pageSize: 3);

            var result2 = await service.GetTimeline(query2);

            // Assert
            Assert.Equal(10, result1.TotalItems);
            Assert.Equal(3, result1.Items.Count);
            Assert.Equal(3, result2.Items.Count);
            Assert.DoesNotContain(result1.Items.Select(t => t.TweetId), id => result2.Items.Select(t => t.TweetId).Contains(id));
        }

        [Fact]
        public async Task GetTimeline_ShouldReturnEmpty_WhenUserDoesNotExist()
        {
            // Arrange
            var db = CreateInMemoryContext();

            var service = new TimelineQueryService(db);

            var query = new GetTimelineQuery(
                userId: Guid.NewGuid(), // No user
                page: 1,
                pageSize: 10);

            // Act
            var result = await service.GetTimeline(query);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalItems);
        }

        [Fact]
        public async Task GetTimeline_ShouldReturnEmpty_WhenNoTweetsFromFollowedUsers()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var user1 = new User(Guid.NewGuid(), "user1");
            var user2 = new User(Guid.NewGuid(), "user2");

            db.Users.AddRange(user1, user2);
            db.FollowRelationships.Add(new FollowRelationship(user1, user2));
            db.SaveChanges();

            var service = new TimelineQueryService(db);
            var query = new GetTimelineQuery(
                userId: user1.Id,
                page: 1,
                pageSize: 10);

            // Act
            var result = await service.GetTimeline(query);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalItems);
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

        private (User follower, List<User> followed, List<Tweet> tweets) SeedUsersWithTweets(TwitterApiDbContext db, int follows = 3, int tweetsPerUser = 2)
        {
            var follower = new User(Guid.NewGuid(), "followerUser");

            db.Users.Add(follower);

            var followedUsers = new List<User>();
            var tweets = new List<Tweet>();

            for (int i = 0; i < follows; i++)
            {
                var user = new User(Guid.NewGuid(), $"followedUser{i}");

                db.Users.Add(user);
                db.FollowRelationships.Add(new FollowRelationship(follower, followed: user));

                followedUsers.Add(user);

                for (int j = 0; j < tweetsPerUser; j++)
                {
                    // Set CreatedAt for order test
                    var createdAt = DateTime.UtcNow.AddMinutes(-(i * 10 + j));
                    var tweet = new Tweet(
                        content: $"tweet{i}-{j}", 
                        author: user, 
                        createdAt: createdAt);

                    db.Tweets.Add(tweet);

                    tweets.Add(tweet);
                }
            }

            db.SaveChanges();

            return (follower, followedUsers, tweets);
        }


        #endregion
    }

}
