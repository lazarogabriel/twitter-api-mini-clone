using System;
using System.Collections.Generic;

namespace twitter.api.application.Models.Base
{
    public class PaginatedQueryResponse<TItem>
    {
        public PaginatedQueryResponse(int page, int pageSize, int totalCountItems, List<TItem> items)
        {
            Page = page;
            PageSize = pageSize;
            TotalItems = totalCountItems;
            Pages = (int)Math.Ceiling(totalCountItems / (decimal)pageSize);
            Items = items;
        }

        public int Page { get; set; }

        public int Pages { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public List<TItem> Items { get; set; }
    }
}
