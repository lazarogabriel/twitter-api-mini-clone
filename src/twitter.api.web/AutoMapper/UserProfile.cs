using AutoMapper;
using twitter.api.application.Models.Users;
using twitter.api.domain.Models;
using twitter.api.web.Models.Responses;

namespace twitter.api.web.AutoMapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<FollowRelationship, FollowRelationshipResponse>()
               .ForMember(d => d.Follower, o => o.MapFrom(s => s.Follower))
               .ForMember(d => d.Followed, o => o.MapFrom(s => s.Followed))
               .ForMember(d => d.FollowedAt, o => o.MapFrom(s => s.FollowedAt));

            CreateMap<User, BasicUserResponse>()
               .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
               .ForMember(d => d.Username, o => o.MapFrom(s => s.Username));

            CreateMap<BasicUserQueryResponse, BasicUserResponse>()
               .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
               .ForMember(d => d.Username, o => o.MapFrom(s => s.Username));
        }
    }
}
