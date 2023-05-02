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

        public TModel Create(TModel model)
        {
            try
            {
                _set.Add(model);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new CreateEntityFailedException(typeof(TModel), ex);
            }

            return model;
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

        public void Delete(TModel model)
        {
            try
            {
                _set.Remove(model);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DeleteEntityFailedException(typeof(TModel), ex);
            }
        }

        public TModel Delete(int id)
        {
            TModel model = _set.Find(id);
            try
            {
                if (model != null)
                {
                    _set.Remove(model);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new DeleteEntityFailedException(typeof(TModel), ex);
            }

            return model;
        }

        public async Task DeleteAsync(TModel model)
        {
            try
            {
                _set.Remove(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DeleteEntityFailedException(typeof(TModel), ex);
            }
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

        public TModel Get(int id)
        {
            try
            {
                return _set.Find(id);
            }
            catch (Exception ex)
            {
                throw new RetrieveEntityFailedException<TModel>(id.ToString(), ex);
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

        public async Task<TModel> GetAsync(Expression<Func<TModel, bool>> query = null, Expression<Func<TModel, object>> include = null)
        {
            try
            {
                IQueryable<TModel> dbSet = _set;
                if (query != null) dbSet = dbSet.Where(query);
                if (include != null) dbSet = dbSet.Include(include);

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

        public IEnumerable<TModel> GetAll()
        {
            try
            {
                return _set.AsNoTracking().ToList();
            }
            catch (Exception ex)
            {
                throw new RetrieveEntitiesQueryFailedException(typeof(TModel), ex);
            }
        }

        public IEnumerable<TModel> GetAll(
            int limit,
            int page,
            Expression<Func<TModel, bool>> query = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null)
        {
            try
            {
                IQueryable<TModel> dbSet = _set;

                if (query != null) dbSet = dbSet.Where(query);
                if (orderBy != null) dbSet = orderBy(dbSet);
                dbSet = dbSet.Skip((page - 1) * limit).Take(limit);

                return dbSet.ToList();
            }
            catch (Exception ex)
            {
                throw new RetrieveEntitiesQueryFailedException(typeof(TModel), ex);
            }
        }

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            try
            {
                IQueryable<TModel> dbSet = _set;
                return await dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RetrieveEntitiesQueryFailedException(typeof(TModel), ex);
            }
        }

        public async Task<IEnumerable<TModel>> GetAllAsync(Expression<Func<TModel, bool>> query = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null)
        {
            try
            {
                IQueryable<TModel> dbSet = _set;

                if (query != null) dbSet = dbSet.Where(query);
                if (orderBy != null) dbSet = orderBy(dbSet);

                return await dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RetrieveEntitiesQueryFailedException(typeof(TModel), ex);
            }
        }

        public async Task<IEnumerable<TModel>> GetAllAsync(
            Expression<Func<TModel, bool>> query = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            Expression<Func<TModel, object>> include = null)
        {
            try
            {
                IQueryable<TModel> dbSet = _set;

                if (query != null) dbSet = dbSet.Where(query);
                if (orderBy != null) dbSet = orderBy(dbSet);
                if (include != null) dbSet = dbSet.Include(include);

                return await dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RetrieveEntitiesQueryFailedException(typeof(TModel), ex);
            }
        }

        public TModel Update(TModel model)
        {
            try
            {
                _set.Attach(model);
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UpdateEntityFailedException(typeof(TModel), ex);
            }

            return model;
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
