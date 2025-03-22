using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories.School;

namespace SchoolWebApp.Infrastructure.Repositories.School
{
    public class ToDoListRepository : BaseRepository<ToDoList>, IToDoListRepository
    {
        public ToDoListRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<ToDoList>> GetByStaffId(int staffId)
        {
            var toDoLists = await _dbContext.ToDoLists.Where(e => e.StaffDetailsId == staffId && e.Completed == false)
                .Include(s => s.StaffDetails)
                .ToListAsync();
            return toDoLists;
        }
    }
}