using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public abstract class AppBaseService<TModel, Dto> where TModel : class where Dto : class
    {
        public IMapper Mapper { get; }

        public IRepository<TModel> Repository { get; set; }

        public ISearchResult<Dto> SearchResult { get; set; }

        public IServiceResult<TModel> ServiceResult { get; set; }

        public AppBaseService(
            IMapper mapper,
            IRepository<TModel> repository,
            ISearchResult<Dto> searchResult,
            IServiceResult<TModel> serviceResult)
        {
            Mapper = mapper;
            Repository = repository;
            SearchResult = searchResult;
            ServiceResult = serviceResult;
        }

        public async Task<ISearchResult<Dto>> Search(
            int limit = 0,
            int page = 1,
            string search = "",
            List<Expression<Func<TModel, bool>>> filters = null,
            OrderType order = OrderType.None,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            params Expression<Func<TModel, object>>[] navigationProperties)
        {
            ServiceResult = await Repository.GetAsync(limit, page, filters, orderBy, navigationProperties ?? null);

            SearchResult.PageSize = limit;
            SearchResult.CurrentPageNumber = page;
            SearchResult.SearchCriteria = search;
            SearchResult.Order = order;
            SearchResult.ItemList = ServiceResult == null ? null : Mapper.Map<IEnumerable<Dto>>(ServiceResult.Items);
            SearchResult.PageCount = ServiceResult == null ? 0 : Convert.ToInt32(Math.Ceiling((double)ServiceResult.TotalCount / limit));
            SearchResult.TotalItemCount = ServiceResult == null ? 0 : (int)(ServiceResult.TotalCount);

            return SearchResult;
        }

        /// <summary>
        /// Checks if object with given id exists in the database. In this realization uses stored procedure for MS SQL.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True or False</returns>
        public abstract Task<bool> IsExistAsync(int id);

        /// <summary>
        /// Gets object from repository using int identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Data transfer object of type D</returns>
        public async Task<Dto> GetAsync(int id) => Mapper.Map<Dto>(await Repository.GetAsync(id));

        /// <summary>
        /// Creates object of type D in a database using repository
        /// </summary>
        /// <param name="modelDto">Data transfer object of type D</param>
        /// <returns>Created Dto object</returns>
        public async Task<Dto> CreateAsync(Dto modelDto)
        {
            var model = Mapper.Map<TModel>(modelDto);

            return Mapper.Map<Dto>(await Repository.CreateAsync(model));
        }

        /// <summary>
        /// Updates object of type D in a database using repository
        /// </summary>
        /// <param name="modelDto"></param>
        /// <returns>Updated Dto object</returns>
        public async Task UpdateAsync(Dto modelDto) => await Repository.UpdateAsync(Mapper.Map<TModel>(modelDto));

        /// <summary>
        /// Deletes object of type D from a database using repository
        /// </summary>
        /// <param name="id"></param>
        /// <returns>void</returns>
        public async Task DeleteAsync(int id) => await Repository.DeleteAsync(id);

        /// <summary>
        /// Updates partly object of type T (or array) in a database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDocument"></param>
        /// <returns>Updated Updated Dto object</returns>
        public async Task<Dto> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument)
        {
            TModel model = await Repository.GetAsync(id);
            patchDocument.ApplyTo(model);
            return Mapper.Map<Dto>(await Repository.SaveAsync(model));
        }
    }
}
