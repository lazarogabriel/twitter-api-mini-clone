using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using twitter.api.application.Models.Follow;
using twitter.api.application.Services.Abstractions;
using twitter.api.domain.Models;
using twitter.api.web.Controllers;
using twitter.api.web.Models.Responses;
using twitter.api.web.Models;
using twitter.api.application.Models.Users;

namespace twitter.api.web.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly Mock<IFollowService> _followServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UsersController _classUnderTests;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _userContextMock = new Mock<IUserContext>();
            _followServiceMock = new Mock<IFollowService>();
            _mapperMock = new Mock<IMapper>();

            _classUnderTests = new UsersController(
                _userServiceMock.Object,
                _userContextMock.Object,
                _followServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Follow_ShouldReturnCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var follower = new BasicUserResponse
            {
                Id = Guid.NewGuid(),
                Username = "follower",
            };

            var userToFollow = new BasicUserResponse
            {
                Id = Guid.NewGuid(),
                Username = "followed",
            };

            _userContextMock
                .Setup(x => x.GetCurrentUserId())
                .Returns(follower.Id);

            var relationship = new FollowRelationship(
                follower: new User(follower.Id, follower.Username),
                followed: new User(userToFollow.Id, userToFollow.Username)
            );

            _followServiceMock
                .Setup(x => x.FollowUser(It.Is<CreateFollowerCommand>(
                    c => c.FollowerId == follower.Id && 
                    c.UserToFollowId == userToFollow.Id)))
                .ReturnsAsync(relationship);

            var response = new FollowRelationshipResponse 
            { 
                Follower = follower, 
                Followed = userToFollow 
            };

            _mapperMock
                .Setup(x => x.Map<FollowRelationshipResponse>(relationship))
                .Returns(response);

            // Act
            var result = await _classUnderTests.Follow(userToFollow.Id);

            // Assert
            var createdAt = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_classUnderTests.GetUserById), createdAt.ActionName);
            Assert.Equal(response, createdAt.Value);

            _userContextMock.Verify(x => x.GetCurrentUserId(), Times.Once);
            _followServiceMock.Verify(x => x.FollowUser(It.IsAny<CreateFollowerCommand>()), Times.Once);
            _mapperMock.Verify(x => x.Map<FollowRelationshipResponse>(It.IsAny<FollowRelationship>()), Times.Once);
        }

        [Fact]
        public async Task Unfollow_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var unfollowerId = Guid.NewGuid();
            var userToUnfollowId = Guid.NewGuid();

            _userContextMock
                .Setup(x => x.GetCurrentUserId())
                .Returns(unfollowerId);

            _followServiceMock
                .Setup(x => x.UnfollowUser(It.Is<DeleteFollowerCommand>(
                    c => c.UnfollowerId == unfollowerId && c.UserToUnfollowId == userToUnfollowId)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _classUnderTests.Unfollow(userToUnfollowId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _userContextMock.Verify(x => x.GetCurrentUserId(), Times.Once);
            _followServiceMock.Verify(x => x.UnfollowUser(It.IsAny<DeleteFollowerCommand>()), Times.Once);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var username = "newuser";
            var request = new CreateUserRequest { Username = username };

            var user = new User(userId, username);

            _userServiceMock
                .Setup(x => x.CreateUser(username))
                .ReturnsAsync(user);

            var response = new BasicUserResponse { Id = userId, Username = username };

            _mapperMock
                .Setup(x => x.Map<BasicUserResponse>(user))
                .Returns(response);

            // Act
            var result = await _classUnderTests.CreateUser(request);

            // Assert
            var createdAt = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_classUnderTests.GetUserById), createdAt.ActionName);
            Assert.Equal(response, createdAt.Value);

            _userServiceMock.Verify(x => x.CreateUser(username), Times.Once);
            _mapperMock.Verify(x => x.Map<BasicUserResponse>(user), Times.Once);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnOkResult_WithUsers()
        {
            // Arrange
            var users = new List<BasicUserQueryResponse>
            {
                new (Guid.NewGuid(), "alice"),
                new (Guid.NewGuid(), "bob")
            };

            _userServiceMock
                .Setup(x => x.GetAllUsers())
                .ReturnsAsync(users);

            var userResponses = new List<BasicUserResponse>
            {
                new() { Id = users[0].Id, Username = "alice" },
                new() { Id = users[1].Id, Username = "bob" }
            };

            _mapperMock
                .Setup(x => x.Map<IEnumerable<BasicUserResponse>>(users))
                .Returns(userResponses);

            // Act
            var result = await _classUnderTests.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<BasicUserResponse>>(okResult.Value);
            Assert.Equal(2, ((List<BasicUserResponse>)value).Count);

            _userServiceMock.Verify(x => x.GetAllUsers(), Times.Once);
            _mapperMock.Verify(x => x.Map<IEnumerable<BasicUserResponse>>(users), Times.Once);
        }
    }
}
