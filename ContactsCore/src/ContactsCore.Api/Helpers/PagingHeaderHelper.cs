using ContactsCore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ContactsCore.Api.Helpers
{
    public class PagingHeaderHelper
    {
        public static class HeaderKeys
        {
            public const string PageNumber = "x-paging-pagenumber";
            public const string PageSize = "x-paging-pagesize";
            public const string Total = "x-paging-total";
        }

        public void AddHeaders(HttpResponse response, PagingMetadata meta)
        {
            response.Headers.Add(HeaderKeys.PageNumber, new StringValues(meta.PageNumber.ToString()));
            response.Headers.Add(HeaderKeys.PageSize, new StringValues(meta.PageSize.ToString()));
            response.Headers.Add(HeaderKeys.Total, new StringValues(meta.Total.ToString()));
        }
    }
}
