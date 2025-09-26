using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Entities.Shared;
using System.Linq.Expressions;

namespace SchoolWebApp.Core.Interfaces.IServices
{
    public interface IGenericService<T> where T : Base
    {
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "");
        Task<PaginatedDto<T>> GetPaginatedData(int pageNumber, int pageSize, string? includeProperties = "");
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id, string includeProperties = "");
        Task<bool> ItemExistsAsync(Expression<Func<T, bool>> expression);
        Task<bool> IsExists<Tvalue>(string key, Tvalue value);
        Task<bool> IsExistsForUpdate<Tid>(Tid id, string key, string value);
        Task<int> RecordCount(Expression<Func<T, bool>> filter = null);

        void Create(T entity);
        void CreateRange(List<T> model);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
    }
}
