using System;

namespace twitter.api.application.Models.Timeline
{
    public class TweetQueryResponse
    {
        public long TweetId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid AuthorId { get; set; }

        public string AuthorUsername { get; set; }
    }
}
