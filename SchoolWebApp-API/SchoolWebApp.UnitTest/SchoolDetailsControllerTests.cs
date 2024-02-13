

using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolWebApp.API.Controllers.School;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.School.SchoolDetails;

namespace SchoolWebApp.UnitTest
{
    public class SchoolDetailsControllerTests
    {
        private Mock<ISchoolDetailsService> _schoolDetailsServiceMock;
        private Mock<ILogger<SchoolDetailsController>> _loggerMock;
        private SchoolDetailsController _schoolDetailsController;

        public SchoolDetailsControllerTests()
        {
            _schoolDetailsServiceMock = new Mock<ISchoolDetailsService>();
            _loggerMock = new Mock<ILogger<SchoolDetailsController>>();
            _schoolDetailsController = new SchoolDetailsController(_loggerMock.Object, _schoolDetailsServiceMock.Object);
        }

        private CreateSchoolDetailsDto GetCreateSchoolDetailsDto(int objectNum)
        {
            return new CreateSchoolDetailsDto()
            {
                Name = "Test name " + objectNum,
                Address = "Test address " + objectNum,
                Telephone = "Test telephone " + objectNum,
                Email = "testemail" + objectNum + "@test.com",
                Motto = "Test motto",
                Vision = "Test vision",
                Initials = "Test initials",
                Website = "testwebsite1@test.com",
                LogoUrl = "https://www.google.com/",
                SchoolLevelId = 1
            };
        }
        private List<SchoolDetailsDto> GetSchoolDetailsList()
        {
            return new List<SchoolDetailsDto>
            {
                new SchoolDetailsDto {
                    Id = 1,
                    Name = "Test name 1",
                    Address = "Test address 1",
                    Telephone = "Test telephone 1",
                    Email = "testemail1@test.com",
                    Motto = "Test motto",
                    Vision = "Test vision",
                    Initials = "Test initials",
                    Website = "testwebsite1@test.com",
                    SchoolLevelId = 1
                },
                new SchoolDetailsDto {
                    Id = 2,
                    Name = "Test name 2",
                    Address = "Test address 2",
                    Telephone = "Test telephone 2",
                    Email = "testemail1@test.com",
                    Motto = "Test motto",
                    Vision = "Test vision",
                    Initials = "Test initials",
                    Website = "testwebsite1@test.com",
                    SchoolLevelId = 1
                }
            };
        }

        [Fact]
        public async Task Get_ReturnsListOfSchoolDetails()
        {
            // Arrange
            _schoolDetailsServiceMock.Setup(service => service.GetSchoolDetails())
                               .ReturnsAsync(GetSchoolDetailsList());

            // Act
            var result = await _schoolDetailsController.Get();
            var resultType = result as OkObjectResult;
            var resultsList = resultType.Value as List<SchoolDetailsDto>;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<SchoolDetailsDto>>(resultType.Value);
            Assert.Equal(2, GetSchoolDetailsList().Count);
        }

        [Fact]
        public async Task GetById_Returns_SchoolDetailsById()
        {
            //Arrange
            int validId = 1;
            int invalidId = 1000;
            int idLessThanZero = 0;

            SchoolDetailsDto schoolDetails = GetSchoolDetailsList()[0];

            _schoolDetailsServiceMock.Setup(service => service.GetSchoolDetail(validId))
                               .ReturnsAsync(schoolDetails);

            //Act
            var errorResult = await _schoolDetailsController.Get(invalidId);
            var errorResultIdLessThanZero = await _schoolDetailsController.Get(idLessThanZero);
            var successResult = await _schoolDetailsController.Get(validId);
            var successModel = successResult as OkObjectResult;
            var fetchedRecord = successModel.Value as SchoolDetailsDto;

            //Assert
            Assert.IsType<OkObjectResult>(successResult);
            Assert.IsType<NotFoundResult>(errorResult);
            Assert.IsType<BadRequestObjectResult>(errorResultIdLessThanZero);
            Assert.Equal(1, fetchedRecord.Id);
        }

