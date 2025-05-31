using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using twitter.api.application.Models.Follow;
using twitter.api.application.Services.Abstractions;
using twitter.api.web.Controllers;
using twitter.api.web.Models;
using twitter.api.web.Models.Responses;
using twitter.api.domain.Models;

namespace twitter.api.web.Tests.Controllers
{
    public class TweetsControllerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITweetService> _tweetServiceMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly TweetsController _classUnderTests;

        public TweetsControllerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _tweetServiceMock = new Mock<ITweetService>();
            _userContextMock = new Mock<IUserContext>();
            _classUnderTests = new TweetsController(
                _mapperMock.Object,
                _tweetServiceMock.Object,
                _userContextMock.Object
            );
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WithTweetResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var content = "Hello, world!";
            var request = new CreateTweetRequest { Content = content };

            var tweet = new Tweet(content, new User(userId, "testuser"));
            var tweetResponse = new TweetResponse
            {
                Id = tweet.Id,
                Content = content,
                AuthorId = userId,
                AuthorUsername = "testuser",
                CreatedAt = tweet.CreatedAt
            };

            _userContextMock
                .Setup(u => u.GetCurrentUserId())
                .Returns(userId);

            _tweetServiceMock
                .Setup(s => s.CreateTweet(It.Is<CreateTweetCommand>(c =>
                    c.AuthorId == userId && 
                    c.Content == content
                )))
                .ReturnsAsync(tweet);

            _mapperMock
                .Setup(m => m.Map<TweetResponse>(tweet))
                .Returns(tweetResponse);

            // Act
            var result = await _classUnderTests.Create(request);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_classUnderTests.GetById), createdAtResult.ActionName);

            var value = Assert.IsType<TweetResponse>(createdAtResult.Value);
            Assert.Equal(tweetResponse.Content, value.Content);
            Assert.Equal(tweetResponse.AuthorUsername, value.AuthorUsername);

            _userContextMock.Verify();
            _tweetServiceMock.Verify();
            _mapperMock.Verify();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithTweets()
        {
            // Arrange
            var tweets = new List<Tweet>
            {
                new Tweet("Hi!", new User(Guid.NewGuid(), "user1")),
                new Tweet("Hola!", new User(Guid.NewGuid(), "user2"))
            };

            var tweetResponses = new List<TweetResponse>
            {
                new() { Id = tweets[0].Id, Content = "Hi!", AuthorUsername = "user1", AuthorId = tweets[0].Author.Id, CreatedAt = tweets[0].CreatedAt },
                new() { Id = tweets[1].Id, Content = "Hola!", AuthorUsername = "user2", AuthorId = tweets[1].Author.Id, CreatedAt = tweets[1].CreatedAt }
            };

            _tweetServiceMock
                .Setup(s => s.GetAllTweets())
                .ReturnsAsync(tweets);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<TweetResponse>>(tweets))
                .Returns(tweetResponses);

            // Act
            var result = await _classUnderTests.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<TweetResponse>>(okResult.Value);
            Assert.Equal(2, (value as List<TweetResponse>).Count);

            _tweetServiceMock.Verify();
            _mapperMock.Verify();
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoTweets()
        {
            // Arrange
            var tweets = new List<Tweet>();
            var tweetResponses = new List<TweetResponse>();

            _tweetServiceMock
                .Setup(s => s.GetAllTweets())
                .ReturnsAsync(tweets);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<TweetResponse>>(tweets))
                .Returns(tweetResponses);

            // Act
            var result = await _classUnderTests.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<TweetResponse>>(okResult.Value);
            Assert.Empty(value);

            _tweetServiceMock.Verify();
            _mapperMock.Verify();
        }
    }
}
