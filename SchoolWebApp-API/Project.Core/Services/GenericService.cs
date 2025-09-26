using Microsoft.EntityFrameworkCore;
using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices;
using System.Linq.Expressions;

namespace SchoolWebApp.Core.Services
{
    public class GenericService<T> : IGenericService<T> where T : Base
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<T> _repository;

        public GenericService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.Repository<T>(); // always resolve repo from UoW
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await _repository.GetAll();
        }
        public virtual async Task<PaginatedDto<T>> GetPaginatedData(int pageNumber, int pageSize, string? includeProperties = "")
        {
            return await _repository.GetPaginatedData(pageNumber, pageSize, includeProperties);
        }

        public virtual async Task<T?> GetById(int id, string includeProperties = "")
        {
            return await _repository.GetById(id, includeProperties);
        }

        public void Create(T entity)
        {
            _repository.Create(entity);
        }

        public void Update(T entity)
        {
            _repository.Update(entity);            
        }
        public void Delete(T entity)
        {
            _repository.Delete(entity);
        }

        public void CreateRange(List<T> model)
        {
            _repository.CreateRange(model);
        }
        public async Task<IEnumerable<T>> Find(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            return await _repository.Find(filter, orderBy, includeProperties);
        }

        public async Task<bool> ItemExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await _repository.ItemExistsAsync(expression);
        }

        public async Task<bool> IsExists<Tvalue>(string key, Tvalue value)
        {
            return await _repository.IsExists(key, value);
        }

        public async Task<bool> IsExistsForUpdate<Tid>(Tid id, string key, string value)
        {
            return await _repository.IsExistsForUpdate(id, key, value);
        }
        public async Task<int> RecordCount(Expression<Func<T, bool>> filter = null)
        {
            return await _repository.RecordCount(filter);
        }

        public async Task SaveChangesAsync()
        {
            await _unitOfWork.SaveChangesAsync();
        }
    }

}