        [Fact]
        public async Task GetByPageNumberAndPageSize_ListOfSchoolDetails()
        {
            //Arranging
            int pageSize = 5;
            int pageNumber = 1;

            PaginatedDto<SchoolDetailsDto> paginatedData = new PaginatedDto<SchoolDetailsDto>(GetSchoolDetailsList().ToList(), GetSchoolDetailsList().Count);

            _schoolDetailsServiceMock.Setup(service => service.GetPaginatedSchoolDetails(pageNumber, pageSize))
                              .ReturnsAsync(paginatedData);

            // Act
            var result = await _schoolDetailsController.Get(pageNumber, pageSize);
            var resultType = result as OkObjectResult;
            var resultsList = resultType.Value as List<SchoolDetailsDto>;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PaginatedDto<SchoolDetailsDto>>(resultType.Value);
            Assert.Equal(2, GetSchoolDetailsList().Count);

        }

        [Fact]
        public async Task Create_SchoolDetailsDto_ReturnsCreatedRecord()
        {
            //Arrange
            SchoolDetailsDto createdSchooldetails = GetSchoolDetailsList()[0];
            CreateSchoolDetailsDto toCreate = GetCreateSchoolDetailsDto(1);
            CreateSchoolDetailsDto toCreateNameExists = GetCreateSchoolDetailsDto(2);

            _schoolDetailsServiceMock.Setup(service => service.Create(toCreate))
                              .ReturnsAsync(createdSchooldetails);
            _schoolDetailsServiceMock.Setup(service => service.IsExists("Email", "testemail2@test.com"))
                              .ReturnsAsync(true);
            _schoolDetailsServiceMock.Setup(service => service.IsExists("Name", "Test name 2"))
                              .ReturnsAsync(true);

            //Act
            var result = await _schoolDetailsController.Create(toCreate);
            var resultEmailExists = await _schoolDetailsController.Create(toCreateNameExists);
            var resultNameExists = await _schoolDetailsController.Create(toCreateNameExists);


            var resultType = result as OkObjectResult;
            var resultObject = resultType.Value as SchoolDetailsDto;

            var resultTypeEmailExists = resultEmailExists as ConflictObjectResult;
            var resultTypeNameExists = resultNameExists as ConflictObjectResult;

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SchoolDetailsDto>(resultType.Value);
            Assert.Equal("Test name 1", resultObject.Name);
            Assert.IsType<ConflictObjectResult>(resultEmailExists);
            Assert.IsType<ConflictObjectResult>(resultNameExists);

        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            SchoolDetailsDto createdSchooldetails = GetSchoolDetailsList()[0];
            CreateSchoolDetailsDto toCreate = GetCreateSchoolDetailsDto(1);
            toCreate.Name = null;
            _schoolDetailsServiceMock.Setup(service => service.Create(toCreate))
                              .ReturnsAsync(createdSchooldetails);
            _schoolDetailsController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _schoolDetailsController.Create(toCreate);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Update_SchoolDetailsDto_ReturnsOkObjectResult()
        {
            //Arrange
            SchoolDetailsDto toUpdate = GetSchoolDetailsList()[0];
            toUpdate.Name = "Updated Name";

            //CreateSchoolDetailsDto toCreateNameExists = GetCreateSchoolDetailsDto(2);

            _schoolDetailsServiceMock.Setup(service => service.Update(toUpdate))
                              .Returns(Task.CompletedTask);
            //_schoolDetailsServiceMock.Setup(service => service.IsExists("Id", "1"))
            //                  .ReturnsAsync(true);

            //Act
            var result = await _schoolDetailsController.Edit(toUpdate);
            //var resultEmailExists = await _schoolDetailsController.Create(toCreateNameExists);
            //var resultNameExists = await _schoolDetailsController.Create(toCreateNameExists);


            var resultType = result as OkObjectResult;
            //var resultObject = resultType.Value as SchoolDetailsDto;

            //var resultTypeEmailExists = resultEmailExists as ConflictObjectResult;
            //var resultTypeNameExists = resultNameExists as ConflictObjectResult;

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            //Assert.IsType<SchoolDetailsDto>(resultType.Value);
            //Assert.Equal("Test name 1", resultObject.Name);
            //Assert.IsType<ConflictObjectResult>(resultEmailExists);
            //Assert.IsType<ConflictObjectResult>(resultNameExists);

        }

        
    }
}