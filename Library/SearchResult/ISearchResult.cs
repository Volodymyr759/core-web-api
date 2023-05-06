using System.Collections.Generic;

namespace CoreWebApi.Library
{
    public interface ISearchResult<TModel> where TModel : class
    {
        int CurrentPageNumber { get; set; }

        IEnumerable<TModel> ItemList { get; set; }

        OrderType? Order { get; set; }

        int PageCount { get; set; }

        int PageSize { get; set; }

        string SearchCriteria { get; set; }

        int TotalItemCount { get; set; }
    }
}