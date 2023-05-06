using System.Collections.Generic;

namespace CoreWebApi.Library
{
    public interface IServiceResult<TModel> where TModel : class
    {
        IEnumerable<TModel> Items { get; set; }

        int TotalCount { get; set; }
    }
}
