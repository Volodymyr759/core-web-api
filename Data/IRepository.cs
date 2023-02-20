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

        Task<TModel> CreateAsync(TModel model);

        void Delete(TModel model);

        TModel Delete(int id);

        Task DeleteAsync(TModel model);

        Task DeleteAsync(int id);

        TModel Get(int id);

        Task<TModel> GetAsync(int id);

        /// <summary>
        /// Gets a list of StringValue objects
        /// </summary>
        /// <param name="sqlQuery">String line which containes call to stored procedures</param>
        /// <param name="parameters">Array of params passed to the stored procedure, null or array with 1 or more parameters</param>
        /// <returns>List of StringValue objects like { value: "title1" }</returns>
        Task<IEnumerable<TModel>> GetAsync(string sqlQuery, SqlParameter[] parameters = null);

        IEnumerable<TModel> GetAll();

        Task<IEnumerable<TModel>> GetAllAsync();

        TModel Update(TModel model);

        Task UpdateAsync(TModel model);

        IEnumerable<TModel> GetAll(int limit, int page, Expression<Func<TModel, bool>> query = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null);
        
        Task<IEnumerable<TModel>> GetAllAsync(Expression<Func<TModel, bool>> query = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null);

        /// <summary>
        /// Saves the entity, tracked by EntityFramework
        /// </summary>
        /// <param name="model">The object, which previously got from the db and is on tracking by Entity Framework</param>
        /// <returns>Saved object</returns>
        Task<TModel> SaveAsync(TModel model);

        /// <summary>
        /// Checks in the entity exists, using shot store procedure instead EntityFramework
        /// </summary>
        /// <param name="sqlQuery">SQL query to stored procedure. Includes @id (input) and @returnVal (output) params.</param>
        /// <param name="parameters">Formalized array of SqlParameter's</param>
        /// <remarks>
        /// 
        /// Sample stored procedure:
        ///     CREATE OR ALTER PROCEDURE [dbo].[sp_checkVacancyById]
        ///     @id int,
	    ///     @returnVal int out
        ///         AS
        ///         SELECT @returnVal = COUNT(Id)
        ///         FROM Vacancies
        ///         WHERE Id = @id
        ///         
        ///     RETURN @returnVal
        ///     GO
        ///     
        /// Sample SQL query: "EXEC @returnVal=sp_checkVacancyById @id, @returnVal"
        /// 
        /// Sample array of parameters:
        ///     SqlParameter[] parameters =
        ///         {
        ///             new SqlParameter("@id", SqlDbType.Int) { Value = id },
        ///             new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
        ///         };
        /// 
        /// </remarks>
        /// <returns>True - if exists: output contains more than 0 rows, False - if not</returns>
        Task<bool> IsExistAsync(string sqlQuery, SqlParameter[] parameters);
    }
}
