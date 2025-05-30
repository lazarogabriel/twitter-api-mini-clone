using twitter.api.domain.Constants;
using twitter.api.domain.Exceptions;
using twitter.api.domain.Models;

namespace twitter.api.domain.tests.Models
{
    public class TweetTests
    {
        private User CreateValidUser() => new User("lazarovecchi");

        [Fact]
        public void Constructor_SetsPropertiesCorrectly_WhenValidData()
        {
            var user = CreateValidUser();
            var content = "Hello Twitter!";

            var tweet = new Tweet(content, user);

            Assert.Equal(content, tweet.Content);
            Assert.Equal(user, tweet.Author);
            Assert.True((DateTime.UtcNow - tweet.CreatedAt).TotalSeconds < 2);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void Constructor_ShouldThrowInvalidParameterException_WhenContentIsNullOrWhiteSpace(string invalidContent)
        {
            var user = CreateValidUser();

            var exception = Assert.Throws<InvalidParameterException>(() => new Tweet(invalidContent, user));

            Assert.Equal(nameof(Errors.TweetContentIsRequired), exception.ErrorType);
        }

        [Fact]
        public void Constructor_ShouldThrowInvalidParameterException_WhenContentIsLongerThan280()
        {
            var user = CreateValidUser();
            var tooLongContent = new string('a', 281);

            var excpetion = Assert.Throws<InvalidParameterException>(() => new Tweet(tooLongContent, user));

            Assert.Equal(nameof(Errors.TweetContentCannotBeMoreThan280Chars), excpetion.ErrorType);
        }

        [Fact]
        public void Constructor_ShouldThrowInvalidParameterException_WhenAuthorIsNull()
        {
            var content = "Valid tweet!";

            var exception = Assert.Throws<InvalidParameterException>(() => new Tweet(content, null));

            Assert.Equal(nameof(Errors.TweetAuthorIsRequired), exception.ErrorType);
        }

        [Fact]
        public void Content_IsTrimmed_WhenSet()
        {
            var user = CreateValidUser();
            var tweet = new Tweet("  Trim me!  ", user);

            Assert.Equal("Trim me!", tweet.Content);
        }

        [Fact]
        public void Setting_Content_ToNullOrWhiteSpace_ThrowsInvalidParameterException()
        {
            var user = CreateValidUser();
            var tweet = new Tweet("Initial", user);

            var invalidParameterExceptionNull = Assert.Throws<InvalidParameterException>(() => tweet.Content = null);
            var invalidParameterExceptionWhiteSpace = Assert.Throws<InvalidParameterException>(() => tweet.Content = "  ");

            Assert.Equal(nameof(Errors.TweetContentIsRequired), invalidParameterExceptionNull.ErrorType);
            Assert.Equal(nameof(Errors.TweetContentIsRequired), invalidParameterExceptionWhiteSpace.ErrorType);

        }

        [Fact]
        public void Setting_Content_ToTooLong_ThrowsInvalidParameterException()
        {
            var user = CreateValidUser();
            var tweet = new Tweet("Initial", user);
            var tooLongContent = new string('x', 281);

            var exception = Assert.Throws<InvalidParameterException>(() => tweet.Content = tooLongContent);

            Assert.Equal(nameof(Errors.TweetContentCannotBeMoreThan280Chars), exception.ErrorType);
        }
    }
}
