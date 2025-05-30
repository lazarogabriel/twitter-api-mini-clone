using System;

namespace twitter.api.application.Models.Follow
{
    public class CreateTweetCommand
    {
        public CreateTweetCommand(Guid authorId, string content)
        {
            AuthorId = authorId;
            Content = content;
        }

        public Guid AuthorId { get; set; }

        public string Content { get; set; }
    }
}
