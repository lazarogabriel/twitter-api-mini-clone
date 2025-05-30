using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using twitter.api.application.Models.Follow;
using twitter.api.application.Services.Abstractions;
using twitter.api.web.Models;
using twitter.api.web.Models.Responses;

namespace twitter.api.web.Controllers
{
    [ApiController]
    [Route("Tweets")]
    public class TweetsController : ControllerBase
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly ITweetService _tweetService;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructor

        public TweetsController(IMapper mapper, ITweetService tweetService, IUserContext userContext)
        {
            _mapper = mapper;
            _tweetService = tweetService;
            _userContext = userContext;
        }

        #endregion

        #region Endpoints

        /// <summary>
        /// Creates a tweet.
        /// </summary>
        /// <param name="request">Required information to create a tweet.</param>
        /// <returns>Tweet just created.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TweetResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(CreateTweetRequest request)
        {
            var userId = _userContext.GetCurrentUserId();

            var tweet = await _tweetService.CreateTweet(
                new CreateTweetCommand(
                    authorId: userId,
                    content: request.Content));

            return CreatedAtAction(
                nameof(GetById),
                routeValues: new { TweetId = tweet.Id },
                _mapper.Map<TweetResponse>(tweet));
        }

        /// <summary>
        /// Gets all tweets by user id.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(TweetResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var userId = _userContext.GetCurrentUserId();

            var tweets = await _tweetService.GetAllTweetsByAuthorId(authorId: userId);

            return Ok(_mapper.Map<IEnumerable<TweetResponse>>(tweets));
        }

        [HttpGet("{tweetId}")]
        public Task<IActionResult> GetById([FromRoute] long tweetId)
        {
            throw new NotImplementedException("Get by id is not implemented yet");
        }

        #endregion
    }
}
