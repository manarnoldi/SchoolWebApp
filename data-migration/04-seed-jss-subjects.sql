-- =============================================================================
-- Junior School (Grade 7-9) Learning Areas per KICD revised CBC curriculum.
-- Run AGAINST the swikunda MySQL database.
--
-- Source: KICD Junior School Curriculum Design (2022, revised 2023+) - the
-- 9 core learning areas every JSS learner studies. Optional foreign-language
-- electives are at the bottom (commented out by default).
--
-- Idempotent: each INSERT uses WHERE NOT EXISTS on the Code column, so the
-- script can be re-run without creating duplicates.
--
-- FK resolution: SubjectGroupId resolves by best-effort Name match against
-- whichever SubjectGroups already exist in your DB. DepartmentId defaults to
-- 'General' if present, else any department. Reassign per subject in the UI
-- afterwards if you want a tighter departmental split.
-- =============================================================================

USE swikunda;
SET SQL_SAFE_UPDATES = 0;

-- ---------------------------------------------------------------------------
-- Helpers: resolve a SubjectGroupId by candidate names; fall back to MIN(Id).
-- ---------------------------------------------------------------------------
-- Pick one default department for new subjects.
SET @def_dept_id := (
    SELECT COALESCE(
        (SELECT Id FROM Departments WHERE Name = 'General' LIMIT 1),
        (SELECT MIN(Id) FROM Departments)
    )
);

-- Default fallback subject group (used if a more specific name match fails).
SET @def_group_id := (SELECT MIN(Id) FROM SubjectGroups);

-- ---------------------------------------------------------------------------
-- The 9 JSS core learning areas
-- ---------------------------------------------------------------------------
-- 1. English
INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, Examinable, `Rank`,
                      SubjectGroupId, DepartmentId, Created, CreatedBy)
SELECT 'ENG', 'English', 'ENG', 5, 'JSS English Language (Grade 7-9)', 0, 1, 10,
       COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Languages', 'Language') LIMIT 1), @def_group_id),
       @def_dept_id, NOW(), 'jss-seed'
WHERE NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = 'ENG');

-- 2. Kiswahili (or KSL for hearing impaired learners)
INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, Examinable, `Rank`,
                      SubjectGroupId, DepartmentId, Created, CreatedBy)
SELECT 'KIS', 'Kiswahili', 'KIS', 4, 'JSS Kiswahili (or KSL alternative) (Grade 7-9)', 0, 1, 20,
       COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Languages', 'Language') LIMIT 1), @def_group_id),
       @def_dept_id, NOW(), 'jss-seed'
WHERE NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = 'KIS');

-- 3. Mathematics
INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, Examinable, `Rank`,
                      SubjectGroupId, DepartmentId, Created, CreatedBy)
SELECT 'MAT', 'Mathematics', 'MAT', 5, 'JSS Mathematics (Grade 7-9)', 0, 1, 30,
       COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Mathematics', 'Maths') LIMIT 1), @def_group_id),
       @def_dept_id, NOW(), 'jss-seed'
WHERE NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = 'MAT');

-- 4. Integrated Science (subsumes the former Health Education)
INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, Examinable, `Rank`,
                      SubjectGroupId, DepartmentId, Created, CreatedBy)
SELECT 'SCI', 'Integrated Science', 'SCI', 5, 'JSS Integrated Science incl. Health Education (Grade 7-9)', 0, 1, 40,
       COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Sciences', 'Science') LIMIT 1), @def_group_id),
       @def_dept_id, NOW(), 'jss-seed'
WHERE NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = 'SCI');

-- 5. Social Studies
INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, Examinable, `Rank`,
                      SubjectGroupId, DepartmentId, Created, CreatedBy)
SELECT 'SST', 'Social Studies', 'SST', 3, 'JSS Social Studies (Grade 7-9)', 0, 1, 50,
       COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Humanities and Social Sciences', 'Humanities', 'Social Sciences') LIMIT 1), @def_group_id),
       @def_dept_id, NOW(), 'jss-seed'
WHERE NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = 'SST');

-- 6. Religious Education (learners pick CRE / IRE / HRE)
INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, Examinable, `Rank`,
                      SubjectGroupId, DepartmentId, Created, CreatedBy)
SELECT 'RE', 'Religious Education', 'RE', 2, 'JSS Religious Education (CRE / IRE / HRE) (Grade 7-9)', 0, 1, 60,
       COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Religious Education', 'Humanities and Social Sciences', 'Humanities') LIMIT 1), @def_group_id),
       @def_dept_id, NOW(), 'jss-seed'
