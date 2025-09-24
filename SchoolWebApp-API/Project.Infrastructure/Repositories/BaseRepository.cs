using Microsoft.EntityFrameworkCore;
using Project.Core.Exceptions;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Entities.Shared;
using System.Collections;
using System.Linq.Expressions;

namespace Project.Infrastructure.Repositories
{
    //Unit of Work Pattern
    public class BaseRepository<T> : IBaseRepository<T> where T : Base
    {
        protected readonly ApplicationDbContext _dbContext;
        protected DbSet<T> DbSet => _dbContext.Set<T>();

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> RecordCount(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.CountAsync();
        }

        public async Task<IEnumerable<T>> Find(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var data = await _dbContext.Set<T>()
                .AsNoTracking()
                .ToListAsync();
            return data;
        }

        public virtual async Task<PaginatedDto<T>> GetPaginatedData(int pageNumber, int pageSize, string includeProperties = "")
        {
            IQueryable<T> query = _dbContext.Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking();
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            var data = await query.ToListAsync();
            var totalCount = await _dbContext.Set<T>().CountAsync();

            return new PaginatedDto<T>(data, totalCount);
        }

        //public async Task<T> GetById<Tid>(Tid id)
        //{
        //    var data = await _dbContext.Set<T>().FindAsync(id);
        //    return data;
        //}
        public async Task<T> GetById(int? id, string includeProperties = "")
        {
            IQueryable<T> query = _dbContext.Set<T>();
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<bool> ItemExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbContext.Set<T>().AnyAsync(expression);
        }

        public async Task<bool> IsExists<Tvalue>(string key, Tvalue value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, key);
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return await _dbContext.Set<T>().AnyAsync(lambda);
        }

        //Before update existence check
        public async Task<bool> IsExistsForUpdate<Tid>(Tid id, string key, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, key);
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(property, constant);

            var idProperty = Expression.Property(parameter, "Id");
            var idEquality = Expression.NotEqual(idProperty, Expression.Constant(id));

            var combinedExpression = Expression.AndAlso(equality, idEquality);
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

            return await _dbContext.Set<T>().AnyAsync(lambda);
        }


        public void Create(T model)
        {
            _dbContext.Set<T>().AddAsync(model);
        }

        public void CreateRange(List<T> model)
        {
            _dbContext.Set<T>().AddRangeAsync(model);
        }

        public void Update(T model)
        {
            _dbContext.Set<T>().Update(model);           
        }

        public void Delete(T model)
        {
            _dbContext.Set<T>().Remove(model);
        }





        //public async Task SaveChangeAsync()
        //{
        //    await _dbContext.SaveChangesAsync();
        //}

    }
}
