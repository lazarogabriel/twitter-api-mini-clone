using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using twitter.api.application.Models.Base;
using twitter.api.application.Models.Timeline;
using twitter.api.application.Services.Abstractions;
using twitter.api.data.DbContexts;

namespace twitter.api.application.Services
{
    public class TimelineQueryService : ITimelineQueryService
    {
        #region Constructor

        private readonly ITwitterApiDbContext _dbContext;

        #endregion

        #region Constructor

        public TimelineQueryService(ITwitterApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        /// <inheritdoc/>
        public async Task<PaginatedQueryResponse<TweetQueryResponse>> GetTimeline(GetTimelineQuery query)
        {
            var skip = query.GetSkipItems;
            var take = query.PageSize;

            var timelineQuery = _dbContext.Tweets
                .AsNoTracking()
                .Join(
                    inner: _dbContext.FollowRelationships.AsNoTracking(),
                    outerKeySelector: tweet => tweet.AuthorId,
                    innerKeySelector: follow => follow.FollowedId,
                    resultSelector: (tweet, follow) => new { tweet, follow }
                )
                .Where(tf => tf.follow.FollowerId == query.UserId);

            var totalCount = await timelineQuery.CountAsync();

            var tweets = await timelineQuery
                .OrderByDescending(tf => tf.tweet.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(tf => new TweetQueryResponse
                {
                    TweetId = tf.tweet.Id,
                    Content = tf.tweet.Content,
                    CreatedAt = tf.tweet.CreatedAt,
                    AuthorId = tf.tweet.AuthorId,
                    AuthorUsername = tf.tweet.Author.Username
                })
                .ToListAsync();

            return new PaginatedQueryResponse<TweetQueryResponse>(
                page: query.Page,
                pageSize: query.PageSize,
                totalCountItems: totalCount,
                items: tweets
            );
        }
    }
}
