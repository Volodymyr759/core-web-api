using Microsoft.Data.SqlClient;
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
        TModel Delete(int id);
        void DeleteAsync(TModel model);
        TModel Get(int id);

        /// <summary>
        /// Gets a list of  StringValue objects
        /// </summary>
        /// <param name="sqlQuery">String line which containes call to stored procedures</param>
        /// <param name="parameters">Array of params passed to the stored procedure, null or array with 1 or more parameters</param>
        /// <returns>List of StringValue objects like { value: "title1" }</returns>
        Task<IEnumerable<TModel>> GetAsync(string sqlQuery, SqlParameter[] parameters = null);

        Task<TModel> GetAsync(int id);
        IEnumerable<TModel> GetAll();
        Task<IEnumerable<TModel>> GetAllAsync();
        TModel Update(TModel model);
        void UpdateAsync(TModel model);
        IEnumerable<TModel> GetAll(int limit, int page, Expression<Func<TModel, bool>> query = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null);
        Task<IEnumerable<TModel>> GetAllAsync(Expression<Func<TModel, bool>> query = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null);
    }
}
