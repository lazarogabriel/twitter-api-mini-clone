using Microsoft.EntityFrameworkCore;
using twitter.api.application.Models.Follow;
using twitter.api.application.Services;
using twitter.api.data.DbContexts;
using twitter.api.domain.Exceptions;
using twitter.api.domain.Models;

namespace twitter.api.application.tests.Services
{ 
    public class TweetServiceTests
    {
        [Fact]
        public async Task CreateTweet_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var service = new TweetService(db);
            var command = new CreateTweetCommand(authorId: Guid.NewGuid(), content: "Hola mundo");

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.CreateTweet(command));
        }

        [Fact]
        public async Task CreateTweet_ShouldThrowInvalidParameterException_WhenContentIsNullOrWhiteSpace()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var user = new User(Guid.NewGuid(), "testuser");

            db.Users.Add(user);
            db.SaveChanges();

            var service = new TweetService(db);

            var commands = new[]
            {
                new CreateTweetCommand(authorId: user.Id, content: null),
                new CreateTweetCommand(authorId: user.Id, content: ""),
                new CreateTweetCommand(authorId: user.Id, content: "     ")
            };

            // Act & Assert
            foreach (var command in commands)
            {
                await Assert.ThrowsAsync<InvalidParameterException>(() => service.CreateTweet(command));
            }
        }

        [Fact]
        public async Task CreateTweet_ShouldThrowInvalidParameterException_WhenContentExceedsMaxLength()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var user = new User(Guid.NewGuid(), "testuser");

            db.Users.Add(user);
            db.SaveChanges();

            var service = new TweetService(db);

            var longContent = new string('A', 281); // Exceeds 280 chars
            var command = new CreateTweetCommand(authorId: user.Id, content: longContent);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidParameterException>(() => service.CreateTweet(command));
        }

        [Fact]
        public async Task CreateTweet_ShouldCreateTweet_WhenValid()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var user = new User(Guid.NewGuid(), "testuser");

            db.Users.Add(user);
            db.SaveChanges();

            var service = new TweetService(db);
            var content = "Hello Twitter!";
            var command = new CreateTweetCommand(authorId: user.Id, content: content);

            // Act
            var result = await service.CreateTweet(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(content, result.Content);
            Assert.Equal(user.Id, result.Author.Id);
            Assert.True(result.CreatedAt <= DateTime.UtcNow && result.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
            Assert.True(await db.Tweets.AnyAsync(t => t.Id == result.Id));
        }

        [Fact]
        public async Task CreateTweet_ShouldTrimContent_WhenContentHasLeadingOrTrailingSpaces()
        {
            // Arrange
            var db = CreateInMemoryContext();
            var user = new User(Guid.NewGuid(), "testuser");

            db.Users.Add(user);
            db.SaveChanges();

            var service = new TweetService(db);
            var contentWithSpaces = "    Tweet with spaces     ";
            var expected = "Tweet with spaces";
            var command = new CreateTweetCommand(authorId: user.Id, content: contentWithSpaces);

            // Act
            var result = await service.CreateTweet(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, result.Content);
        }

        #region Private Methods

        private TwitterApiDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TwitterApiDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TwitterApiDbContext(options);
        }

        #endregion
    }
}
