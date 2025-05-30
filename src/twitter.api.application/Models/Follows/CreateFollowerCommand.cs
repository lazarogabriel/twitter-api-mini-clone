using System;

namespace twitter.api.application.Models.Follow
{
    public class CreateFollowerCommand
    {
        public CreateFollowerCommand(Guid followerId, Guid userToFollowId)
        {
            FollowerId = followerId;
            UserToFollowId = userToFollowId;
        }

        public Guid FollowerId { get; set; }

        public Guid UserToFollowId { get; set; }
    }
}
