-- =============================================================================
-- Link the 10 existing JSS-relevant Subjects to the Junior Secondary
-- EducationLevel for the active AcademicYear.
--
-- Run AGAINST the swikunda MySQL database.
--
-- This script does NOT insert any Subjects - it only creates the
-- EducationLevelSubject linkage rows. The Subject Ids below were identified
-- from the user's existing data:
--
--    Id 15 ENG  English
--    Id  9 KIS  Kiswahili
--    Id 11 MAT  Mathematics
--    Id 20 ISC  Integrated Science (includes Health Education)
--    Id 18 SST  Social Studies
--    Id 12 CRE  Christian Religious Education
--    Id 13 IRE  Islamic Religious Education
--    Id 21 TEC  Pre-Technical Studies (incl. Business & Computer)
--    Id 17 AGR  Agriculture & Nutrition
--    Id 19 ART  Creative Arts (Art/Crafts, Music & PE combined)
--
-- Idempotent: a row is inserted only if no EducationLevelSubject already
-- exists for the same (SubjectId, EducationLevelId, AcademicYearId) triple.
--
-- FK guards: ABORTS with a clear message if the Junior Secondary
-- EducationLevel or the active AcademicYear can't be resolved, instead
-- of silently inserting NULLs.
-- =============================================================================

USE swikunda;
SET SQL_SAFE_UPDATES = 0;

-- ---------------------------------------------------------------------------
-- 1) Resolve the Junior Secondary EducationLevel by Name.
-- ---------------------------------------------------------------------------
SET @jss_edu_level_id := (
    SELECT Id
    FROM   EducationLevels
    WHERE  Name IN ('Junior Secondary','Junior School','JSS','Junior Secondary School')
       OR  Name LIKE '%Junior%'
    ORDER BY
        CASE Name
            WHEN 'Junior Secondary'        THEN 1
            WHEN 'Junior Secondary School' THEN 2
            WHEN 'Junior School'           THEN 3
            WHEN 'JSS'                     THEN 4
            ELSE 5
        END
    LIMIT 1
);

-- ---------------------------------------------------------------------------
-- 2) Resolve the active AcademicYear (prefer Status=1, else latest by Name).
-- ---------------------------------------------------------------------------
SET @active_year_id := (
    SELECT Id
    FROM   AcademicYears
    ORDER BY Status DESC, Name DESC
    LIMIT 1
);

-- ---------------------------------------------------------------------------
-- 3) Pre-flight: show what we resolved + abort if anything is missing.
-- ---------------------------------------------------------------------------
SELECT
    @jss_edu_level_id AS jss_education_level_id,
    @active_year_id   AS active_academic_year_id;

DELIMITER //
DROP PROCEDURE IF EXISTS swikunda.__assert_jss_link_prereqs //
CREATE PROCEDURE swikunda.__assert_jss_link_prereqs(IN edu_id INT, IN year_id INT)
BEGIN
    IF edu_id IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT =
            'No EducationLevel matches Junior Secondary / Junior School / JSS / Junior%. Create it first.';
    END IF;
    IF year_id IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT =
            'No AcademicYear found. Create one (Status=1 for the active year) first.';
    END IF;
END //
DELIMITER ;
CALL swikunda.__assert_jss_link_prereqs(@jss_edu_level_id, @active_year_id);
DROP PROCEDURE swikunda.__assert_jss_link_prereqs;

-- ---------------------------------------------------------------------------
-- 4) Insert one EducationLevelSubject per JSS subject (by explicit Id, so
--    code collisions like ART [Id 10] vs ART [Id 19] don't matter). Skips
--    any triple that already exists.
-- ---------------------------------------------------------------------------
INSERT INTO EducationLevelSubjects
    (Description, EducationLevelId, SubjectId, AcademicYearId, Created, CreatedBy)
SELECT  CONCAT('JSS link: ', s.Code, ' - ', s.Name),
        @jss_edu_level_id,
        s.Id,
        @active_year_id,
        NOW(),
        'jss-link-seed'
FROM    Subjects s
WHERE   s.Id IN (15, 9, 11, 20, 18, 12, 13, 21, 17, 19)
  AND   NOT EXISTS (
            SELECT 1
            FROM   EducationLevelSubjects els
            WHERE  els.SubjectId        = s.Id
              AND  els.EducationLevelId = @jss_edu_level_id
              AND  els.AcademicYearId   = @active_year_id
        );

-- ---------------------------------------------------------------------------
-- Verification
-- ---------------------------------------------------------------------------
SELECT els.Id, s.Id AS SubjectId, s.Code, s.Name AS Subject,
       el.Name AS EducationLevel, ay.Name AS AcademicYear
FROM   EducationLevelSubjects els
JOIN   Subjects        s  ON s.Id  = els.SubjectId
JOIN   EducationLevels el ON el.Id = els.EducationLevelId
JOIN   AcademicYears   ay ON ay.Id = els.AcademicYearId
WHERE  els.EducationLevelId = @jss_edu_level_id
  AND  els.AcademicYearId   = @active_year_id
ORDER BY s.`Rank`, s.Id;

SELECT COUNT(*) AS jss_links_total
FROM   EducationLevelSubjects
WHERE  EducationLevelId = @jss_edu_level_id
  AND  AcademicYearId   = @active_year_id;
