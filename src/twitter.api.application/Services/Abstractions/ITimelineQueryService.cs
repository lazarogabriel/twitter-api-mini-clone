using System.Threading.Tasks;
using twitter.api.application.Models.Base;
using twitter.api.application.Models.Timeline;

namespace twitter.api.application.Services.Abstractions
{
    /// <summary>
    /// Exposes operations for retrieving a user's timeline of tweets,
    /// specifically the tweets from users that the user is following.
    /// </summary>
    public interface ITimelineQueryService
    {
        /// <summary>
        /// Retrieves a paginated timeline of tweets for a specific user.
        /// </summary>
        /// <param name="query">The query parameters including userId, pagination, etc.</param>
        /// <returns>A paginated list of tweets for the user's timeline.</returns>
        Task<PaginatedQueryResponse<TweetQueryResponse>> GetTimeline(GetTimelineQuery query);
    }
}
