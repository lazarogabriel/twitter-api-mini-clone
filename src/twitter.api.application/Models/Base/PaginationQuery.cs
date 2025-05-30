using twitter.api.domain.Constants;
using twitter.api.domain.Exceptions;

namespace twitter.api.application.Models.Base
{
    public class PaginationQuery
    {
        private int _page;
        private int _pageSize;

        public PaginationQuery(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public int Page
        {
            get => _page;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidParameterException(Errors.PageCannotBeZeroOrLess);
                }

                _page = value;
            }
        }


        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidParameterException(Errors.PageSizeCannotBeZeroOrLess);
                }

                _pageSize = value;
            }
        }

        public int GetSkipItems => (Page - 1) * PageSize;
    }
}
