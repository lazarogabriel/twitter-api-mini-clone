using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using twitter.api.application.Models.Timeline;
using twitter.api.application.Services.Abstractions;
using twitter.api.web.Models.Responses;
using twitter.api.web.Models.Responses.Base;

namespace twitter.api.web.Controllers
{
    [ApiController]
    [Route("Timeline")]
    public class TimelineController : ControllerBase
    {
        #region Fields

        private readonly ITimelineQueryService _timelineQueryService;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public TimelineController(
            ITimelineQueryService timelineQueryService, 
            IUserContext userContext, 
            IMapper mapper)
        {
            _timelineQueryService = timelineQueryService;
            _userContext = userContext;
            _mapper = mapper;
        }

        #endregion

        /// <summary>
        /// Gets the timeline for the authenticated user, showing tweets from users they follow.
        /// </summary>
        /// <param name="page">The page number (starting from 1).</param>
        /// <param name="pageSize">The page size (default 20, max 100).</param>
        /// <returns>Paged list of tweets in the user's timeline.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<TweetResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTimeline([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = _userContext.GetCurrentUserId();

            var timelinePaginated = await _timelineQueryService.GetTimeline(
                new GetTimelineQuery(
                    userId: userId,
                    page: page,
                    pageSize: pageSize));

            return Ok(_mapper.Map<PaginatedResponse<TweetResponse>>(timelinePaginated));
        }
    }
}
