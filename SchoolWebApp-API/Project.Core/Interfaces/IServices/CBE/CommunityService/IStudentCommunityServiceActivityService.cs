using SchoolWebApp.Core.Entities.CBE.CommunityService;

namespace SchoolWebApp.Core.Interfaces.IServices.CBE.CommunityService
{
    public interface IStudentCommunityServiceActivityService : IGenericService<StudentCommunityServiceActivity>
    {
        Task<List<StudentCommunityServiceActivity>> GetByStudentId(int studentId);
        Task<List<StudentCommunityServiceActivity>> GetByActivityId(int activityId);
    }
}
