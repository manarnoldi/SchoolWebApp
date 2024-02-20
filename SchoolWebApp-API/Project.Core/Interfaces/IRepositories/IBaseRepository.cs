using SchoolWebApp.Core.DTOs;
using System.Linq.Expressions;

namespace Project.Core.Interfaces.IRepositories
{
    //Unit of Work Pattern
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<PaginatedDto<T>> GetPaginatedData(int pageNumber, int pageSize);
        Task<T> GetById<Tid>(Tid id);
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
