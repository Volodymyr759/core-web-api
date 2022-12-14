using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task CreateAsync(TModel model)
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

        public async void DeleteAsync(TModel model)
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

        public TModel Get(Expression<Func<TModel, bool>> query)
        {
            try
            {
                return _set.AsNoTracking().FirstOrDefault(query);
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

        public Task<IEnumerable<TModel>> GetAllAsync(Expression<Func<TModel, bool>> query = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null)
        {
            throw new NotImplementedException();
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

        public async void UpdateAsync(TModel model)
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
    }
}
