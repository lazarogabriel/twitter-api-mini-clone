using AutoMapper;
using twitter.api.application.Models.Timeline;
using twitter.api.web.Models.Responses;

namespace twitter.api.web.AutoMapper
{
    public class TweetProfile : Profile
    {
        public TweetProfile()
        {
            CreateMap<TweetQueryResponse, TweetResponse>()
               .ForMember(d => d.Id, o => o.MapFrom(s => s.TweetId))
               .ForMember(d => d.AuthorUsername, o => o.MapFrom(s => s.AuthorUsername))
               .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.AuthorId))
               .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt))
               .ForMember(d => d.Content, o => o.MapFrom(s => s.Content));
        }
    }
}