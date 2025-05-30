using twitter.api.domain.Constants;
using twitter.api.domain.Exceptions;
using twitter.api.domain.Models;

namespace twitter.api.domain.tests.Models
{
    public class UserTests
    {
        private User CreateValidUser(Guid? id = null, string username = "lazarovecchi") => new User(id ?? Guid.NewGuid(), username);

        [Fact]
        public void Constructor_SetsUsernameAndCreatedAt()
        {
            var username = "testuser";
            var before = DateTime.UtcNow;

            var user = new User(username);

            Assert.Equal(username, user.Username);
            Assert.True(user.CreatedAt >= before && user.CreatedAt <= DateTime.UtcNow);
            Assert.Empty(user.Followers);
            Assert.Empty(user.Following);
            Assert.Empty(user.Tweets);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_Throws_WhenUsernameNullOrWhitespace(string invalid)
        {
            Assert.Throws<InvalidParameterException>(() => new User(invalid));
        }

        [Theory]
        [InlineData("abc")] // too short
        [InlineData("thisusernameistoolong")] // too long
        public void Constructor_Throws_WhenUsernameOutOfRange(string invalid)
        {
            Assert.Throws<InvalidParameterException>(() => new User(invalid));
        }

        [Fact]
        public void Username_IsTrimmed_WhenSet()
        {
            var username = "   validuser   ";
            var user = new User(username);

            Assert.Equal("validuser", user.Username);
        }

        [Fact]
        public void CreateTweet_AddsTweetToUserTweets()
        {
            var user = CreateValidUser();
            var content = "my tweet content";

            var tweet = user.CreateTweet(content);

            Assert.Single(user.Tweets);
            Assert.Equal(tweet, user.Tweets.First());
            Assert.Equal(content, tweet.Content);
            Assert.Equal(user, tweet.Author);
        }

        [Fact]
        public void Follow_AddsRelationshipToBothUsers()
        {
            var follower = CreateValidUser(username: "follower");
            var followed = CreateValidUser(username: "followed");

            var relationship = follower.Follow(followed);

            Assert.Single(follower.Following);
            Assert.Single(followed.Followers);
            Assert.Equal(relationship, follower.Following.First());
            Assert.Equal(relationship, followed.Followers.First());
            Assert.Equal(follower, relationship.Follower);
            Assert.Equal(followed, relationship.Followed);
        }

        [Fact]
        public void Follow_ShouldThrowInvalidParameterException_WhenUserToFollowIsNull()
        {
            var user = CreateValidUser();

            var exception = Assert.Throws<InvalidParameterException>(() => user.Follow(null));

            Assert.Equal(nameof(Errors.UserToFollowIsRequired), exception.ErrorType);
        }

        [Fact]
        public void Follow_ShouldThrowValidationException_WhenTryingToFollowYourself()
        {
            var user = CreateValidUser();

            var exception = Assert.Throws<ValidationException>(() => user.Follow(user));

            Assert.Equal(nameof(Errors.CannotFollowYourself), exception.ErrorType);
        }

        [Fact]
        public void Unfollow_RemovesRelationshipFromBothUsers()
        {
            var user1 = CreateValidUser(username: "user1");
            var user2 = CreateValidUser(username: "user2");

            var rel = user1.Follow(user2);

            user1.Unfollow(user2, rel);

            Assert.Empty(user1.Following);
            Assert.Empty(user2.Followers);
        }

        [Fact]
        public void RemoveFollower_RemovesRelationship()
        {
            var user1 = CreateValidUser(username: "user1");
            var user2 = CreateValidUser(username: "user2");

            var relationship = user2.Follow(user1);

            user1.RemoveFollower(relationship);

            Assert.Empty(user1.Followers);
            Assert.Single(user2.Following); // user2 is still following user1 (unless also unfollowed)
        }

        [Fact]
        public void FollowersCount_ReturnsNumberOfFollowers()
        {
            var user = CreateValidUser();
            Assert.Equal(0, user.FollowersCount);

            var follower1 = CreateValidUser(username: "userOne");
            var follower2 = CreateValidUser(username: "userTwo");

            follower1.Follow(user);
            follower2.Follow(user);

            Assert.Equal(2, user.FollowersCount);
        }
    }
}
