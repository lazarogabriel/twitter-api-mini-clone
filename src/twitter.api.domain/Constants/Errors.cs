namespace twitter.api.domain.Constants
{
    public class Errors
    {
        public const string UsernameIsRequired = "User name is required";
        public const string UsernameMustBeBetween4And15CharsLength = "User name must be between 4 and 15 chars length";
        public const string CannotFollowYourself = "Cannot follow yourself";
        public const string TweetContentIsRequired = "Tweet content is required.";
        public const string TweetContentCannotBeMoreThan280Chars = "Tweet content cannot be more than 280 chars";
        public const string UserNotFound = "User not found";
        public const string FollowerUserNotFound = "Follower user not found";
        public const string UserToFollowNotFound = "User to follow not found";
        public const string AlreadyFollowingUser = "Already following uaser";
        public const string FollowRelationshipNotFound = "Follow relationship not found";
        public const string UserToFollowIsRequired = "User to follow is required";
    }
}
