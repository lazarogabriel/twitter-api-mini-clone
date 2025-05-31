using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using twitter.api.application.Models.Timeline;
using twitter.api.application.Services.Abstractions;
using twitter.api.web.Controllers;
using twitter.api.web.Models.Responses;
using twitter.api.web.Models.Responses.Base;
using System.Threading.Tasks;
using System;
using twitter.api.application.Models.Base;
using System.Linq;
using System.Collections.Generic;

namespace twitter.api.web.tests.Controllers
{
    public class TimelineControllerTests
    {
        private readonly Mock<ITimelineQueryService> _timelineQueryServiceMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TimelineController _classUnderTests;

        public TimelineControllerTests()
        {
            _timelineQueryServiceMock = new Mock<ITimelineQueryService>();
            _userContextMock = new Mock<IUserContext>();
            _mapperMock = new Mock<IMapper>();
            _classUnderTests = new TimelineController(
                _timelineQueryServiceMock.Object,
                _userContextMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task GetTimeline_ShouldReturnOkResult_WithPaginatedResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var page = 1;
            var pageSize = 20;

            var paginatedQueryResponse = new PaginatedQueryResponse<TweetQueryResponse>(
                page, pageSize, 2, new()
                {
                new TweetQueryResponse { TweetId = 1, Content = "Hi", AuthorId = Guid.NewGuid(), AuthorUsername = "alice" },
                new TweetQueryResponse { TweetId = 2, Content = "Hello", AuthorId = Guid.NewGuid(), AuthorUsername = "bob" }
                }
            );

            var paginatedResponse = new PaginatedResponse<TweetResponse> {
                Page = page,
                PageSize = pageSize, 
                TotalItems = 2, 
                Items =
                [
                    new TweetResponse { Id = 1, Content = "Hi", AuthorId = paginatedQueryResponse.Items[0].AuthorId, AuthorUsername = "alice", CreatedAt = DateTime.UtcNow },
                    new TweetResponse { Id = 2, Content = "Hello", AuthorId = paginatedQueryResponse.Items[1].AuthorId, AuthorUsername = "bob", CreatedAt = DateTime.UtcNow }
                ]
            };

            _userContextMock
                .Setup(u => u.GetCurrentUserId())
                .Returns(userId);

            _timelineQueryServiceMock
                .Setup(t => t.GetTimeline(It.Is<GetTimelineQuery>(q =>
                    q.UserId == userId && 
                    q.Page == page && 
                    q.PageSize == pageSize)))
                .ReturnsAsync(paginatedQueryResponse);

            _mapperMock
                .Setup(m => m.Map<PaginatedResponse<TweetResponse>>(paginatedQueryResponse))
                .Returns(paginatedResponse);

            // Act
            var result = await _classUnderTests.GetTimeline(page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var response = Assert.IsType<PaginatedResponse<TweetResponse>>(okResult.Value);

            Assert.Equal(paginatedResponse.TotalItems, response.TotalItems);
            Assert.Equal(paginatedResponse.Items.Count(), response.Items.Count());
        }

        [Fact]
        public async Task GetTimeline_ShouldCallService_WithCorrectParameters()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var page = 2;
            var pageSize = 10;

            _userContextMock
                .Setup(u => u.GetCurrentUserId())
                .Returns(userId);

            _timelineQueryServiceMock
                .Setup(t => t.GetTimeline(It.Is<GetTimelineQuery>(q =>
                    q.UserId == userId && 
                    q.Page == page && 
                    q.PageSize == pageSize)))
                .ReturnsAsync(new PaginatedQueryResponse<TweetQueryResponse>(page, pageSize, 0, new List<TweetQueryResponse>()))
                .Verifiable();

            _mapperMock
                .Setup(m => m.Map<PaginatedResponse<TweetResponse>>(It.IsAny<PaginatedQueryResponse<TweetQueryResponse>>()))
                .Returns(new PaginatedResponse<TweetResponse>
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = 0,
                    Items = Array.Empty<TweetResponse>()
                });

            // Act
            await _classUnderTests.GetTimeline(page, pageSize);

            // Assert
            _timelineQueryServiceMock.Verify();
        }

        [Fact]
        public async Task GetTimeline_ShouldReturnEmptyResult_WhenNoTweets()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var page = 1;
            var pageSize = 20;
            var emptyPaginatedQueryResponse = new PaginatedQueryResponse<TweetQueryResponse>(page, pageSize, 0, new List<TweetQueryResponse>());
            var emptyPaginatedResponse = new PaginatedResponse<TweetResponse>
            { 
                Page = page, 
                PageSize = pageSize,
                TotalItems = 0, 
                Items = Array.Empty<TweetResponse>() 
            };

            _userContextMock
                .Setup(u => u.GetCurrentUserId())
                .Returns(userId);

            _timelineQueryServiceMock
                .Setup(t => t.GetTimeline(It.IsAny<GetTimelineQuery>()))
                .ReturnsAsync(emptyPaginatedQueryResponse);

            _mapperMock
                .Setup(m => m.Map<PaginatedResponse<TweetResponse>>(emptyPaginatedQueryResponse))
                .Returns(emptyPaginatedResponse);

            // Act
            var result = await _classUnderTests.GetTimeline(page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PaginatedResponse<TweetResponse>>(okResult.Value);

            Assert.Empty(response.Items);
        }
    }
}