WHERE NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = 'RE');

-- 7. Pre-Technical Studies
INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, Examinable, `Rank`,
                      SubjectGroupId, DepartmentId, Created, CreatedBy)
SELECT 'PTS', 'Pre-Technical Studies', 'PTS', 3, 'JSS Pre-Technical Studies (Grade 7-9)', 0, 1, 70,
       COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Pre-Technical', 'Applied Sciences', 'Sciences') LIMIT 1), @def_group_id),
       @def_dept_id, NOW(), 'jss-seed'
WHERE NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = 'PTS');

-- 8. Agriculture and Nutrition (subsumes Home Science)
INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, Examinable, `Rank`,
                      SubjectGroupId, DepartmentId, Created, CreatedBy)
SELECT 'AGN', 'Agriculture and Nutrition', 'AGN', 4, 'JSS Agriculture and Nutrition incl. Home Science (Grade 7-9)', 0, 1, 80,
       COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Agriculture', 'Applied Sciences', 'Sciences') LIMIT 1), @def_group_id),
       @def_dept_id, NOW(), 'jss-seed'
WHERE NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = 'AGN');

-- 9. Creative Arts and Sports (subsumes Visual Arts, Performing Arts, PE)
INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, Examinable, `Rank`,
                      SubjectGroupId, DepartmentId, Created, CreatedBy)
SELECT 'CAS', 'Creative Arts and Sports', 'CAS', 5, 'JSS Creative Arts and Sports incl. Visual/Performing Arts and PE (Grade 7-9)', 0, 1, 90,
       COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Creative Arts and Sports', 'Creative Arts', 'Arts and Sports', 'Sports') LIMIT 1), @def_group_id),
       @def_dept_id, NOW(), 'jss-seed'
WHERE NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = 'CAS');

-- ---------------------------------------------------------------------------
-- Optional foreign-language electives - uncomment whichever your school offers.
-- ---------------------------------------------------------------------------
-- INSERT INTO Subjects (Code, Name, Abbr, NumOfLessons, Description, Optional, Examinable, `Rank`,
--                       SubjectGroupId, DepartmentId, Created, CreatedBy)
-- SELECT * FROM (
--     SELECT 'FRE' AS Code, 'French' AS Name, 'FRE' AS Abbr, 3 AS NumOfLessons,
--            'JSS optional French (Grade 7-9)' AS Description, 1 AS Optional, 1 AS Examinable, 100 AS `Rank`,
--            COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Languages','Language') LIMIT 1), @def_group_id) AS SubjectGroupId,
--            @def_dept_id AS DepartmentId, NOW() AS Created, 'jss-seed' AS CreatedBy
--     UNION ALL SELECT 'GER','German',   'GER',3,'JSS optional German (Grade 7-9)',  1,1,110, COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Languages','Language') LIMIT 1), @def_group_id), @def_dept_id, NOW(),'jss-seed'
--     UNION ALL SELECT 'MAN','Mandarin', 'MAN',3,'JSS optional Mandarin (Grade 7-9)',1,1,120, COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Languages','Language') LIMIT 1), @def_group_id), @def_dept_id, NOW(),'jss-seed'
--     UNION ALL SELECT 'ARA','Arabic',   'ARA',3,'JSS optional Arabic (Grade 7-9)',  1,1,130, COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Languages','Language') LIMIT 1), @def_group_id), @def_dept_id, NOW(),'jss-seed'
--     UNION ALL SELECT 'IL', 'Indigenous Language','IL',2,'JSS optional Indigenous Language (Grade 7-9)',1,1,140, COALESCE((SELECT Id FROM SubjectGroups WHERE Name IN ('Languages','Language') LIMIT 1), @def_group_id), @def_dept_id, NOW(),'jss-seed'
-- ) optionals
-- WHERE optionals.Code NOT IN (SELECT Code FROM Subjects);

-- ---------------------------------------------------------------------------
-- Verification - confirm what landed.
-- ---------------------------------------------------------------------------
SELECT s.Id, s.Code, s.Name, s.Abbr, s.NumOfLessons, s.Optional, s.Examinable,
       sg.Name AS `Group`, d.Name AS Dept
FROM   Subjects s
JOIN   SubjectGroups sg ON sg.Id = s.SubjectGroupId
JOIN   Departments   d  ON d.Id  = s.DepartmentId
WHERE  s.CreatedBy = 'jss-seed'
ORDER BY s.`Rank`;

SELECT COUNT(*) AS jss_subjects_inserted FROM Subjects WHERE CreatedBy = 'jss-seed';
