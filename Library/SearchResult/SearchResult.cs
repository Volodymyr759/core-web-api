using System.Collections.Generic;

namespace CoreWebApi.Library
{
    public class SearchResult<TModel> : ISearchResult<TModel> where TModel : class
    {
        public int CurrentPageNumber { get; set; }

        public IEnumerable<TModel> ItemList { get; set; }

        public OrderType? Order { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public string SearchCriteria { get; set; }

        public int TotalItemCount { get; set; }
    }
}
