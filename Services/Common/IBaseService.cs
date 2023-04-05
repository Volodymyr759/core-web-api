using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IBaseService<T> where T : class
    {
        /// <summary>
        /// Gets object of type T from repository using int identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Instance of type T</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Creates object of type T in a database using repository
        /// </summary>
        /// <param name="modelDto"></param>
        /// <returns>Object of type T</returns>
        Task<T> CreateAsync(T modelDto);

        /// <summary>
        /// Updates object of type T in a database using repository
        /// </summary>
        /// <param name="modelDto"></param>
        /// <returns></returns>
        Task UpdateAsync(T modelDto);

        /// <summary>
        /// Updates partly object of type T (or array) in a database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        Task<T> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument);

        /// <summary>
        /// Deletes object of type T in a database from repository
        /// </summary>
        /// <param name="id"></param>
        /// <returns>void</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// Checks if object with given id exists in the database. In this realization uses stored procedure for MS SQL.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True or False</returns>
        Task<bool> IsExistAsync(int id);
    }
}
