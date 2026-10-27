-- KICD "Level learning outcomes for Primary Education" (Grade 1-6 curriculum
-- designs, revised 2024), stored for Lower primary and Upper primary on their own
-- (GeneralOutcomes.EducationLevelId, migration GeneralOutcomeEducationLevel).
-- Pre-primary and Junior School keep the outcomes of their education level type.
-- Run after deploying the API (its startup adds the column). Safe to run twice.
-- Education levels are matched by name - check them first:
SELECT Id, Name, EducationLevelTypeId FROM educationlevels WHERE Name IN ('Lower primary', 'Upper primary');

INSERT INTO generaloutcomes (EducationLevelTypeId, EducationLevelId, Name, Description, `Rank`, Created, CreatedBy)
SELECT el.EducationLevelTypeId, el.Id, o.Name, '', o.r, NOW(), 'admin'
FROM educationlevels el
CROSS JOIN (
    SELECT 1 r, 'Communicate appropriately using verbal and or non-verbal modes in a variety of contexts' Name
    UNION ALL SELECT 2, 'Demonstrate mastery of number concepts to solve problems in day to day life'
    UNION ALL SELECT 3, 'Demonstrate social skills, moral and religious values for positive contribution to society'
    UNION ALL SELECT 4, 'Develop one''s interests and talents for personal fulfilment'
    UNION ALL SELECT 5, 'Make informed decisions as local and global citizens of a diverse, democratic society in an interdependent world'
    UNION ALL SELECT 6, 'Explore, manipulate, manage and conserve the environment effectively for learning and sustainable development'
    UNION ALL SELECT 7, 'Acquire digital literacy skills for learning and enjoyment'
    UNION ALL SELECT 8, 'Appreciate the country''s rich, diverse cultural heritage for harmonious living'
) o
WHERE el.Name IN ('Lower primary', 'Upper primary')
  AND NOT EXISTS (SELECT 1 FROM generaloutcomes g WHERE g.EducationLevelId = el.Id AND g.Name = o.Name);

SELECT g.Id, el.Name AS level, g.`Rank`, g.Name FROM generaloutcomes g JOIN educationlevels el ON el.Id = g.EducationLevelId ORDER BY el.`Rank`, g.`Rank`;
