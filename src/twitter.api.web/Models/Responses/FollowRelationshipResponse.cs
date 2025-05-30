using System;

namespace twitter.api.web.Models.Responses
{
    public class FollowRelationshipResponse
    {
        public DateTime FollowedAt { get; set; }

        public Guid FollowerId { get; set; }

        public Guid FollowedId { get; set; }
    }
}
