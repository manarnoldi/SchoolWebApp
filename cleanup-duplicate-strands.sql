-- ============================================================
-- Cleanup duplicate strands and substrands
-- Removes duplicates where same Name+Code+SubjectId+LearningLevelId
-- Keeps the one with the lowest Id, reassigns children to it
-- ============================================================

USE schoolwebappdb;
SET SQL_SAFE_UPDATES = 0;

-- Step 1: Find duplicate substrands and reassign their specificoutcomes to the keeper
-- A substrand is duplicate if same Name+Code+StrandId
CREATE TEMPORARY TABLE IF NOT EXISTS dup_substrands AS
SELECT MIN(ss.Id) AS KeepId, ss.Name, ss.Code, ss.StrandId
FROM substrands ss
GROUP BY ss.Name, ss.Code, ss.StrandId
HAVING COUNT(*) > 1;

-- Reassign specificoutcomes from duplicate substrands to the keeper
UPDATE specificoutcomes so
JOIN substrands ss ON so.SubStrandId = ss.Id
JOIN dup_substrands d ON ss.Name = d.Name AND ss.Code = d.Code AND ss.StrandId = d.StrandId AND ss.Id != d.KeepId
SET so.SubStrandId = d.KeepId;

-- Delete duplicate substrands (not the keeper)
DELETE ss FROM substrands ss
JOIN dup_substrands d ON ss.Name = d.Name AND ss.Code = d.Code AND ss.StrandId = d.StrandId AND ss.Id != d.KeepId;

DROP TEMPORARY TABLE IF EXISTS dup_substrands;

-- Step 2: Find duplicate strands and reassign their substrands to the keeper
-- A strand is duplicate if same Name+Code+SubjectId+LearningLevelId
CREATE TEMPORARY TABLE IF NOT EXISTS dup_strands AS
SELECT MIN(s.Id) AS KeepId, s.Name, s.Code, s.SubjectId, s.LearningLevelId
FROM strands s
GROUP BY s.Name, s.Code, s.SubjectId, s.LearningLevelId
HAVING COUNT(*) > 1;

-- Reassign substrands from duplicate strands to the keeper
UPDATE substrands ss
JOIN strands s ON ss.StrandId = s.Id
JOIN dup_strands d ON s.Name = d.Name AND s.Code = d.Code AND s.SubjectId = d.SubjectId AND s.LearningLevelId = d.LearningLevelId AND s.Id != d.KeepId
SET ss.StrandId = d.KeepId;

-- Reassign studentassessments from duplicate strands to the keeper
UPDATE studentassessments sa
JOIN dup_strands d ON sa.StrandId != d.KeepId
JOIN strands s ON sa.StrandId = s.Id AND s.Name = d.Name AND s.Code = d.Code AND s.SubjectId = d.SubjectId AND s.LearningLevelId = d.LearningLevelId
SET sa.StrandId = d.KeepId;

-- Delete duplicate strands (not the keeper)
DELETE s FROM strands s
JOIN dup_strands d ON s.Name = d.Name AND s.Code = d.Code AND s.SubjectId = d.SubjectId AND s.LearningLevelId = d.LearningLevelId AND s.Id != d.KeepId;

DROP TEMPORARY TABLE IF EXISTS dup_strands;

-- Step 3: Now handle substrands that may have become duplicates after strand merging
-- (same Name+Code under same StrandId)
CREATE TEMPORARY TABLE IF NOT EXISTS dup_substrands2 AS
SELECT MIN(ss.Id) AS KeepId, ss.Name, ss.Code, ss.StrandId
FROM substrands ss
GROUP BY ss.Name, ss.Code, ss.StrandId
HAVING COUNT(*) > 1;

-- Reassign specificoutcomes
UPDATE specificoutcomes so
JOIN substrands ss ON so.SubStrandId = ss.Id
JOIN dup_substrands2 d ON ss.Name = d.Name AND ss.Code = d.Code AND ss.StrandId = d.StrandId AND ss.Id != d.KeepId
SET so.SubStrandId = d.KeepId;

-- Reassign studentassessments
UPDATE studentassessments sa
JOIN dup_substrands2 d ON sa.SubStrandId != d.KeepId
JOIN substrands ss ON sa.SubStrandId = ss.Id AND ss.Name = d.Name AND ss.Code = d.Code AND ss.StrandId = d.StrandId
SET sa.SubStrandId = d.KeepId;

-- Delete duplicate substrands
DELETE ss FROM substrands ss
JOIN dup_substrands2 d ON ss.Name = d.Name AND ss.Code = d.Code AND ss.StrandId = d.StrandId AND ss.Id != d.KeepId;

DROP TEMPORARY TABLE IF EXISTS dup_substrands2;

-- Step 4: Remove duplicate specificoutcomes (same Name+SubStrandId)
CREATE TEMPORARY TABLE IF NOT EXISTS dup_outcomes AS
SELECT MIN(so.Id) AS KeepId, so.Name, so.SubStrandId
FROM specificoutcomes so
GROUP BY so.Name, so.SubStrandId
HAVING COUNT(*) > 1;

DELETE so FROM specificoutcomes so
JOIN dup_outcomes d ON so.Name = d.Name AND so.SubStrandId = d.SubStrandId AND so.Id != d.KeepId;

DROP TEMPORARY TABLE IF EXISTS dup_outcomes;

SET SQL_SAFE_UPDATES = 1;

-- Verify
SELECT 'Strands' AS Entity, COUNT(*) AS Total FROM strands
UNION ALL SELECT 'SubStrands', COUNT(*) FROM substrands
UNION ALL SELECT 'SpecificOutcomes', COUNT(*) FROM specificoutcomes;

-- Check for remaining duplicates
SELECT 'Duplicate Strands' AS CheckType, COUNT(*) AS Remaining FROM (
    SELECT Name, Code, SubjectId, LearningLevelId FROM strands
    GROUP BY Name, Code, SubjectId, LearningLevelId HAVING COUNT(*) > 1
) t
UNION ALL
SELECT 'Duplicate SubStrands', COUNT(*) FROM (
    SELECT Name, Code, StrandId FROM substrands
    GROUP BY Name, Code, StrandId HAVING COUNT(*) > 1
) t2
UNION ALL
SELECT 'Duplicate Outcomes', COUNT(*) FROM (
    SELECT Name, SubStrandId FROM specificoutcomes
    GROUP BY Name, SubStrandId HAVING COUNT(*) > 1
) t3;
