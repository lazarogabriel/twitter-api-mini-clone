using System.Collections.Generic;

namespace twitter.api.web.Models.Responses.Base
{
    public class PaginatedResponse<TItems>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int Pages { get; set; }

        public int TotalItems { get; set; }

        public IEnumerable<TItems> Items { get; set; }
    }
}
