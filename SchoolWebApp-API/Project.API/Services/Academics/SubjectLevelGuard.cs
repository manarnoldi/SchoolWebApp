using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;

namespace SchoolWebApp.API.Services.Academics
{
    /// <summary>
    /// Keeps class subjects in line with Edu-Level Subjects: a subject can be
    /// allocated to a teacher, taken by a student or examined in a class only
    /// when it is set for the class's education level (for the class's
    /// academic year), and a level's subject cannot be removed while classes
    /// at that level still use it.
    /// </summary>
    public class SubjectLevelGuard
    {
        private readonly ApplicationDbContext _db;

        public SubjectLevelGuard(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Null when the subject is set for the class's level, else the reason
        /// it is not. When the level has no subjects set for the class's year
        /// at all, any year's list for the level is used.
        /// </summary>
        public async Task<string?> NotSetForClassAsync(int subjectId, int schoolClassId)
        {
            var cls = await _db.SchoolClasses.AsNoTracking().Include(c => c.LearningLevel)
                .FirstOrDefaultAsync(c => c.Id == schoolClassId);
            if (cls?.LearningLevel == null) return "The class does not exist.";
            var levelId = cls.LearningLevel.EducationLevelId;

            var forLevel = _db.EducationLevelSubjects.AsNoTracking().Where(e => e.EducationLevelId == levelId);
            if (await forLevel.AnyAsync(e => e.AcademicYearId == cls.AcademicYearId))
                forLevel = forLevel.Where(e => e.AcademicYearId == cls.AcademicYearId);
            if (await forLevel.AnyAsync(e => e.SubjectId == subjectId)) return null;

            var subject = await _db.Subjects.AsNoTracking().Where(s => s.Id == subjectId).Select(s => s.Name).FirstOrDefaultAsync();
            var level = await _db.EducationLevels.AsNoTracking().Where(l => l.Id == levelId).Select(l => l.Name).FirstOrDefaultAsync();
            return subject == null
                ? "The subject does not exist."
                : $"{subject} is not a subject of {level ?? "this class's education level"}, so it cannot be used in {cls.Name}. "
                  + "Add it to the level under Edu-Level Subjects first if the class really takes it.";
        }

        /// <summary>The same check for a student's subject, by the student's class record.</summary>
        public async Task<string?> NotSetForStudentClassAsync(int subjectId, int studentClassId)
        {
            var classId = await _db.StudentClasses.AsNoTracking().Where(s => s.Id == studentClassId)
                .Select(s => (int?)s.SchoolClassId).FirstOrDefaultAsync();
            return classId == null ? "The student's class does not exist." : await NotSetForClassAsync(subjectId, classId.Value);
        }

        /// <summary>
        /// Null when no class of the level and year uses the subject, else what
        /// still uses it - checked before the subject is taken off the level.
        /// </summary>
        public async Task<string?> InUseAsync(int educationLevelId, int academicYearId, int subjectId)
        {
            var classIds = _db.SchoolClasses.AsNoTracking()
                .Where(c => c.AcademicYearId == academicYearId && c.LearningLevel!.EducationLevelId == educationLevelId)
                .Select(c => c.Id);
            var staff = await _db.StaffSubjects.CountAsync(s => s.SubjectId == subjectId && classIds.Contains(s.SchoolClassId));
            var students = await _db.StudentSubjects.CountAsync(s => s.SubjectId == subjectId && classIds.Contains(s.StudentClass!.SchoolClassId));
            var exams = await _db.Exams.CountAsync(e => e.SubjectId == subjectId && classIds.Contains(e.SchoolClassId));
            if (staff + students + exams == 0) return null;

            var parts = new List<string>();
            if (staff > 0) parts.Add($"{staff} teacher allocation{(staff == 1 ? "" : "s")}");
            if (students > 0) parts.Add($"{students} student subject record{(students == 1 ? "" : "s")}");
            if (exams > 0) parts.Add($"{exams} exam{(exams == 1 ? "" : "s")}");
            return $"The subject is still in use in this level's classes ({string.Join(", ", parts)}). "
                + "Remove those first, or keep the subject on the level.";
        }
    }
}
