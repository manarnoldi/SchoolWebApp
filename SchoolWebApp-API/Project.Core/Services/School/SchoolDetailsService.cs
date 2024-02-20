using AutoMapper;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.School.SchoolDetails;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices.School;
using System.Linq.Expressions;

namespace SchoolWebApp.Core.Services.School
{
    public class SchoolDetailsService : ISchoolDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SchoolDetailsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<SchoolDetailsDto>> GetSchoolDetails()
        {
            var schoolDetailsList = await _unitOfWork.SchoolDetails.GetAll();
            var returnSchoolDetailsList = _mapper.Map<List<SchoolDetailsDto>>(schoolDetailsList);
            return returnSchoolDetailsList;
        }

        public async Task<PaginatedDto<SchoolDetailsDto>> GetPaginatedSchoolDetails(int pageNumber, int pageSize)
        {
            var paginatedData = await _unitOfWork.SchoolDetails.GetPaginatedData(pageNumber, pageSize);
            var mappedData = _mapper.Map<List<SchoolDetailsDto>>(paginatedData.Data);
            var paginatedDataViewModel = new PaginatedDto<SchoolDetailsDto>(mappedData.ToList(), paginatedData.TotalCount);
            return paginatedDataViewModel;
        }

        public async Task<SchoolDetailsDto> GetSchoolDetail(int id)
        {
            var _schoolDetail = await _unitOfWork.SchoolDetails.GetById(id);
            var _schoolDetailDto = _mapper.Map<SchoolDetailsDto>(_schoolDetail);
            return _schoolDetailDto;
        }

        public async Task<bool> IsExists(string key, string value)
        {
            return await _unitOfWork.SchoolDetails.IsExists(key, value);
        }

        public async Task<bool> ItemExistsAsync(SchoolDetailsDto model)
        {
            return await _unitOfWork.SchoolDetails.ItemExistsAsync(m => m.Id == model.Id);
        }

        public async Task<bool> IsExistsForUpdate(int id, string key, string value)
        {
            return await _unitOfWork.SchoolDetails.IsExistsForUpdate(id, key, value);
        }

        public async Task<SchoolDetailsDto> Create(CreateSchoolDetailsDto model)
        {
            var _schoolDetail = _mapper.Map<SchoolDetails>(model);
            _unitOfWork.SchoolDetails.Create(_schoolDetail);
            await _unitOfWork.SaveChangesAsync();
            var returnSchoolDetails = _mapper.Map<SchoolDetailsDto>(_schoolDetail);
            return returnSchoolDetails;
        }

        public async Task Update(SchoolDetailsDto model)
        {
            var existingData = await _unitOfWork.SchoolDetails.GetById(model.Id);

            //Manual mapping
            existingData.Name = model.Name;
            existingData.Address = model.Address;
            existingData.SchoolLevelId = model.SchoolLevelId;
            existingData.Telephone = model.Telephone;
            existingData.Email = model.Email;
            existingData.Initials = model.Initials;
            existingData.LogoUrl = model.LogoUrl;
            existingData.Motto = model.Motto;
            existingData.Vision = model.Vision;
            existingData.Website = model.Website;

            _unitOfWork.SchoolDetails.Update(existingData);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _unitOfWork.SchoolDetails.GetById(id);
            _unitOfWork.SchoolDetails.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
