using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebApi.Data
{
    public interface IRepository<TModel> where TModel : class
    {
        TModel Create(TModel model);
        Task CreateAsync(TModel model);
        void Delete(TModel model);
        void Delete(int id);
        void DeleteAsync(TModel model);
        TModel Get(int id);
        TModel Get(Expression<Func<TModel, bool>> query);
        Task<TModel> GetAsync(int id);
        IEnumerable<TModel> GetAll();
        TModel Update(TModel model);
        void UpdateAsync(TModel model);
        IEnumerable<TModel> GetAll(Expression<Func<TModel, bool>> query = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null);
        Task<IEnumerable<TModel>> GetAllAsync(Expression<Func<TModel, bool>> query = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null);
    }
}
