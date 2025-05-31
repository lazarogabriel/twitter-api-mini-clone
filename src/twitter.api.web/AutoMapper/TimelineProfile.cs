using AutoMapper;
using twitter.api.web.Models.Responses.Base;
using twitter.api.web.Models.Responses;
using twitter.api.application.Models.Base;
using twitter.api.application.Models.Timeline;

namespace twitter.api.web.AutoMapper
{
    public class TimelineProfile : Profile
    {
        public TimelineProfile()
        {
            CreateMap<PaginatedQueryResponse<TweetQueryResponse>, PaginatedResponse<TweetResponse>>()
               .ForMember(d => d.Page, o => o.MapFrom(s => s.Page))
               .ForMember(d => d.Pages, o => o.MapFrom(s => s.Pages))
               .ForMember(d => d.PageSize, o => o.MapFrom(s => s.PageSize))
               .ForMember(d => d.TotalItems, o => o.MapFrom(s => s.TotalItems))
               .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));
        }
    }
}
