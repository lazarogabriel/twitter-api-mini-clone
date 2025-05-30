using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using twitter.api.application.Models.Follow;
using twitter.api.application.Services.Abstractions;
using twitter.api.data.DbContexts;
using twitter.api.domain.Constants;
using twitter.api.domain.Exceptions;
using twitter.api.domain.Models;

namespace twitter.api.application.Services
{
    public class TweetService : ITweetService
    {
        #region Fields

        private readonly ITwitterApiDbContext _dbContext;

        #endregion

        #region Constructor

        public TweetService(ITwitterApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async Task<Tweet> CreateTweet(CreateTweetCommand command)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == command.AuthorId);

            if (user is null)
            {
                throw new NotFoundException(Errors.UserNotFound);
            }

            var tweet = user.CreateTweet(command.Content);

            _dbContext.Tweets.Add(tweet);
            await _dbContext.CommitAsync();
            
            return tweet;
        }

        #endregion
    }
}
