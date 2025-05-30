using System.Threading.Tasks;
using twitter.api.application.Models.Follow;
using twitter.api.domain.Models;

namespace twitter.api.application.Services.Abstractions
{
    /// <summary>
    /// Exposes operations related to user follow and unfollow actions.
    /// Encapsulates the creation and removal of follow relationships between users.
    /// </summary>
    public interface IFollowService
    {
        /// <summary>
        /// Starts following a target user, establishing a follow relationship.
        /// </summary>
        /// <param name="command">The command containing follower and followed user identifiers.</param>
        /// <returns>Relationship just created.</returns>
        Task<FollowRelationship> FollowUser(CreateFollowerCommand command);

        /// <summary>
        /// Stops following a user by removing the existing follow relationship.
        /// </summary>
        /// <param name="command">The command containing the involved user identifiers.</param>
        /// <returns></returns>
        Task UnfollowUser(DeleteFollowerCommand command);
    }
}
