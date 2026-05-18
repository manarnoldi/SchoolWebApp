using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Finance;
using SchoolWebApp.Core.Interfaces.IRepositories.Payroll;

namespace SchoolWebApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        #region Academics
        public IAcademicYearRepository AcademicYears { get; }
        public ICurriculumRepository Curricula { get; }
        public IExamTypeRepository ExamTypes { get; }
        public IExamRepository Exams { get; }
        public IExamResultRepository ExamResults { get; }
        public IGradeRepository Grades { get; }
        public ISubjectGroupRepository SubjectGroups { get; }
        public ISubjectRepository Subjects { get; }
        public IEducationLevelSubjectRepository EducationLevelSubjects { get; }
        #endregion

        #region School
        public ISchoolDetailsRepository SchoolDetails { get; }
        public IDepartmentsRepository Departments { get; }
        public ILearningModesRepository LearningModes { get; }
        public IEventRepository Events { get; }
        public IEducationLevelTypesRepository EducationLevelTypes { get; }
        public IEducationLevelRepository EducationLevels { get; }
        public ISchoolStreamsRepository SchoolStreams { get; }
        public IToDoListRepository ToDoLists { get; }
        #endregion

        #region Staff
        public IStaffDetailsRepository StaffDetails { get; }
        public IStaffAttendanceRepository StaffAttendances { get; }
        public IStaffDisciplineRepository StaffDisciplines { get; }
        public IStaffSubjectRepository StaffSubjects { get; }
        #endregion

        #region Student
        public IParentsRepository Parents { get; }
        public IFormerSchoolsRepository FormerSchools { get; }
        public IStudentsRepository Students { get; }
        public IStudentDisciplineRepository StudentDisciplines { get; }
        public IStudentParentRepository StudentParent { get; }
        public IStudentAttendanceRepository StudentAttendances { get; }
        public IStudentClassRepository StudentClasses { get; }
        public IStudentSubjectRepository StudentSubjects { get; }
        #endregion

        #region Class
        public ISessionRepository Sessions { get; }
        public ILearningLevelRepository LearningLevels { get; }
        public ISchoolClassRepository SchoolClasses { get; }
        public ISchoolClassLeadersRepository SchoolClassLeaders { get; }
        public IClassLeadershipRoleRepository ClassLeadershipRoles { get; }
        #endregion

        #region Settings
        public IDesignationsRepository Designations { get; }
        public IEmploymentTypesRepository EmploymentTypes { get; }
        public IGenderRepository Genders { get; }
        public INationalityRepository Nationalities { get; }
        public IOccupationsRepository Occupations { get; }
        public IOccurenceTypesRepository OccurenceTypes { get; }
        public IOutcomesRepository Outcomes { get; }
        public IRelationshipsRepository Relationships { get; }
        public IReligionsRepository Religions { get; }
        public ISessionTypesRepository SessionTypes { get; }
        public IStaffCategoryRepository StaffCategories { get; }
        #endregion

        #region Finance
        public IAccountRepository Accounts { get; }
        public IFeeCategoryRepository FeeCategories { get; }
        public IFeeStructureRepository FeeStructures { get; }
        public IFeeStructureItemRepository FeeStructureItems { get; }
        public IStudentInvoiceRepository StudentInvoices { get; }
        public IStudentInvoiceItemRepository StudentInvoiceItems { get; }
        public IPaymentRepository Payments { get; }
        public IExpenseCategoryRepository ExpenseCategories { get; }
        public IExpenseRepository Expenses { get; }
        public IExpenseLineRepository ExpenseLines { get; }
        public IJournalEntryRepository JournalEntries { get; }
        public IJournalLineRepository JournalLines { get; }
        public IBudgetRepository Budgets { get; }
        public IBudgetLineRepository BudgetLines { get; }
        public IBudgetMasterRepository BudgetMasters { get; }
        public IBudgetAmendmentRepository BudgetAmendments { get; }
        public IBudgetAmendmentLineRepository BudgetAmendmentLines { get; }
        #endregion

        #region Payroll
        public IEarningTypeRepository EarningTypes { get; }
        public IDeductionTypeRepository DeductionTypes { get; }
        public ITaxBandRepository TaxBands { get; }
        public IPayrollSettingRepository PayrollSettings { get; }
        public IEmployeeSalaryRepository EmployeeSalaries { get; }
        public IEmployeeSalaryItemRepository EmployeeSalaryItems { get; }
        public ILoanAdvanceRepository LoanAdvances { get; }
        public IPayrollPeriodRepository PayrollPeriods { get; }
        public IPayslipRepository Payslips { get; }
        public IPayslipEarningRepository PayslipEarnings { get; }
        public IPayslipDeductionRepository PayslipDeductions { get; }
        #endregion

        public UnitOfWork(ApplicationDbContext context,
                //Academics
                IAcademicYearRepository academicYearRepository,
                ICurriculumRepository curriculumRepository,
                IExamTypeRepository examTypeRepository,
                IExamRepository examRepository,
                IExamResultRepository examResultsRepository,
                IGradeRepository gradeRepository,
                ISubjectGroupRepository subjectGroupRepository,
                ISubjectRepository subjectRepository,
                IEducationLevelSubjectRepository educationLevelSubjectRepository,

                //School
                ISchoolDetailsRepository schoolDetailsRepository,
                IDepartmentsRepository departmentsRepository,
                ILearningModesRepository learningModesRepository,
                IEventRepository eventRepository,
                IEducationLevelTypesRepository educationLevelTypesRepository,
                IEducationLevelRepository educationLevelsRepository,
                ISchoolStreamsRepository schoolStreamsRepository,
                IToDoListRepository toDoListRepository,

                //Staff
                IStaffDetailsRepository staffDetailsRepository,
                IStaffAttendanceRepository staffAttendancesRepository,
                IStaffDisciplineRepository staffDisciplinesRepository,
                IStaffSubjectRepository staffSubjectRepository,

                //Student
                IParentsRepository parentsRepository,
                IFormerSchoolsRepository formerSchoolsRepository,
                IStudentsRepository studentsRepository,
                IStudentParentRepository studentParentRepository,
                IStudentDisciplineRepository studentDisciplineRepository,
                IStudentAttendanceRepository studentAttendanceRepository,
                IStudentClassRepository studentClassRepository,
                IStudentSubjectRepository studentSubjectRepository,

                //Class
                ISessionRepository sessionRepository,
                ILearningLevelRepository learningLevelRepository,
                ISchoolClassRepository schoolClassRepository,
                ISchoolClassLeadersRepository schoolClassLeadersRepository,
                IClassLeadershipRoleRepository classLeadershipRoleRepository,

                //Settings
                IDesignationsRepository designationsRepository,
                IEmploymentTypesRepository employmentTypesRepository,
                IGenderRepository genderRepository,
                INationalityRepository nationalityRepository,
                IOccupationsRepository occupationsRepository,
                IOccurenceTypesRepository occurenceTypesRepository,
                IOutcomesRepository outcomesRepository,
                IRelationshipsRepository relationshipsRepository,
                IReligionsRepository religionsRepository,
                ISessionTypesRepository sessionTypesRepository,
                IStaffCategoryRepository staffCategoryRepository,

                //Finance
                IAccountRepository accountRepository,
                IFeeCategoryRepository feeCategoryRepository,
                IFeeStructureRepository feeStructureRepository,
                IFeeStructureItemRepository feeStructureItemRepository,
                IStudentInvoiceRepository studentInvoiceRepository,
                IStudentInvoiceItemRepository studentInvoiceItemRepository,
                IPaymentRepository paymentRepository,
                IExpenseCategoryRepository expenseCategoryRepository,
                IExpenseRepository expenseRepository,
                IExpenseLineRepository expenseLineRepository,
                IJournalEntryRepository journalEntryRepository,
                IJournalLineRepository journalLineRepository,
                IBudgetRepository budgetRepository,
                IBudgetLineRepository budgetLineRepository,
                IBudgetMasterRepository budgetMasterRepository,
                IBudgetAmendmentRepository budgetAmendmentRepository,
                IBudgetAmendmentLineRepository budgetAmendmentLineRepository,

                //Payroll
                IEarningTypeRepository earningTypeRepository,
                IDeductionTypeRepository deductionTypeRepository,
                ITaxBandRepository taxBandRepository,
                IPayrollSettingRepository payrollSettingRepository,
                IEmployeeSalaryRepository employeeSalaryRepository,
                IEmployeeSalaryItemRepository employeeSalaryItemRepository,
                ILoanAdvanceRepository loanAdvanceRepository,
                IPayrollPeriodRepository payrollPeriodRepository,
                IPayslipRepository payslipRepository,
                IPayslipEarningRepository payslipEarningRepository,
                IPayslipDeductionRepository payslipDeductionRepository
         )
        {
            _context = context;
            #region Academics
            AcademicYears = academicYearRepository;
            Curricula = curriculumRepository;
            ExamTypes = examTypeRepository;
            ExamResults = examResultsRepository;
            Exams = examRepository;
            Grades = gradeRepository;
            SubjectGroups = subjectGroupRepository;
            Subjects = subjectRepository;
            EducationLevelSubjects = educationLevelSubjectRepository;
            #endregion

            #region School
            SchoolDetails = schoolDetailsRepository;
            Departments = departmentsRepository;
            LearningModes = learningModesRepository;
            Events = eventRepository;
            EducationLevelTypes = educationLevelTypesRepository;
            EducationLevels = educationLevelsRepository;
            SchoolStreams = schoolStreamsRepository;
            ToDoLists = toDoListRepository;
            #endregion

            #region Staff
            StaffDetails = staffDetailsRepository;
            StaffAttendances = staffAttendancesRepository;
            StaffDisciplines = staffDisciplinesRepository;
            StaffSubjects = staffSubjectRepository;
            #endregion

            #region Student
            Parents = parentsRepository;
            FormerSchools = formerSchoolsRepository;
            Students = studentsRepository;
            StudentParent = studentParentRepository;
            StudentDisciplines = studentDisciplineRepository;
            StudentAttendances = studentAttendanceRepository;
            StudentClasses = studentClassRepository;
            StudentSubjects = studentSubjectRepository;
            #endregion

            #region Class
            Sessions = sessionRepository;
            LearningLevels = learningLevelRepository;
            SchoolClasses = schoolClassRepository;
            SchoolClassLeaders = schoolClassLeadersRepository;
            ClassLeadershipRoles = classLeadershipRoleRepository;
            #endregion

            #region Settings
            Designations = designationsRepository;
            EmploymentTypes = employmentTypesRepository;
            Genders = genderRepository;
            Nationalities = nationalityRepository;
            Occupations = occupationsRepository;
            OccurenceTypes = occurenceTypesRepository;
            Outcomes = outcomesRepository;
            Relationships = relationshipsRepository;
            Religions = religionsRepository;
            SessionTypes = sessionTypesRepository;
            StaffCategories = staffCategoryRepository;
            #endregion

            #region Finance
            Accounts = accountRepository;
            FeeCategories = feeCategoryRepository;
            FeeStructures = feeStructureRepository;
            FeeStructureItems = feeStructureItemRepository;
            StudentInvoices = studentInvoiceRepository;
            StudentInvoiceItems = studentInvoiceItemRepository;
            Payments = paymentRepository;
            ExpenseCategories = expenseCategoryRepository;
            Expenses = expenseRepository;
            ExpenseLines = expenseLineRepository;
            JournalEntries = journalEntryRepository;
            JournalLines = journalLineRepository;
            Budgets = budgetRepository;
            BudgetLines = budgetLineRepository;
            BudgetMasters = budgetMasterRepository;
            BudgetAmendments = budgetAmendmentRepository;
            BudgetAmendmentLines = budgetAmendmentLineRepository;
            #endregion

            #region Payroll
            EarningTypes = earningTypeRepository;
            DeductionTypes = deductionTypeRepository;
            TaxBands = taxBandRepository;
            PayrollSettings = payrollSettingRepository;
            EmployeeSalaries = employeeSalaryRepository;
            EmployeeSalaryItems = employeeSalaryItemRepository;
            LoanAdvances = loanAdvanceRepository;
            PayrollPeriods = payrollPeriodRepository;
            Payslips = payslipRepository;
            PayslipEarnings = payslipEarningRepository;
            PayslipDeductions = payslipDeductionRepository;
            #endregion
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Generic repo accessor
        public IBaseRepository<T> Repository<T>() where T : Base
        {
            if (!_repositories.ContainsKey(typeof(T)))
            {
                var repo = new BaseRepository<T>(_context);
                _repositories.Add(typeof(T), repo);
            }

            return (IBaseRepository<T>)_repositories[typeof(T)];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
