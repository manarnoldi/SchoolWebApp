-- ============================================================
-- Merge Social Skills into Responsibilities table
-- ============================================================

USE schoolwebappdb;
SET SQL_SAFE_UPDATES = 0;

-- Step 1: Tag existing responsibilities
UPDATE responsibilities SET Category = 'Responsibility' WHERE Category IS NULL;

-- Step 2: Copy social skills into responsibilities table
INSERT INTO responsibilities (Name, Description, `Rank`, Category, Created, Modified)
SELECT Name, Description, `Rank`, 'Social Skill', Created, Modified
FROM socialskills;

-- Step 3: Remap studentresponsibilities that referenced old social skill IDs
-- Use a regular table since MySQL can't reopen temp tables
DROP TABLE IF EXISTS _skill_mapping;

CREATE TABLE _skill_mapping AS
SELECT ss.Id AS OldId, r.Id AS NewId
FROM socialskills ss
JOIN responsibilities r ON r.Name = ss.Name AND r.Category = 'Social Skill';

UPDATE studentresponsibilities sr
JOIN _skill_mapping sm ON sr.ResponsibilitySocialSkillId = sm.OldId
SET sr.ResponsibilitySocialSkillId = sm.NewId;

DROP TABLE IF EXISTS _skill_mapping;

SET SQL_SAFE_UPDATES = 1;

-- Verify
SELECT Category, COUNT(*) AS Total FROM responsibilities GROUP BY Category;
SELECT * FROM responsibilities ORDER BY Category, `Rank`;
