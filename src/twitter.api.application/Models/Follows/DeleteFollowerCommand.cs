using System;

namespace twitter.api.application.Models.Follow
{
    public class DeleteFollowerCommand
    {
        public DeleteFollowerCommand(Guid unfollowerId, Guid userToUnfollowId)
        {
            UnfollowerId = unfollowerId;
            UserToUnfollowId = userToUnfollowId;
        }

        public Guid UnfollowerId { get; set; }

        public Guid UserToUnfollowId { get; set; }
    }
}
