using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.School.SchoolDetails;

namespace SchoolWebApp.Core.Interfaces.IServices.School
{
    public interface ISchoolDetailsService
    {
        Task<IEnumerable<SchoolDetailsDto>> GetSchoolDetails();
        Task<PaginatedDto<SchoolDetailsDto>> GetPaginatedSchoolDetails(int pageNumber, int pageSize);
        Task<SchoolDetailsDto> GetSchoolDetail(int id);
        Task<bool> IsExists(string key, string value);
        Task<bool> IsExistsForUpdate(int id, string key, string value);
        Task<SchoolDetailsDto> Create(CreateSchoolDetailsDto model);
        Task Update(SchoolDetailsDto model);
        Task Delete(int id);
    }
}
