namespace Project.DataMigration;

// Holds in-memory old-PK -> new-PK lookup tables that later phases consume.
// Each map is keyed by the source bigint PK and maps to the new MySQL int PK
// produced by EF after SaveChangesAsync.
public sealed class MigrationContext
{
    // Year-scoped phases (SchoolClass, StudentClass, StaffSubject, Student, StaffDetails)
    // restrict source rows to this year - typically the current academic year. Master
    // data phases (Department, Subject, SubjectGroup, SchoolStream) ignore it.
    public int TargetYear { get; init; } = 2026;

    public Dictionary<long, int> AcademicYearMap { get; } = new();   // old tblClasses.year -> AcademicYears.Id
    public Dictionary<string, int> SchoolStreamMap { get; } = new(StringComparer.OrdinalIgnoreCase); // old stream text -> SchoolStreams.Id
    public Dictionary<long, int> SchoolClassMap { get; } = new();    // old tblClasses.classId -> SchoolClasses.Id
    public Dictionary<long, int> DepartmentMap { get; } = new();     // old tblSchoolDepts.deptId -> Departments.Id
    public Dictionary<string, int> SubjectGroupMap { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<long, int> SubjectMap { get; } = new();        // old tblSubjects.subJectId -> Subjects.Id
    public Dictionary<long, int> StaffMap { get; } = new();          // old tblSchoolStaff.empId -> StaffDetails.Id
    public Dictionary<long, int> StudentMap { get; } = new();        // old tblStudDetails.studId -> Students.Id
    public Dictionary<string, int> ReligionMap { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<long, int> ParentMap { get; } = new();
}
