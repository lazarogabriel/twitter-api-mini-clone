using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using twitter.api.application.Models.Follow;
using twitter.api.application.Services.Abstractions;
using twitter.api.domain.Models;
using twitter.api.web.Models.Responses;
using twitter.api.web.Models;
using System.Collections.Generic;

namespace twitter.api.web.Controllers
{
    [ApiController]
    [Route("Users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserContext _userContext;
        private readonly IFollowService _followService;
        private readonly IMapper _mapper;

        public UsersController(
            IUserService userService,
            IUserContext userContext,
            IFollowService followService,
            IMapper mapper)
        {
            _userService = userService;
            _userContext = userContext;
            _followService = followService;
            _mapper = mapper;
        }

        /// <summary>
        /// Starts following the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to follow.</param>
        /// <returns>Information about the new follow relationship.</returns>
        [HttpPost("{userId:guid}/followers")]
        [ProducesResponseType(typeof(FollowRelationshipResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Follow(Guid userId)
        {
            var followerId = _userContext.GetCurrentUserId();

            var relationship = await _followService.FollowUser(
                new CreateFollowerCommand(
                    followerId: followerId,
                    userToFollowId: userId));

            return CreatedAtAction(
                nameof(GetUserById), 
                new { userId },
                _mapper.Map<FollowRelationshipResponse>(relationship));
        }

        /// <summary>
        /// Stops following the specified user.
        /// </summary>
        /// <param name="userId">The id of the user to unfollow.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{userId:guid}/followers")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Unfollow(Guid userId)
        {
            var unfollowerId = _userContext.GetCurrentUserId();

            await _followService.UnfollowUser(
                new DeleteFollowerCommand(
                    unfollowerId: unfollowerId, 
                    userToUnfollowId: userId));

            return NoContent();
        }

        /// <summary>
        /// Creates a new user (Solo para la evaluacion del ejercicio tecnico).
        /// </summary>
        /// <param name="request">The information required to create a user.</param>
        /// <returns>The created user.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BasicUserResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var user = await _userService.CreateUser(request.Username);

            return CreatedAtAction(
                nameof(GetUserById),
                new { userId = user.Id },
                _mapper.Map<BasicUserResponse>(user));
        }

        /// <summary>
        /// Gets a list of all users in the system (Solo para la evaluacion del ejercicio tecnico).
        /// </summary>
        /// <returns>The users.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BasicUserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();

            return Ok(_mapper.Map<IEnumerable<BasicUserResponse>>(users));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            throw new NotImplementedException("Get user by id not implemented yet");
        }
    }
}

