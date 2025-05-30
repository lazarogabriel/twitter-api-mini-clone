using System;
using System.Collections.Generic;
using twitter.api.domain.Constants;
using twitter.api.domain.Exceptions;

namespace twitter.api.domain.Models
{
    /// <summary>
    /// Represents a user in the system with their basic info and relationships.
    /// </summary>
    public class User
    {
        #region Fields

        private string _username;

        #endregion

        #region Constructor

        /// <summary>
        /// Ef core constructor.
        /// </summary>
        public User()
        {
            Followers = new List<FollowRelationship>();
            Following = new List<FollowRelationship>();
            Tweets = new List<Tweet>();
        }

        public User(string username)
        {
            CreatedAt = DateTime.UtcNow;
            Username = username;
            Followers = new List<FollowRelationship>();
            Following = new List<FollowRelationship>();
            Tweets = new List<Tweet>();
        }

        /// <summary>
        /// Domain constructor for testing pourpuses.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        public User(Guid id, string username) : this(username)
        {
            Id = id;
        }

        #endregion

        #region Properties

        /// <summary>
        /// User's identifier.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The moment when the user was created.
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// The user's name.
        /// </summary>
        public string Username
        { 
            get => _username; 
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidParameterException(Errors.UsernameIsRequired);
                }

                var trimmedValue = value.Trim();

                if (trimmedValue.Length < 4 || trimmedValue.Length > 15)
                {
                    throw new InvalidParameterException(Errors.UsernameMustBeBetween4And15CharsLength);
                }

                _username = trimmedValue;
            }
        }

        public int FollowersCount => Followers.Count;

        public List<FollowRelationship> Followers { get; }

        public List<FollowRelationship> Following { get; }

        /// <summary>
        /// Tweets that the user created as an author.
        /// </summary>
        public List<Tweet> Tweets { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a tweet.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Tweet just created.</returns>
        public Tweet CreateTweet(string content)
        {
            var tweet = new Tweet(content: content, author: this);

            Tweets.Add(tweet);

            return tweet;
        }

        /// <summary>
        /// Starts following a user creating a relationship btw.
        /// </summary>
        /// <param name="userToFollow"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public FollowRelationship Follow(User userToFollow)
        {
            if (userToFollow is null)
            {
                throw new InvalidParameterException(Errors.UserToFollowIsRequired);
            }

            if (userToFollow.Id == Id)
            {
                throw new ValidationException(Errors.CannotFollowYourself);
            }

            var followRelationship = new FollowRelationship(follower: this, followed: userToFollow);

            Following.Add(followRelationship);
            userToFollow.Followers.Add(followRelationship);

            return followRelationship;
        }

        /// <summary>
        /// Stop following a user by removing all relationship as this user as a follower
        /// and the followed.
        /// </summary>
        /// <param name="followed"></param>
        /// <param name="relationship"></param>
        public void Unfollow(User followed, FollowRelationship relationship)
        {
            Following.Remove(relationship);
            followed.RemoveFollower(relationship);
        }

        /// <summary>
        /// Removes the relationship as a user being followed.
        /// </summary>
        /// <param name="relationship"></param>
        public void RemoveFollower(FollowRelationship relationship)
        {
            Followers.Remove(relationship);
        }

        #endregion
    }
}
