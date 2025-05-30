using System;
using twitter.api.domain.Constants;
using twitter.api.domain.Exceptions;

namespace twitter.api.domain.Models
{
    /// <summary>
    /// Represents a short message published by a user.
    /// A Tweet contains up to 280 characters and is always publicly visible.
    /// Each Tweet is associated with exactly one author and a creation timestamp.
    /// </summary>
    public class Tweet
    {
        #region Fields

        private string _content;
        private User _author;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Ef core constructor.
        /// </summary>
        public Tweet()
        {
            
        }

        /// <summary>
        /// Creates a new tweet with a content and an author.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="author"></param>
        public Tweet(string content, User author)
        {
            Content = content;
            Author = author;
            AuthorId = author.Id;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Internal constructor for testing pourpouse.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="author"></param>
        /// <param name="createdAt"></param>
        internal Tweet(string content, User author, DateTime createdAt)
            : this(content, author)
        {
            CreatedAt = createdAt;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Tweet's identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// User author who creates the tweet.
        /// </summary>
        public User Author
        {
            get => _author;
            set
            {
                if (value == null)
                {
                    throw new InvalidParameterException(Errors.TweetAuthorIsRequired);
                }

                _author = value;
            }
        }

        public Guid AuthorId { get; }

        /// <summary>
        /// Tweet's content.
        /// </summary>
        public string Content
        { 
            get => _content; 
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidParameterException(Errors.TweetContentIsRequired);
                }

                var trimmedValue = value.Trim();

                if (value.Length > 208)
                {
                    throw new InvalidParameterException(Errors.TweetContentCannotBeMoreThan280Chars);
                }

                _content = trimmedValue;
            } 
        }

        /// <summary>
        /// The moment when the tweet was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        #endregion
    }
}
