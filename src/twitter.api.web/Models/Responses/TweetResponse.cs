using System;

namespace twitter.api.web.Models.Responses
{
    public class TweetResponse
    {
        public long Id { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid AuthorId { get; set; }

        public string AuthorUsername { get; set; }
    }
}
