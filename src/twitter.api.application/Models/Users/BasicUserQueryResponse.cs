using System;

namespace twitter.api.application.Models.Users
{
    public class BasicUserQueryResponse
    {
        public BasicUserQueryResponse(Guid id, string username)
        {
            Id = id;
            Username = username;
        }

        public Guid Id { get; set; }

        public string Username { get; set; }
    }
}
