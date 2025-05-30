using System.Threading.Tasks;
using twitter.api.application.Models.Follow;
using twitter.api.domain.Models;

namespace twitter.api.application.Services.Abstractions
{
    public interface ITweetService
    {
        /// <summary>
        /// Creates a tweet.
        /// </summary>
        /// <param name="command">The information to create a tweet.</param>
        /// <returns>Tweet just created.</returns>
        Task<Tweet> CreateTweet(CreateTweetCommand command);
    }
}
