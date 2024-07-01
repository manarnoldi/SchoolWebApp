using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Entities.Shared;
using System.Linq.Expressions;

namespace Project.Core.Interfaces.IRepositories
{
    //Unit of Work Pattern
    public interface IBaseRepository<T> where T : Base
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "");
        Task<PaginatedDto<T>> GetPaginatedData(int pageNumber, int pageSize);
        Task<T> GetById(int? id, string includeProperties = "");
        Task<bool> ItemExistsAsync(Expression<Func<T, bool>> expression);
        Task<bool> IsExists<Tvalue>(string key, Tvalue value);
        Task<bool> IsExistsForUpdate<Tid>(Tid id, string key, string value);
        void Create(T model);
        void CreateRange(List<T> model);
        void Update(T model);
        void Delete(T model);
        //Task SaveChangeAsync();
    }
}
