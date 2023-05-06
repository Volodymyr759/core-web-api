using System.Collections.Generic;

namespace CoreWebApi.Library
{
    public class ServiceResult<TModel>: IServiceResult<TModel> where TModel : class
    {
        public IEnumerable<TModel> Items { get; set; }

        public int TotalCount { get; set; }
    }
}
