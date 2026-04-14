-- ============================================================
-- Seed 4-Point and 8-Point CBC Grading Systems (Clean)
-- ============================================================

USE schoolwebappdb;
SET SQL_SAFE_UPDATES = 0;

-- Get the CBC curriculum ID
SET @cbcId = (SELECT Id FROM curricula WHERE Name LIKE '%CBC%' OR Name LIKE '%CBE%' ORDER BY Id LIMIT 1);
SET @cbcId = IFNULL(@cbcId, (SELECT Id FROM curricula ORDER BY Id LIMIT 1));

-- ============================================================
-- Step 1: Clean existing data (FK order)
-- ============================================================
DELETE FROM studentassessments;
DELETE FROM examresults;
DELETE FROM grades;

-- ============================================================
-- Step 2: Insert 4-Point grades (EE, ME, AE, BE)
-- ============================================================
INSERT INTO grades (Name, Abbr, MinScore, MaxScore, Points, RemarksSwa, RemarksEng, `Rank`, Category, CurriculumId, Created, Modified) VALUES
('Exceeding Expectation', 'EE', 75, 100, 4, 'Anazidi matarajio', 'Exceeding Expectation', 1, '4-Point', @cbcId, NOW(), NOW()),
('Meeting Expectation', 'ME', 50, 74, 3, 'Anakutana na matarajio', 'Meeting Expectation', 2, '4-Point', @cbcId, NOW(), NOW()),
('Approaching Expectation', 'AE', 25, 49, 2, 'Anakaribia matarajio', 'Approaching Expectation', 3, '4-Point', @cbcId, NOW(), NOW()),
('Below Expectation', 'BE', 0, 24, 1, 'Chini ya matarajio', 'Below Expectation', 4, '4-Point', @cbcId, NOW(), NOW());

-- ============================================================
-- Step 3: Insert 8-Point grades (EE1, EE2, ME1, ME2, AE1, AE2, BE1, BE2)
-- ============================================================
INSERT INTO grades (Name, Abbr, MinScore, MaxScore, Points, RemarksSwa, RemarksEng, `Rank`, Category, CurriculumId, Created, Modified) VALUES
('Highly Exceeding Expectation', 'EE1', 88, 100, 8, 'Anazidi sana matarajio', 'Highly Exceeding Expectation', 1, '8-Point', @cbcId, NOW(), NOW()),
('Exceeding Expectation', 'EE2', 75, 87, 7, 'Anazidi matarajio', 'Exceeding Expectation', 2, '8-Point', @cbcId, NOW(), NOW()),
('Highly Meeting Expectation', 'ME1', 63, 74, 6, 'Anakutana vizuri na matarajio', 'Highly Meeting Expectation', 3, '8-Point', @cbcId, NOW(), NOW()),
('Meeting Expectation', 'ME2', 50, 62, 5, 'Anakutana na matarajio', 'Meeting Expectation', 4, '8-Point', @cbcId, NOW(), NOW()),
('Highly Approaching Expectation', 'AE1', 38, 49, 4, 'Anakaribia vizuri matarajio', 'Highly Approaching Expectation', 5, '8-Point', @cbcId, NOW(), NOW()),
('Approaching Expectation', 'AE2', 25, 37, 3, 'Anakaribia matarajio', 'Approaching Expectation', 6, '8-Point', @cbcId, NOW(), NOW()),
('Highly Below Expectation', 'BE1', 13, 24, 2, 'Chini sana ya matarajio', 'Highly Below Expectation', 7, '8-Point', @cbcId, NOW(), NOW()),
('Below Expectation', 'BE2', 0, 12, 1, 'Chini ya matarajio', 'Below Expectation', 8, '8-Point', @cbcId, NOW(), NOW());

-- ============================================================
-- Step 4: Add Global Settings for grading systems per section
-- ============================================================
DELETE FROM globalsettings WHERE Module = 'Assessment' AND SettingKey = 'grading_system';

INSERT INTO globalsettings (Module, SettingKey, SettingValue, Description, Created, Modified)
SELECT 'Grading', 'ExamResults', '4-Point', 'Grading for exam results, report form and broadsheet', NOW(), NOW()
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM globalsettings WHERE SettingKey = 'ExamResults' AND Module = 'Grading');

INSERT INTO globalsettings (Module, SettingKey, SettingValue, Description, Created, Modified)
SELECT 'Grading', 'StudentAssessment', '4-Point', 'Grading for student assessments and assessment report', NOW(), NOW()
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM globalsettings WHERE SettingKey = 'StudentAssessment' AND Module = 'Grading');

INSERT INTO globalsettings (Module, SettingKey, SettingValue, Description, Created, Modified)
SELECT 'Grading', 'ValueScores', '4-Point', 'Grading for values assessment', NOW(), NOW()
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM globalsettings WHERE SettingKey = 'ValueScores' AND Module = 'Grading');

INSERT INTO globalsettings (Module, SettingKey, SettingValue, Description, Created, Modified)
SELECT 'Grading', 'CoCurricular', '4-Point', 'Grading for co-curricular student scores', NOW(), NOW()
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM globalsettings WHERE SettingKey = 'CoCurricular' AND Module = 'Grading');

INSERT INTO globalsettings (Module, SettingKey, SettingValue, Description, Created, Modified)
SELECT 'Grading', 'RankingMethod', 'mean_points', 'How students are ranked on broadsheet and report form (mean_marks or mean_points)', NOW(), NOW()
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM globalsettings WHERE SettingKey = 'RankingMethod' AND Module = 'Grading');

SET SQL_SAFE_UPDATES = 1;

-- Verify
SELECT Category, COUNT(*) AS Total, GROUP_CONCAT(Abbr ORDER BY `Rank`) AS Grades FROM grades GROUP BY Category;
SELECT * FROM globalsettings WHERE SettingKey = 'grading_system';
