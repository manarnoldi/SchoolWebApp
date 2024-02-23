﻿using SchoolWebApp.Core.Interfaces.IRepositories.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;

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

        #region Academics
        IAcademicYearRepository AcademicYears { get; }
        ICurriculumRepository Curricula { get; }
        IExamTypeRepository ExamTypes { get; }
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
