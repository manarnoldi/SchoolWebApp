using SchoolWebApp.Core.Interfaces.IRepositories.School;

namespace SchoolWebApp.Core.Interfaces.IRepositories
{
    public interface IUnitOfWork
    {
        ISchoolDetailsRepository SchoolDetails { get; }

        Task<int> SaveChangesAsync();
    }
}
