-- Removes class subject data for subjects that are not set for the class's
-- education level (Edu-Level Subjects, same academic year). These came from
-- subjects being unticked on a level after classes had been allocated them;
-- the API now refuses both (SubjectLevelGuard).
--
-- Run step 1, read it, then step 2. Step 2 deletes only teacher allocations
-- and exams WITHOUT results; anything with student subjects or exam results
-- is listed by step 1 and must be decided by hand.

-- 1. What is out of line, with what hangs off it.
SELECT c.Name AS class_name, s.Name AS subject,
       (SELECT COUNT(*) FROM staffsubjects x WHERE x.SchoolClassId = c.Id AND x.SubjectId = s.Id) AS teacher_allocations,
       (SELECT COUNT(*) FROM studentsubjects x JOIN studentclasses sc ON sc.Id = x.StudentClassId
         WHERE sc.SchoolClassId = c.Id AND x.SubjectId = s.Id) AS student_subjects,
       (SELECT COUNT(*) FROM exams x WHERE x.SchoolClassId = c.Id AND x.SubjectId = s.Id) AS exams,
       (SELECT COUNT(*) FROM examresults r JOIN exams x ON x.Id = r.ExamId
         WHERE x.SchoolClassId = c.Id AND x.SubjectId = s.Id) AS exam_results
FROM schoolclasses c
JOIN learninglevels ll ON ll.Id = c.LearningLevelId
JOIN subjects s ON s.Id IN (
    SELECT SubjectId FROM staffsubjects WHERE SchoolClassId = c.Id
    UNION SELECT x.SubjectId FROM studentsubjects x JOIN studentclasses sc ON sc.Id = x.StudentClassId WHERE sc.SchoolClassId = c.Id
    UNION SELECT SubjectId FROM exams WHERE SchoolClassId = c.Id)
WHERE NOT EXISTS (SELECT 1 FROM educationlevelsubjects e
                  WHERE e.SubjectId = s.Id AND e.EducationLevelId = ll.EducationLevelId AND e.AcademicYearId = c.AcademicYearId)
ORDER BY ll.`Rank`, c.Name, s.Name;

-- 2. Remove the teacher allocations, and the exams that have no results.
START TRANSACTION;

DELETE ss FROM staffsubjects ss
JOIN schoolclasses c ON c.Id = ss.SchoolClassId
JOIN learninglevels ll ON ll.Id = c.LearningLevelId
WHERE NOT EXISTS (SELECT 1 FROM educationlevelsubjects e
                  WHERE e.SubjectId = ss.SubjectId AND e.EducationLevelId = ll.EducationLevelId AND e.AcademicYearId = c.AcademicYearId);

DELETE x FROM exams x
JOIN schoolclasses c ON c.Id = x.SchoolClassId
JOIN learninglevels ll ON ll.Id = c.LearningLevelId
WHERE NOT EXISTS (SELECT 1 FROM educationlevelsubjects e
                  WHERE e.SubjectId = x.SubjectId AND e.EducationLevelId = ll.EducationLevelId AND e.AcademicYearId = c.AcademicYearId)
  AND NOT EXISTS (SELECT 1 FROM examresults r WHERE r.ExamId = x.Id);

COMMIT;
