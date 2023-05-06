using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IBaseService<D> where D : class
    {
        /// <summary>
        /// Gets object of type T from repository using int identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Instance of type T</returns>
        Task<D> GetAsync(int id);

        /// <summary>
        /// Creates object of type T in a database using repository
        /// </summary>
        /// <param name="modelDto"></param>
        /// <returns>Object of type T</returns>
        Task<D> CreateAsync(D modelDto);

        /// <summary>
        /// Updates object of type D in a database using repository
        /// </summary>
        /// <param name="modelDto"></param>
        /// <returns></returns>
        Task UpdateAsync(D modelDto);

        /// <summary>
        /// Updates partly object of type T (or array) in a database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        Task<D> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument);

        /// <summary>
        /// Deletes object of type T from a database using repository
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
