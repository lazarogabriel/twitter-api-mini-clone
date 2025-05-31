using System;

namespace twitter.api.web.Models.Responses
{
    public class FollowRelationshipResponse
    {
        public BasicUserResponse Follower { get; set; }

        public BasicUserResponse Followed { get; set; }

        public DateTime FollowedAt { get; set; }
    }
}
