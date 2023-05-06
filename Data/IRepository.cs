using CoreWebApi.Library;
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
        /// <summary>
        /// Creates a new TModel item.
        /// </summary>
        /// <param name="model">TModel instance</param>
        /// <returns>Created TModel object with identifier</returns>
        Task<TModel> CreateAsync(TModel model);

        /// <summary>
        /// Deletes TModel item.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Completed Task</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// Gets a specific TModel Item. In use by AppBaseService.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>TModel instance</returns>
        Task<TModel> GetAsync(int id);

        /// <summary>
        /// Gets a specific TModel Item with Expressions for complex object.
        /// </summary>
        /// <param name="query">Delegate for Where-condition like 'e => e.Id == id'</param>
        /// <param name="navigationProperties">Array of delegates to include navigation properties</param>
        /// <returns>TModel instance</returns>
        Task<TModel> GetAsync(Expression<Func<TModel, bool>> query = null, params Expression<Func<TModel, object>>[] navigationProperties);

        /// <summary>
        /// Gets a list of objects from stored procedures
        /// </summary>
        /// <param name="sqlQuery">Calls stored procedures</param>
        /// <param name="parameters">Array of params passed to the stored procedure, null or array with 1 or more parameters</param>
        /// <returns>List of objects</returns>
        Task<IEnumerable<TModel>> GetAsync(string sqlQuery, SqlParameter[] parameters = null);

        Task<IServiceResult<TModel>> GetAsync(
            int limit = 0,
            int page = 1,
            List<Expression<Func<TModel, bool>>> filters = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            params Expression<Func<TModel, object>>[] navigationProperties);

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

        /// <summary>
        /// Saves the entity, tracked by EntityFramework
        /// </summary>
        /// <param name="model">The object, which previously got from the db and is on tracking by Entity Framework</param>
        /// <returns>Saved object</returns>
        Task<TModel> SaveAsync(TModel model);

        Task UpdateAsync(TModel model);
    }
}
