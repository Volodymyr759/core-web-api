using CoreWebApi.Library;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebApi.Data
{
    public class EFRepository<TModel> : IRepository<TModel> where TModel : class
    {
        private SeerDbContext _context;

        private DbSet<TModel> _set { get; set; }

        public EFRepository(SeerDbContext context)
        {
            _context = context;
            _set = _context.Set<TModel>();
        }

        public async Task<TModel> CreateAsync(TModel model)
        {
            try
            {
                _set.Add(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new CreateEntityFailedException(typeof(TModel), ex);
            }

            return model;
        }

        public async Task DeleteAsync(int id)
        {
            TModel model = await _set.FindAsync(id);
            try
            {
                if (model != null)
                {
                    _set.Remove(model);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new DeleteEntityFailedException(typeof(TModel), ex);
            }
        }

        public async Task<TModel> GetAsync(int id)
        {
            try
            {
                return await _set.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new RetrieveEntityFailedException<TModel>(id.ToString(), ex);
            }
        }

        public async Task<TModel> GetAsync(Expression<Func<TModel, bool>> query = null, params Expression<Func<TModel, object>>[] navigationProperties)
        {
            try
            {
                IQueryable<TModel> dbSet = _set;
                if (query != null) dbSet = dbSet.Where(query);
                foreach (Expression<Func<TModel, object>> property in navigationProperties)
                    dbSet = dbSet.Include<TModel, object>(property);

                return await dbSet.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new RetrieveEntitiesQueryFailedException(typeof(TModel), ex);
            }
        }

        public async Task<IEnumerable<TModel>> GetAsync(string sqlQuery, SqlParameter[] parameters)
        {
            try
            {
                IQueryable<TModel> result = parameters == null ? _set.FromSqlRaw(sqlQuery) :
                    _set.FromSqlRaw(sqlQuery, parameters);
                return await result.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RetrieveEntitiesQueryFailedException(typeof(TModel), ex);
            }
        }

        public async Task<IServiceResult<TModel>> GetAsync(int limit = 0, int page = 1, List<Expression<Func<TModel, bool>>> filters = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null, params Expression<Func<TModel, object>>[] navigationProperties)
        {
            var serviceResult = new ServiceResult<TModel>() { };
            IQueryable<TModel> dbSet = _set;
            try
            {
                if (filters?.Count > 0) foreach (var filter in filters) dbSet = dbSet.Where(filter);
                if (navigationProperties != null)
                {
                    foreach (Expression<Func<TModel, object>> property in navigationProperties)
                        dbSet = dbSet.Include(property);
                }
                if (orderBy != null) dbSet = orderBy(dbSet);
                serviceResult.TotalCount = dbSet.Count();
                if (limit > 0) dbSet = dbSet.Skip((page - 1) * limit).Take(limit);
                serviceResult.Items = await dbSet.ToListAsync();

                return serviceResult;
            }
            catch (Exception ex)
            {
                throw new RetrieveEntitiesQueryFailedException(typeof(TModel), ex);
            }
        }

        public async Task UpdateAsync(TModel model)
        {
            try
            {
                _set.Attach(model);
                _context.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new UpdateEntityFailedException(typeof(TModel), ex);
            }
        }

        public async Task<TModel> SaveAsync(TModel model)
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new UpdateEntityFailedException(typeof(TModel), ex);
            }

            return model;
        }

        public async Task<bool> IsExistAsync(string sqlQuery, SqlParameter[] parameters)
        {
            await _context.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
            var result = parameters[1].Value;// Important! Into each stored procedure keep the output parameter on 2nd place (for example as it was done in 'sp_checkMailSubscriberBySubscriptionIdAndEmail')

            return int.Parse(result.ToString()) > 0;
        }
    }
}
