-- =============================================================================
-- Clean up orphaned rows that point at Person Ids no longer present.
--
-- WHY ORPHANS EXIST
--   The data-migration tool wrapped its student/staff wipe-and-reload in
--     SET FOREIGN_KEY_CHECKS = 0;
--   which bypasses MySQL's referential-integrity guard so the DELETE
--   statements would succeed. That worked for the wipe, but any tables that
--   referenced the deleted rows kept their (now invalid) FK values. We did
--   NOT remove the FK definitions themselves at the schema level - they're
--   still in place via EF, just with stale data on the dependent side.
--
-- WHAT THIS SCRIPT DOES
--   1. Reports the orphan counts so you can see the damage before acting.
--   2. Deletes the orphans table-by-table, in dependency order:
--        ExamResults  -> Student (Person)
--        StudentSubjects, StudentClasses, etc. (extend list below as needed)
--   3. Re-enables FK checks at the end.
--
-- SAFETY
--   Run section 1 (the SELECT diagnostics) first, share the numbers, and only
--   run section 2 once you're happy with what's about to be removed.
-- =============================================================================

USE swikunda;

-- ---------------------------------------------------------------------------
-- Section 1: diagnostic counts. Run these first and inspect.
-- ---------------------------------------------------------------------------
SELECT 'ExamResults orphans (StudentId)' AS what, COUNT(*) AS cnt
FROM   ExamResults er
WHERE  er.StudentId NOT IN (SELECT Id FROM `Person`);

SELECT 'StudentClasses orphans (StudentId)' AS what, COUNT(*) AS cnt
FROM   StudentClasses sc
WHERE  sc.StudentId NOT IN (SELECT Id FROM `Person`);

SELECT 'StudentSubjects orphans (via StudentClass.StudentId)' AS what, COUNT(*) AS cnt
FROM   StudentSubjects ss
JOIN   StudentClasses sc ON sc.Id = ss.StudentClassId
WHERE  sc.StudentId NOT IN (SELECT Id FROM `Person`);

-- Add more diagnostic queries below if you suspect other tables hold dangling
-- references (StudentAttendances, ExamResults via deleted Exams, etc.).

-- ---------------------------------------------------------------------------
-- Section 2: cleanup. Uncomment when ready.
-- ---------------------------------------------------------------------------
-- SET @prev_safe_updates = @@SQL_SAFE_UPDATES;
-- SET SQL_SAFE_UPDATES = 0;
--
-- -- 2a) Exam results pointing at a deleted student.
-- DELETE er
-- FROM   ExamResults er
-- LEFT JOIN `Person` p ON p.Id = er.StudentId
-- WHERE  p.Id IS NULL;
--
-- -- 2b) Student-subject rows whose parent studentClass references a deleted student.
-- DELETE ss
-- FROM   StudentSubjects ss
-- JOIN   StudentClasses sc ON sc.Id = ss.StudentClassId
-- LEFT JOIN `Person` p ON p.Id = sc.StudentId
-- WHERE  p.Id IS NULL;
--
-- -- 2c) The orphan StudentClass rows themselves (parent of 2b - must run AFTER 2b).
-- DELETE sc
-- FROM   StudentClasses sc
-- LEFT JOIN `Person` p ON p.Id = sc.StudentId
-- WHERE  p.Id IS NULL;
--
-- SET SQL_SAFE_UPDATES = @prev_safe_updates;
--
-- -- Re-verify
-- SELECT 'ExamResults orphans AFTER cleanup'   AS what, COUNT(*) AS cnt
-- FROM   ExamResults er
-- WHERE  er.StudentId NOT IN (SELECT Id FROM `Person`);
--
-- SELECT 'StudentClasses orphans AFTER cleanup' AS what, COUNT(*) AS cnt
-- FROM   StudentClasses sc
-- WHERE  sc.StudentId NOT IN (SELECT Id FROM `Person`);
