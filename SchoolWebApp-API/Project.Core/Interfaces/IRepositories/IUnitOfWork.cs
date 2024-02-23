﻿using SchoolWebApp.Core.Interfaces.IRepositories.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories
{
    public interface IUnitOfWork
    {
        #region School
        ISchoolDetailsRepository SchoolDetails { get; }
        IDepartmentsRepository Departments { get; }
        ILearningModesRepository LearningModes { get; }
        IEventRepository Events { get; }
        #endregion

        #region Staff
        IStaffDetailsRepository StaffDetails { get; }
        #endregion

        #region Student
        IParentsRepository Parents { get; }
        IStudentsRepository Students { get; }
        #endregion

        #region Academics
        IAcademicYearRepository AcademicYears { get; }
        ICurriculumRepository Curricula { get; }
        IExamTypeRepository ExamTypes { get; }
        IGradeRepository Grades { get; }
        ISubjectGroupRepository SubjectGroups { get; }
        #endregion

        #region Class
        ISessionRepository Sessions { get; }
        #endregion

        #region Settings
        IDesignationsRepository Designations { get; }
        IEmploymentTypesRepository EmploymentTypes { get; }
        IGenderRepository Genders { get; }
        INationalityRepository Nationalities { get; }
        IOccupationsRepository Occupations { get; }
        IOccurenceTypesRepository OccurenceTypes { get; }
        IOutcomesRepository Outcomes { get; }
        IRelationshipsRepository Relationships { get; }
        IReligionsRepository Religions { get; }
        ISchoolLevelsRepository SchoolLevels { get; }
        ISessionTypesRepository SessionTypes { get; }
        IStaffCategoryRepository StaffCategories { get; }
        #endregion

        Task<int> SaveChangesAsync();
    }
}
