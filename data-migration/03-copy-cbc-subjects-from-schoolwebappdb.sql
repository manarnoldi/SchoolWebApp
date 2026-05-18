-- =============================================================================
-- Copy the latest CBC Subjects + SubjectGroups from SchoolWebAppDb -> swikunda.
-- Run AGAINST the swikunda database; requires the MySQL user to also have read
-- access to SchoolWebAppDb (root in your config has both).
--
-- What it does:
--   1. Disables FK checks for the duration so we can truncate/insert freely.
--   2. INSERT IGNORE backfills any Curricula and Departments from SchoolWebAppDb
--      that don't yet exist in swikunda (by Id) - so the Subject FKs resolve.
--   3. Wipes swikunda.Subjects and swikunda.SubjectGroups (the legacy 13 + 1).
--   4. Copies SchoolWebAppDb's SubjectGroups and Subjects across, preserving
--      every Id (SubjectGroupId, DepartmentId, CurriculumId) so the FK graph
--      from SchoolWebAppDb stays intact.
--   5. Sets StaffDetailsId = NULL on copied Subjects because staff Ids differ
--      between the two DBs - you can reassign teachers in the UI.
--
-- CAVEAT: any existing rows in swikunda that referenced the OLD Subject Ids
-- (e.g. Strands, BroadOutcomes, StudentSubjects, etc.) will become orphaned.
-- Run the verification query at the bottom to spot any orphans before
-- continuing further.
-- =============================================================================

SET @prev_safe_updates = @@SQL_SAFE_UPDATES;
SET SQL_SAFE_UPDATES = 0;
SET FOREIGN_KEY_CHECKS = 0;

-- ---------------------------------------------------------------------------
-- 1) Backfill missing Curricula (preserve Ids).
-- ---------------------------------------------------------------------------
INSERT IGNORE INTO swikunda.Curricula
    (Id, Code, Name, Description, `Rank`, Created, CreatedBy, Modified, ModifiedBy)
SELECT  Id, Code, Name, Description, `Rank`, Created, CreatedBy, Modified, ModifiedBy
FROM    SchoolWebAppDb.Curricula;

-- ---------------------------------------------------------------------------
-- 2) Backfill missing Departments (preserve Ids) - needed so Subject
--    DepartmentId values from SchoolWebAppDb continue to resolve.
-- ---------------------------------------------------------------------------
INSERT IGNORE INTO swikunda.Departments
    (Id, Name, Code, Description, StaffDetailsId, Created, CreatedBy, Modified, ModifiedBy)
SELECT  Id, Name, Code, Description, NULL, Created, CreatedBy, Modified, ModifiedBy
FROM    SchoolWebAppDb.Departments;

-- ---------------------------------------------------------------------------
-- 3) Wipe swikunda Subjects + SubjectGroups (legacy 13 + 1 derived).
-- ---------------------------------------------------------------------------
TRUNCATE TABLE swikunda.Subjects;
TRUNCATE TABLE swikunda.SubjectGroups;

-- ---------------------------------------------------------------------------
-- 4) Copy SubjectGroups (preserve Id and CurriculumId).
-- ---------------------------------------------------------------------------
INSERT INTO swikunda.SubjectGroups
    (Id, Name, Abbreviation, Description, `Rank`, CurriculumId,
     Created, CreatedBy, Modified, ModifiedBy)
SELECT  Id, Name, Abbreviation, Description, `Rank`, CurriculumId,
        Created, CreatedBy, Modified, ModifiedBy
FROM    SchoolWebAppDb.SubjectGroups;

-- ---------------------------------------------------------------------------
-- 5) Copy Subjects (preserve Id, SubjectGroupId, DepartmentId).
--    StaffDetailsId set to NULL because staff Ids differ between DBs.
-- ---------------------------------------------------------------------------
INSERT INTO swikunda.Subjects
    (Id, Code, Name, Abbr, NumOfLessons, Description, Optional, Examinable, `Rank`,
     SubjectGroupId, DepartmentId, StaffDetailsId,
     Created, CreatedBy, Modified, ModifiedBy)
SELECT  Id, Code, Name, Abbr, NumOfLessons, Description, Optional, Examinable, `Rank`,
        SubjectGroupId, DepartmentId, NULL,
        Created, CreatedBy, Modified, ModifiedBy
FROM    SchoolWebAppDb.Subjects;

SET FOREIGN_KEY_CHECKS = 1;
SET SQL_SAFE_UPDATES = @prev_safe_updates;

-- ---------------------------------------------------------------------------
-- Verification
-- ---------------------------------------------------------------------------
SELECT 'SubjectGroups', COUNT(*) AS rows_ FROM swikunda.SubjectGroups
UNION ALL SELECT 'Subjects',     COUNT(*) FROM swikunda.Subjects
UNION ALL SELECT 'Departments',  COUNT(*) FROM swikunda.Departments
UNION ALL SELECT 'Curricula',    COUNT(*) FROM swikunda.Curricula;

-- Subjects with their group + dept names (first 30 by Rank)
SELECT s.Id, s.Code, s.Name, s.Abbr, sg.Name AS `Group`, d.Name AS Dept
FROM   swikunda.Subjects s
JOIN   swikunda.SubjectGroups sg ON sg.Id = s.SubjectGroupId
JOIN   swikunda.Departments   d  ON d.Id  = s.DepartmentId
ORDER BY s.`Rank`, s.Id
LIMIT  30;

-- Spot orphaned references that used to point at the OLD swikunda Subject Ids.
-- (Should be zero rows if we hadn't created Strands/StudentSubjects against the
-- old Subjects. If any rows return, you'll need to re-create or remap them.)
SELECT 'Strands orphans',         COUNT(*) FROM swikunda.Strands         WHERE SubjectId         NOT IN (SELECT Id FROM swikunda.Subjects)
UNION ALL SELECT 'SubStrands orphans',      COUNT(*) FROM swikunda.SubStrands      WHERE SubjectId         NOT IN (SELECT Id FROM swikunda.Subjects)
UNION ALL SELECT 'BroadOutcomes orphans',   COUNT(*) FROM swikunda.BroadOutcomes   WHERE SubjectId         NOT IN (SELECT Id FROM swikunda.Subjects)
UNION ALL SELECT 'StudentSubjects orphans', COUNT(*) FROM swikunda.StudentSubjects WHERE SubJectId         NOT IN (SELECT Id FROM swikunda.Subjects)
UNION ALL SELECT 'StaffSubjects orphans',   COUNT(*) FROM swikunda.StaffSubjects   WHERE SubjectId         NOT IN (SELECT Id FROM swikunda.Subjects)
UNION ALL SELECT 'LessonAllocation orphans',COUNT(*) FROM swikunda.LessonAllocation  WHERE SubjectId       NOT IN (SELECT Id FROM swikunda.Subjects)
UNION ALL SELECT 'EducationLevelSubjects orphans', COUNT(*) FROM swikunda.EducationLevelSubjects WHERE SubjectId NOT IN (SELECT Id FROM swikunda.Subjects);
