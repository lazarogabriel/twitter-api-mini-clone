using System;
using twitter.api.application.Models.Base;

namespace twitter.api.application.Models.Timeline
{
    public class GetTimelineQuery : PaginationQuery
    {
        public GetTimelineQuery(Guid userId, int page, int pageSize) : base(page, pageSize)
        {
            UserId = userId;
        }

        public Guid UserId { get; set; }
    }
}
