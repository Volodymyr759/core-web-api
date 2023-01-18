using CoreWebApi.Library.Enums;
using System.Collections.Generic;

namespace CoreWebApi.Library.SearchResult
{
    public class SearchResult<T>
    {
        public List<T> ItemList { get; set; }

        public int CurrentPageNumber { get; set; }

        public OrderType? Order { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public string SearchCriteria { get; set; }

        public int TotalItemCount { get; set; }
    }
}
