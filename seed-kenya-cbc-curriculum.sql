-- ============================================================
-- Kenya CBC Curriculum Full Seed Data
-- Covers Grades 1-9, all subjects with Strands, SubStrands,
-- and Specific Learning Outcomes
-- ============================================================
-- Run: mysql -u root -p schoolwebappdb < seed-kenya-cbc-curriculum.sql
-- ============================================================

USE schoolwebappdb;

SET SQL_SAFE_UPDATES = 0;
SET @now = NOW();

-- ============================================================
-- STEP 1: Clean existing assessment data (FK order matters)
-- ============================================================
DELETE FROM studentassessments;
DELETE FROM keyquestion;
DELETE FROM learningexperience;
DELETE FROM pci;
DELETE FROM lessonallocation;
DELETE FROM specificoutcomes;
DELETE FROM substrands;
DELETE FROM strands;

-- ============================================================
-- STEP 2: Look up reference IDs dynamically
-- ============================================================

-- Curriculum
SET @cbcId = (SELECT Id FROM curricula WHERE Name LIKE '%CBC%' OR Name LIKE '%CBE%' OR Name LIKE '%Competenc%' ORDER BY Id LIMIT 1);
SET @cbcId = IFNULL(@cbcId, (SELECT Id FROM curricula ORDER BY Id LIMIT 1));

-- BroadOutcome & GeneralOutcome (required FKs for SpecificOutcomes)
SET @defaultBroadOutcomeId = (SELECT Id FROM broadoutcomes ORDER BY Id LIMIT 1);
SET @defaultGeneralOutcomeId = (SELECT Id FROM generaloutcomes ORDER BY Id LIMIT 1);

-- Learning Levels (Grade 1-9)
SET @grade1Id = (SELECT Id FROM learninglevels WHERE Name LIKE '%Grade 1%' OR Name LIKE '%Grade1%' OR Name = 'Grade 1' LIMIT 1);
SET @grade2Id = (SELECT Id FROM learninglevels WHERE Name LIKE '%Grade 2%' OR Name LIKE '%Grade2%' OR Name = 'Grade 2' LIMIT 1);
SET @grade3Id = (SELECT Id FROM learninglevels WHERE Name LIKE '%Grade 3%' OR Name LIKE '%Grade3%' OR Name = 'Grade 3' LIMIT 1);
SET @grade4Id = (SELECT Id FROM learninglevels WHERE Name LIKE '%Grade 4%' OR Name LIKE '%Grade4%' OR Name = 'Grade 4' LIMIT 1);
SET @grade5Id = (SELECT Id FROM learninglevels WHERE Name LIKE '%Grade 5%' OR Name LIKE '%Grade5%' OR Name = 'Grade 5' LIMIT 1);
SET @grade6Id = (SELECT Id FROM learninglevels WHERE Name LIKE '%Grade 6%' OR Name LIKE '%Grade6%' OR Name = 'Grade 6' LIMIT 1);
SET @grade7Id = (SELECT Id FROM learninglevels WHERE Name LIKE '%Grade 7%' OR Name LIKE '%Grade7%' OR Name = 'Grade 7' LIMIT 1);
SET @grade8Id = (SELECT Id FROM learninglevels WHERE Name LIKE '%Grade 8%' OR Name LIKE '%Grade8%' OR Name = 'Grade 8' LIMIT 1);
SET @grade9Id = (SELECT Id FROM learninglevels WHERE Name LIKE '%Grade 9%' OR Name LIKE '%Grade9%' OR Name = 'Grade 9' LIMIT 1);

-- Subjects
SET @mathId = (SELECT Id FROM subjects WHERE Name LIKE '%Math%' LIMIT 1);
SET @engId = (SELECT Id FROM subjects WHERE Name LIKE '%English%' LIMIT 1);
SET @litId = (SELECT Id FROM subjects WHERE Name LIKE '%Literacy%' LIMIT 1);
SET @litId = IFNULL(@litId, @engId);
SET @engLangId = @engId;
SET @kisId = (SELECT Id FROM subjects WHERE Name LIKE '%Kiswahili%' OR Name LIKE '%Swahili%' LIMIT 1);
SET @sciId = (SELECT Id FROM subjects WHERE Name LIKE '%Science%' AND Name NOT LIKE '%Social%' AND Name NOT LIKE '%Integrated%' LIMIT 1);
SET @socId = (SELECT Id FROM subjects WHERE Name LIKE '%Social%' LIMIT 1);
SET @agrId = (SELECT Id FROM subjects WHERE Name LIKE '%Agri%' LIMIT 1);
SET @creId = (SELECT Id FROM subjects WHERE Name LIKE '%Christian%' OR Name LIKE '%CRE%' LIMIT 1);
SET @ireId = (SELECT Id FROM subjects WHERE Name LIKE '%Islamic%' OR Name LIKE '%IRE%' LIMIT 1);
SET @artId = (SELECT Id FROM subjects WHERE Name LIKE '%Creative%' OR Name LIKE '%Art%' LIMIT 1);
SET @envId = (SELECT Id FROM subjects WHERE Name LIKE '%Environment%' LIMIT 1);
SET @hygId = (SELECT Id FROM subjects WHERE Name LIKE '%Hygiene%' OR Name LIKE '%Nutrition%' LIMIT 1);
SET @hygId = IFNULL(@hygId, @envId);
SET @preTechId = (SELECT Id FROM subjects WHERE Name LIKE '%Pre-Tech%' OR Name LIKE '%PreTech%' OR Name LIKE '%Technical%' LIMIT 1);
SET @intSciId = (SELECT Id FROM subjects WHERE Name LIKE '%Integrated%' LIMIT 1);
SET @intSciId = IFNULL(@intSciId, @sciId);

-- Verify key IDs
SELECT 'CBC Curriculum ID' AS Item, @cbcId AS Value
UNION ALL SELECT 'Default BroadOutcome ID', @defaultBroadOutcomeId
UNION ALL SELECT 'Default GeneralOutcome ID', @defaultGeneralOutcomeId
UNION ALL SELECT 'Grade 1 ID', @grade1Id
UNION ALL SELECT 'Grade 4 ID', @grade4Id
UNION ALL SELECT 'Grade 7 ID', @grade7Id
UNION ALL SELECT 'Mathematics ID', @mathId
UNION ALL SELECT 'English ID', @engId
UNION ALL SELECT 'Literacy ID', @litId
UNION ALL SELECT 'Kiswahili ID', @kisId
UNION ALL SELECT 'Science ID', @sciId
UNION ALL SELECT 'Social Studies ID', @socId
UNION ALL SELECT 'Agriculture ID', @agrId
UNION ALL SELECT 'CRE ID', @creId
UNION ALL SELECT 'IRE ID', @ireId
UNION ALL SELECT 'Creative Arts ID', @artId
UNION ALL SELECT 'Environment ID', @envId
UNION ALL SELECT 'Hygiene ID', @hygId
UNION ALL SELECT 'Pre-Technical ID', @preTechId
UNION ALL SELECT 'Integrated Science ID', @intSciId;

-- ============================================================
-- STEP 3: INSERT STRANDS
-- ============================================================

-- ============================================
-- MATHEMATICS - Grades 1-9
-- ============================================

-- Grade 1
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Numbers', '1', 'Number concepts, whole numbers, addition and subtraction', 1, @mathId, @grade1Id, @cbcId, 1, @now, @now),
('Measurement', '2', 'Length, mass, capacity, time and money', 2, @mathId, @grade1Id, @cbcId, 1, @now, @now),
('Geometry', '3', 'Lines and shapes', 3, @mathId, @grade1Id, @cbcId, 1, @now, @now);

-- Grade 2
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Numbers', '1', 'Number concepts, whole numbers, operations and fractions', 1, @mathId, @grade2Id, @cbcId, 1, @now, @now),
('Measurement', '2', 'Length, mass, capacity, time and money', 2, @mathId, @grade2Id, @cbcId, 1, @now, @now),
('Geometry', '3', 'Lines and shapes', 3, @mathId, @grade2Id, @cbcId, 1, @now, @now);

-- Grade 3
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Numbers', '1', 'Number concepts, whole numbers, operations and fractions', 1, @mathId, @grade3Id, @cbcId, 1, @now, @now),
('Measurement', '2', 'Length, mass, capacity, time and money', 2, @mathId, @grade3Id, @cbcId, 1, @now, @now),
('Geometry', '3', 'Lines and shapes including 3D', 3, @mathId, @grade3Id, @cbcId, 1, @now, @now);

-- Grade 4
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Numbers', '1', 'Whole numbers, fractions, decimals and percentages', 1, @mathId, @grade4Id, @cbcId, 1, @now, @now),
('Measurement', '2', 'Length, area, mass, capacity, time and money', 2, @mathId, @grade4Id, @cbcId, 1, @now, @now),
('Geometry', '3', 'Lines, angles, plane shapes and symmetry', 3, @mathId, @grade4Id, @cbcId, 1, @now, @now),
('Data Handling', '4', 'Data collection, representation and interpretation', 4, @mathId, @grade4Id, @cbcId, 1, @now, @now);

-- Grade 5
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Numbers', '1', 'Whole numbers, fractions, decimals, percentages, ratio and proportion', 1, @mathId, @grade5Id, @cbcId, 1, @now, @now),
('Measurement', '2', 'Length, area, volume, speed and money', 2, @mathId, @grade5Id, @cbcId, 1, @now, @now),
('Geometry', '3', 'Angles, plane figures and 3D shapes', 3, @mathId, @grade5Id, @cbcId, 1, @now, @now),
('Data Handling', '4', 'Statistics and data interpretation', 4, @mathId, @grade5Id, @cbcId, 1, @now, @now);

-- Grade 6
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Numbers', '1', 'Whole numbers, fractions, decimals, inequalities', 1, @mathId, @grade6Id, @cbcId, 1, @now, @now),
('Measurement', '2', 'Length, area, capacity, mass, time and money', 2, @mathId, @grade6Id, @cbcId, 1, @now, @now),
('Geometry', '3', 'Lines, angles and 3D objects', 3, @mathId, @grade6Id, @cbcId, 1, @now, @now),
('Data Handling', '4', 'Bar graphs and data interpretation', 4, @mathId, @grade6Id, @cbcId, 1, @now, @now);

-- Grade 7
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Numbers', '1', 'Whole numbers, fractions, squares and square roots, integers', 1, @mathId, @grade7Id, @cbcId, 1, @now, @now),
('Algebra', '2', 'Algebraic expressions and linear equations', 2, @mathId, @grade7Id, @cbcId, 1, @now, @now),
('Measurements', '3', 'Length, area, volume, mass, time, speed and money', 3, @mathId, @grade7Id, @cbcId, 1, @now, @now),
('Geometry', '4', 'Angles, lines, plane figures and scale drawing', 4, @mathId, @grade7Id, @cbcId, 1, @now, @now),
('Data Handling and Probability', '5', 'Statistics and probability', 5, @mathId, @grade7Id, @cbcId, 1, @now, @now);

-- Grade 8
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Numbers', '1', 'Further number work, cubes and cube roots', 1, @mathId, @grade8Id, @cbcId, 1, @now, @now),
('Algebra', '2', 'Simultaneous equations and quadratic expressions', 2, @mathId, @grade8Id, @cbcId, 1, @now, @now),
('Measurements', '3', 'Area, surface area, volume and money', 3, @mathId, @grade8Id, @cbcId, 1, @now, @now),
('Geometry', '4', 'Transformation and circles', 4, @mathId, @grade8Id, @cbcId, 1, @now, @now),
('Data Handling and Probability', '5', 'Data interpretation and probability', 5, @mathId, @grade8Id, @cbcId, 1, @now, @now);

-- Grade 9
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Numbers', '1', 'Integers, cubes and cube roots, indices, logarithms, compound proportions', 1, @mathId, @grade9Id, @cbcId, 1, @now, @now),
('Algebra', '2', 'Matrices, equation of a straight line, linear inequalities', 2, @mathId, @grade9Id, @cbcId, 1, @now, @now),
('Measurements', '3', 'Area, volume, mass, time, speed, money, approximations', 3, @mathId, @grade9Id, @cbcId, 1, @now, @now),
('Geometry', '4', 'Coordinates, scale drawing, similarity, trigonometry', 4, @mathId, @grade9Id, @cbcId, 1, @now, @now),
('Data Handling and Probability', '5', 'Grouped data interpretation and probability', 5, @mathId, @grade9Id, @cbcId, 1, @now, @now);

-- ============================================
-- LITERACY ACTIVITIES - Grades 1-3 (use @litId)
-- ============================================

-- Grade 1
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Listening and Speaking', '1', 'Listening and speaking skills', 1, @litId, @grade1Id, @cbcId, 1, @now, @now),
('Reading', '2', 'Phonological awareness, phonics and comprehension', 2, @litId, @grade1Id, @cbcId, 1, @now, @now),
('Writing', '3', 'Pre-writing and writing sentences', 3, @litId, @grade1Id, @cbcId, 1, @now, @now);

-- Grade 2
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Listening and Speaking', '1', 'Oral communication skills', 1, @litId, @grade2Id, @cbcId, 1, @now, @now),
('Reading', '2', 'Word recognition and comprehension', 2, @litId, @grade2Id, @cbcId, 1, @now, @now),
('Writing', '3', 'Guided writing skills', 3, @litId, @grade2Id, @cbcId, 1, @now, @now);

-- Grade 3
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Listening and Speaking', '1', 'Oral communication and presentations', 1, @litId, @grade3Id, @cbcId, 1, @now, @now),
('Reading', '2', 'Fluency, comprehension and vocabulary', 2, @litId, @grade3Id, @cbcId, 1, @now, @now),
('Writing', '3', 'Creative writing skills', 3, @litId, @grade3Id, @cbcId, 1, @now, @now);

-- ============================================
-- ENGLISH LANGUAGE ACTIVITIES - Grades 1-3 (use @engLangId)
-- ============================================

-- Grade 1
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Listening and Speaking', '1', 'Listening and speaking in English', 1, @engLangId, @grade1Id, @cbcId, 1, @now, @now),
('Reading', '2', 'Phonics and word study in English', 2, @engLangId, @grade1Id, @cbcId, 1, @now, @now),
('Writing', '3', 'Early writing in English', 3, @engLangId, @grade1Id, @cbcId, 1, @now, @now),
('Language Use', '4', 'Grammar and vocabulary', 4, @engLangId, @grade1Id, @cbcId, 1, @now, @now);

-- Grade 2
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Listening and Speaking', '1', 'Oral communication in English', 1, @engLangId, @grade2Id, @cbcId, 1, @now, @now),
('Reading', '2', 'Reading fluency and comprehension', 2, @engLangId, @grade2Id, @cbcId, 1, @now, @now),
('Writing', '3', 'Guided writing in English', 3, @engLangId, @grade2Id, @cbcId, 1, @now, @now),
('Language Use', '4', 'Grammar skills', 4, @engLangId, @grade2Id, @cbcId, 1, @now, @now);

-- Grade 3
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Listening and Speaking', '1', 'Advanced oral communication', 1, @engLangId, @grade3Id, @cbcId, 1, @now, @now),
('Reading', '2', 'Comprehension and critical reading', 2, @engLangId, @grade3Id, @cbcId, 1, @now, @now),
('Writing', '3', 'Composition writing', 3, @engLangId, @grade3Id, @cbcId, 1, @now, @now),
('Language Use', '4', 'Grammar and language use', 4, @engLangId, @grade3Id, @cbcId, 1, @now, @now);

-- ============================================
-- ENGLISH - Grades 4-9 (use @engId)
-- ============================================

-- Grade 4
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Listening and Speaking', '1', 'Listening comprehension and oral communication', 1, @engId, @grade4Id, @cbcId, 1, @now, @now),
('Reading', '2', 'Reading fluency, comprehension and vocabulary', 2, @engId, @grade4Id, @cbcId, 1, @now, @now),
('Writing', '3', 'Functional and creative writing', 3, @engId, @grade4Id, @cbcId, 1, @now, @now),
('Language Use', '4', 'Grammar and usage', 4, @engId, @grade4Id, @cbcId, 1, @now, @now);

-- Grade 5
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Listening and Speaking', '1', 'Listening and speaking skills', 1, @engId, @grade5Id, @cbcId, 1, @now, @now),
('Reading', '2', 'Reading and comprehension', 2, @engId, @grade5Id, @cbcId, 1, @now, @now),
('Writing', '3', 'Writing skills', 3, @engId, @grade5Id, @cbcId, 1, @now, @now),
('Language Use', '4', 'Grammar and usage', 4, @engId, @grade5Id, @cbcId, 1, @now, @now);

-- Grade 6
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Listening and Speaking', '1', 'Advanced oral skills', 1, @engId, @grade6Id, @cbcId, 1, @now, @now),
('Reading', '2', 'Critical reading', 2, @engId, @grade6Id, @cbcId, 1, @now, @now),
('Writing', '3', 'Composition and functional writing', 3, @engId, @grade6Id, @cbcId, 1, @now, @now),
('Language Use', '4', 'Grammar', 4, @engId, @grade6Id, @cbcId, 1, @now, @now);

-- Grade 7
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Listening and Speaking', '1', 'Listening and speaking', 1, @engId, @grade7Id, @cbcId, 1, @now, @now),
('Reading', '2', 'Reading and comprehension', 2, @engId, @grade7Id, @cbcId, 1, @now, @now),
('Writing', '3', 'Writing', 3, @engId, @grade7Id, @cbcId, 1, @now, @now),
('Language Use', '4', 'Grammar and usage', 4, @engId, @grade7Id, @cbcId, 1, @now, @now);

-- Grade 8
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Listening and Speaking', '1', 'Advanced oral communication', 1, @engId, @grade8Id, @cbcId, 1, @now, @now),
('Reading', '2', 'Critical and analytical reading', 2, @engId, @grade8Id, @cbcId, 1, @now, @now),
('Writing', '3', 'Advanced writing', 3, @engId, @grade8Id, @cbcId, 1, @now, @now),
('Language Use', '4', 'Advanced grammar', 4, @engId, @grade8Id, @cbcId, 1, @now, @now);

-- Grade 9
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Listening and Speaking', '1', 'Mastery of oral communication', 1, @engId, @grade9Id, @cbcId, 1, @now, @now),
('Reading', '2', 'Advanced critical reading', 2, @engId, @grade9Id, @cbcId, 1, @now, @now),
('Writing', '3', 'Advanced composition', 3, @engId, @grade9Id, @cbcId, 1, @now, @now),
('Language Use', '4', 'Advanced language use', 4, @engId, @grade9Id, @cbcId, 1, @now, @now);

-- ============================================
-- KISWAHILI - Grades 1-9
-- ============================================

-- Grade 1
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Kusikiliza na Kuzungumza', '1', 'Stadi za kusikiliza na kuzungumza', 1, @kisId, @grade1Id, @cbcId, 1, @now, @now),
('Kusoma', '2', 'Fonetiki na ufahamu', 2, @kisId, @grade1Id, @cbcId, 1, @now, @now),
('Kuandika', '3', 'Uandishi wa awali', 3, @kisId, @grade1Id, @cbcId, 1, @now, @now),
('Sarufi', '4', 'Msamiati na sarufi', 4, @kisId, @grade1Id, @cbcId, 1, @now, @now);

-- Grade 2
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Kusikiliza na Kuzungumza', '1', 'Mawasiliano ya mdomo', 1, @kisId, @grade2Id, @cbcId, 1, @now, @now),
('Kusoma', '2', 'Ufasaha wa kusoma na ufahamu', 2, @kisId, @grade2Id, @cbcId, 1, @now, @now),
('Kuandika', '3', 'Uandishi wa mwongozo', 3, @kisId, @grade2Id, @cbcId, 1, @now, @now),
('Sarufi', '4', 'Sarufi na msamiati', 4, @kisId, @grade2Id, @cbcId, 1, @now, @now);

-- Grade 3
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Kusikiliza na Kuzungumza', '1', 'Mawasiliano ya mdomo', 1, @kisId, @grade3Id, @cbcId, 1, @now, @now),
('Kusoma', '2', 'Ufahamu na usomaji wa kina', 2, @kisId, @grade3Id, @cbcId, 1, @now, @now),
('Kuandika', '3', 'Insha', 3, @kisId, @grade3Id, @cbcId, 1, @now, @now),
('Sarufi', '4', 'Sarufi ya kina', 4, @kisId, @grade3Id, @cbcId, 1, @now, @now);

-- Grade 4
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Kusikiliza na Kuzungumza', '1', 'Kusikiliza na kuzungumza', 1, @kisId, @grade4Id, @cbcId, 1, @now, @now),
('Kusoma', '2', 'Kusoma na ufahamu', 2, @kisId, @grade4Id, @cbcId, 1, @now, @now),
('Kuandika', '3', 'Uandishi', 3, @kisId, @grade4Id, @cbcId, 1, @now, @now),
('Sarufi', '4', 'Sarufi', 4, @kisId, @grade4Id, @cbcId, 1, @now, @now);

-- Grade 5
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Kusikiliza na Kuzungumza', '1', 'Mawasiliano ya hali ya juu', 1, @kisId, @grade5Id, @cbcId, 1, @now, @now),
('Kusoma', '2', 'Usomaji wa kina', 2, @kisId, @grade5Id, @cbcId, 1, @now, @now),
('Kuandika', '3', 'Uandishi wa kina', 3, @kisId, @grade5Id, @cbcId, 1, @now, @now),
('Sarufi', '4', 'Sarufi ya kina', 4, @kisId, @grade5Id, @cbcId, 1, @now, @now);

-- Grade 6
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Kusikiliza na Kuzungumza', '1', 'Mawasiliano ya hali ya juu zaidi', 1, @kisId, @grade6Id, @cbcId, 1, @now, @now),
('Kusoma', '2', 'Usomaji wa kina na uchanganuzi', 2, @kisId, @grade6Id, @cbcId, 1, @now, @now),
('Kuandika', '3', 'Uandishi wa kina zaidi', 3, @kisId, @grade6Id, @cbcId, 1, @now, @now),
('Sarufi', '4', 'Sarufi ya hali ya juu', 4, @kisId, @grade6Id, @cbcId, 1, @now, @now);

-- Grade 7
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Kusikiliza na Kuzungumza', '1', 'Mawasiliano ya juu', 1, @kisId, @grade7Id, @cbcId, 1, @now, @now),
('Kusoma', '2', 'Usomaji wa kina', 2, @kisId, @grade7Id, @cbcId, 1, @now, @now),
('Kuandika', '3', 'Uandishi wa hali ya juu', 3, @kisId, @grade7Id, @cbcId, 1, @now, @now),
('Sarufi', '4', 'Sarufi ya kina', 4, @kisId, @grade7Id, @cbcId, 1, @now, @now);

-- Grade 8
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Kusikiliza na Kuzungumza', '1', 'Mawasiliano ya hali ya juu zaidi', 1, @kisId, @grade8Id, @cbcId, 1, @now, @now),
('Kusoma', '2', 'Usomaji makini na uchanganuzi', 2, @kisId, @grade8Id, @cbcId, 1, @now, @now),
('Kuandika', '3', 'Uandishi wa kina na uchanganuzi', 3, @kisId, @grade8Id, @cbcId, 1, @now, @now);

-- Grade 9
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Kusikiliza na Kuzungumza', '1', 'Umahiri wa mawasiliano ya mdomo', 1, @kisId, @grade9Id, @cbcId, 1, @now, @now),
('Kusoma', '2', 'Usomaji wa kina wa hali ya juu', 2, @kisId, @grade9Id, @cbcId, 1, @now, @now),
('Kuandika', '3', 'Uandishi wa hali ya juu zaidi', 3, @kisId, @grade9Id, @cbcId, 1, @now, @now);

-- ============================================
-- ENVIRONMENTAL ACTIVITIES - Grades 1-3
-- ============================================

-- Grade 1
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Home and School', '1', 'Home, school and community helpers', 1, @envId, @grade1Id, @cbcId, 1, @now, @now),
('Plants and Animals', '2', 'Common plants and animals in the environment', 2, @envId, @grade1Id, @cbcId, 1, @now, @now),
('Physical Environment', '3', 'Soil, water and air', 3, @envId, @grade1Id, @cbcId, 1, @now, @now),
('Weather and Climate', '4', 'Types of weather', 4, @envId, @grade1Id, @cbcId, 1, @now, @now);

-- Grade 2
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Home and School', '1', 'Home and school environment', 1, @envId, @grade2Id, @cbcId, 1, @now, @now),
('Plants and Animals', '2', 'Classification and uses of plants and animals', 2, @envId, @grade2Id, @cbcId, 1, @now, @now),
('Physical Environment', '3', 'Soil, water and rocks', 3, @envId, @grade2Id, @cbcId, 1, @now, @now),
('Weather and Climate', '4', 'Seasons and their effects', 4, @envId, @grade2Id, @cbcId, 1, @now, @now);

-- Grade 3
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Our Community', '1', 'Types of communities, transport and communication', 1, @envId, @grade3Id, @cbcId, 1, @now, @now),
('Plants and Animals', '2', 'Plant growth, animal life cycles and interdependence', 2, @envId, @grade3Id, @cbcId, 1, @now, @now),
('Physical Environment', '3', 'Soil, water and simple maps', 3, @envId, @grade3Id, @cbcId, 1, @now, @now),
('Energy', '4', 'Sources of energy', 4, @envId, @grade3Id, @cbcId, 1, @now, @now);

-- ============================================
-- HYGIENE AND NUTRITION ACTIVITIES - Grades 1-3
-- ============================================

-- Grade 1
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Hygiene', '1', 'Personal and environmental hygiene', 1, @hygId, @grade1Id, @cbcId, 1, @now, @now),
('Nutrition', '2', 'Food groups and safe food handling', 2, @hygId, @grade1Id, @cbcId, 1, @now, @now),
('Safety', '3', 'Personal safety', 3, @hygId, @grade1Id, @cbcId, 1, @now, @now);

-- Grade 2
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Hygiene', '1', 'Personal and environmental hygiene', 1, @hygId, @grade2Id, @cbcId, 1, @now, @now),
('Nutrition', '2', 'Meal planning and water safety', 2, @hygId, @grade2Id, @cbcId, 1, @now, @now),
('Safety', '3', 'Home and school safety', 3, @hygId, @grade2Id, @cbcId, 1, @now, @now);

-- Grade 3
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Hygiene', '1', 'Personal, environmental and reproductive hygiene', 1, @hygId, @grade3Id, @cbcId, 1, @now, @now),
('Nutrition', '2', 'Nutrients and food preparation', 2, @hygId, @grade3Id, @cbcId, 1, @now, @now),
('Safety', '3', 'First aid', 3, @hygId, @grade3Id, @cbcId, 1, @now, @now);

-- ============================================
-- CRE - Grades 1-9
-- ============================================

-- Grade 1
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Creation', '1', 'Self-awareness, family and creation', 1, @creId, @grade1Id, @cbcId, 1, @now, @now),
('The Holy Bible', '2', 'Physical handling and Bible stories', 2, @creId, @grade1Id, @cbcId, 1, @now, @now),
('The Early Life of Jesus Christ', '3', 'The birth of Jesus Christ', 3, @creId, @grade1Id, @cbcId, 1, @now, @now);

-- Grade 2
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Creation', '1', 'God''s creation', 1, @creId, @grade2Id, @cbcId, 1, @now, @now),
('The Holy Bible', '2', 'Books of the Bible and Bible stories', 2, @creId, @grade2Id, @cbcId, 1, @now, @now),
('The Life of Jesus Christ', '3', 'Teachings of Jesus', 3, @creId, @grade2Id, @cbcId, 1, @now, @now);

-- Grade 3
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Creation and Stewardship', '1', 'Stewardship of God''s creation', 1, @creId, @grade3Id, @cbcId, 1, @now, @now),
('The Holy Bible', '2', 'The Bible and Christian living', 2, @creId, @grade3Id, @cbcId, 1, @now, @now),
('The Life and Teachings of Jesus', '3', 'Miracles, passion, death and resurrection', 3, @creId, @grade3Id, @cbcId, 1, @now, @now);

-- Grade 4
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Creation and Stewardship', '1', 'Creation and human dignity', 1, @creId, @grade4Id, @cbcId, 1, @now, @now),
('Moral and Religious Values', '2', 'Values for positive living', 2, @creId, @grade4Id, @cbcId, 1, @now, @now);

-- Grade 5
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Religious Texts and Teachings', '1', 'Sacred texts', 1, @creId, @grade5Id, @cbcId, 1, @now, @now),
('Moral and Religious Values', '2', 'Family and social relationships', 2, @creId, @grade5Id, @cbcId, 1, @now, @now);

-- Grade 6
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Religion and Society', '1', 'Religion and national development', 1, @creId, @grade6Id, @cbcId, 1, @now, @now),
('Moral and Religious Values', '2', 'Contemporary issues', 2, @creId, @grade6Id, @cbcId, 1, @now, @now);

-- Grade 7
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Religious Texts and Teachings', '1', 'The Bible and Christian faith', 1, @creId, @grade7Id, @cbcId, 1, @now, @now),
('Moral and Religious Values', '2', 'Christian values in society', 2, @creId, @grade7Id, @cbcId, 1, @now, @now);

-- Grade 8
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Religious Texts and Teachings', '1', 'New Testament studies', 1, @creId, @grade8Id, @cbcId, 1, @now, @now),
('Moral and Religious Values', '2', 'Ethics and social justice', 2, @creId, @grade8Id, @cbcId, 1, @now, @now);

-- Grade 9
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Religious Texts and Teachings', '1', 'Church history and theology', 1, @creId, @grade9Id, @cbcId, 1, @now, @now),
('Moral and Religious Values', '2', 'Christian response to contemporary challenges', 2, @creId, @grade9Id, @cbcId, 1, @now, @now);

-- ============================================
-- IRE - Grades 1-9
-- ============================================

-- Grade 1
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Creation', '1', 'Self and family', 1, @ireId, @grade1Id, @cbcId, 1, @now, @now),
('The Holy Quran', '2', 'Introduction to the Quran', 2, @ireId, @grade1Id, @cbcId, 1, @now, @now),
('Pillars of Islam', '3', 'Shahadah and Salah', 3, @ireId, @grade1Id, @cbcId, 1, @now, @now);

-- Grade 2
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Creation', '1', 'God''s creation', 1, @ireId, @grade2Id, @cbcId, 1, @now, @now),
('The Holy Quran', '2', 'Reading the Quran', 2, @ireId, @grade2Id, @cbcId, 1, @now, @now),
('Pillars of Islam', '3', 'Acts of worship', 3, @ireId, @grade2Id, @cbcId, 1, @now, @now);

-- Grade 3
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Creation', '1', 'Stewardship', 1, @ireId, @grade3Id, @cbcId, 1, @now, @now),
('The Holy Quran', '2', 'Quranic recitation and meaning', 2, @ireId, @grade3Id, @cbcId, 1, @now, @now),
('Pillars of Islam', '3', 'Saum and Hajj', 3, @ireId, @grade3Id, @cbcId, 1, @now, @now);

-- Grade 4
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Islamic Teachings', '1', 'Islamic beliefs and creation', 1, @ireId, @grade4Id, @cbcId, 1, @now, @now),
('Moral and Religious Values', '2', 'Islamic values for positive living', 2, @ireId, @grade4Id, @cbcId, 1, @now, @now);

-- Grade 5
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Islamic Texts and Teachings', '1', 'Quranic studies', 1, @ireId, @grade5Id, @cbcId, 1, @now, @now),
('Moral and Religious Values', '2', 'Family and social relationships in Islam', 2, @ireId, @grade5Id, @cbcId, 1, @now, @now);

-- Grade 6
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Islam and Society', '1', 'Islam and national development', 1, @ireId, @grade6Id, @cbcId, 1, @now, @now),
('Moral and Religious Values', '2', 'Contemporary issues in Islam', 2, @ireId, @grade6Id, @cbcId, 1, @now, @now);

-- Grade 7
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Islamic Texts and Teachings', '1', 'Quran and Hadith studies', 1, @ireId, @grade7Id, @cbcId, 1, @now, @now),
('Moral and Religious Values', '2', 'Islamic values in society', 2, @ireId, @grade7Id, @cbcId, 1, @now, @now);

-- Grade 8
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Islamic Texts and Teachings', '1', 'Islamic jurisprudence', 1, @ireId, @grade8Id, @cbcId, 1, @now, @now),
('Moral and Religious Values', '2', 'Ethics and social justice in Islam', 2, @ireId, @grade8Id, @cbcId, 1, @now, @now);

-- Grade 9
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Islamic Texts and Teachings', '1', 'Islamic history and thought', 1, @ireId, @grade9Id, @cbcId, 1, @now, @now),
('Moral and Religious Values', '2', 'Islamic response to contemporary challenges', 2, @ireId, @grade9Id, @cbcId, 1, @now, @now);

-- ============================================
-- MOVEMENT AND CREATIVE ACTIVITIES - Grades 1-3
-- ============================================

-- Grade 1
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Physical Education', '1', 'Locomotor, non-locomotor and ball skills', 1, @artId, @grade1Id, @cbcId, 1, @now, @now),
('Creative Arts', '2', 'Visual arts, music and drama', 2, @artId, @grade1Id, @cbcId, 1, @now, @now);

-- Grade 2
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Physical Education', '1', 'Movement skills and games', 1, @artId, @grade2Id, @cbcId, 1, @now, @now),
('Creative Arts', '2', 'Visual arts, music and dance', 2, @artId, @grade2Id, @cbcId, 1, @now, @now);

-- Grade 3
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Physical Education', '1', 'Fitness, athletics and games', 1, @artId, @grade3Id, @cbcId, 1, @now, @now),
('Creative Arts', '2', 'Visual arts, music, drama and dance', 2, @artId, @grade3Id, @cbcId, 1, @now, @now);

-- ============================================
-- CREATIVE ARTS AND SPORTS - Grades 4-9
-- ============================================

-- Grade 4
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Visual Arts', '1', 'Drawing, painting and mixed media', 1, @artId, @grade4Id, @cbcId, 1, @now, @now),
('Music', '2', 'Singing and rhythm', 2, @artId, @grade4Id, @cbcId, 1, @now, @now),
('Physical and Health Education', '3', 'Athletics and games', 3, @artId, @grade4Id, @cbcId, 1, @now, @now);

-- Grade 5
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Visual Arts', '1', 'Indigenous Kenyan crafts', 1, @artId, @grade5Id, @cbcId, 1, @now, @now),
('Music', '2', 'Music theory and performance', 2, @artId, @grade5Id, @cbcId, 1, @now, @now),
('Physical and Health Education', '3', 'Games and sports', 3, @artId, @grade5Id, @cbcId, 1, @now, @now);

-- Grade 6
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Visual Arts', '1', 'Graphic design and photography', 1, @artId, @grade6Id, @cbcId, 1, @now, @now),
('Music', '2', 'Composition and performance', 2, @artId, @grade6Id, @cbcId, 1, @now, @now),
('Physical and Health Education', '3', 'Sports leadership and health', 3, @artId, @grade6Id, @cbcId, 1, @now, @now);

-- Grade 7
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Visual Arts', '1', 'Visual arts and design', 1, @artId, @grade7Id, @cbcId, 1, @now, @now),
('Music', '2', 'Music performance and appreciation', 2, @artId, @grade7Id, @cbcId, 1, @now, @now),
('Physical and Health Education', '3', 'Sports and fitness', 3, @artId, @grade7Id, @cbcId, 1, @now, @now);

-- Grade 8
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Visual Arts', '1', 'Advanced visual arts', 1, @artId, @grade8Id, @cbcId, 1, @now, @now),
('Music', '2', 'Advanced music', 2, @artId, @grade8Id, @cbcId, 1, @now, @now),
('Physical and Health Education', '3', 'Advanced sports and fitness', 3, @artId, @grade8Id, @cbcId, 1, @now, @now);

-- Grade 9
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Visual Arts', '1', 'Professional visual arts', 1, @artId, @grade9Id, @cbcId, 1, @now, @now),
('Music', '2', 'Music mastery', 2, @artId, @grade9Id, @cbcId, 1, @now, @now),
('Physical and Health Education', '3', 'Sports mastery and leadership', 3, @artId, @grade9Id, @cbcId, 1, @now, @now);

-- ============================================
-- SCIENCE AND TECHNOLOGY - Grades 4-6
-- ============================================

-- Grade 4
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Living Things and Their Environment', '1', 'Cells and classification of living things', 1, @sciId, @grade4Id, @cbcId, 1, @now, @now),
('Matter', '2', 'States of matter, solid waste and water conservation', 2, @sciId, @grade4Id, @cbcId, 1, @now, @now),
('Energy', '3', 'Forms of energy and electricity', 3, @sciId, @grade4Id, @cbcId, 1, @now, @now),
('Technology', '4', 'ICT and digital literacy', 4, @sciId, @grade4Id, @cbcId, 1, @now, @now);

-- Grade 5
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Living Things and Their Environment', '1', 'Nutrition in plants and animals, reproduction', 1, @sciId, @grade5Id, @cbcId, 1, @now, @now),
('Matter', '2', 'Mixtures, acids and bases', 2, @sciId, @grade5Id, @cbcId, 1, @now, @now),
('Energy', '3', 'Heat energy and magnetism', 3, @sciId, @grade5Id, @cbcId, 1, @now, @now),
('Technology', '4', 'Simple machines', 4, @sciId, @grade5Id, @cbcId, 1, @now, @now);

-- Grade 6
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Living Things and Their Environment', '1', 'Ecosystems, health and disease', 1, @sciId, @grade6Id, @cbcId, 1, @now, @now),
('Matter', '2', 'Chemical reactions', 2, @sciId, @grade6Id, @cbcId, 1, @now, @now),
('Energy', '3', 'Sound, light, renewable and non-renewable energy', 3, @sciId, @grade6Id, @cbcId, 1, @now, @now),
('Technology', '4', 'Innovation and invention', 4, @sciId, @grade6Id, @cbcId, 1, @now, @now);

-- ============================================
-- INTEGRATED SCIENCE - Grades 7-9
-- ============================================

-- Grade 7
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Scientific Investigation', '1', 'Scientific method', 1, @intSciId, @grade7Id, @cbcId, 1, @now, @now),
('Living Things and Their Environment', '2', 'Cell biology, nutrition and digestion', 2, @intSciId, @grade7Id, @cbcId, 1, @now, @now),
('Matter and Its Properties', '3', 'Atomic structure, chemical and physical changes', 3, @intSciId, @grade7Id, @cbcId, 1, @now, @now),
('Energy', '4', 'Heat, temperature, electricity and magnetism', 4, @intSciId, @grade7Id, @cbcId, 1, @now, @now);

-- Grade 8
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Living Things and Their Environment', '1', 'Reproduction and genetics', 1, @intSciId, @grade8Id, @cbcId, 1, @now, @now),
('Matter and Its Properties', '2', 'Periodic table and chemical reactions', 2, @intSciId, @grade8Id, @cbcId, 1, @now, @now),
('Force and Motion', '3', 'Mechanics', 3, @intSciId, @grade8Id, @cbcId, 1, @now, @now),
('Environment and Sustainability', '4', 'Ecology and conservation', 4, @intSciId, @grade8Id, @cbcId, 1, @now, @now);

-- Grade 9
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Living Things and Their Environment', '1', 'Evolution and classification', 1, @intSciId, @grade9Id, @cbcId, 1, @now, @now),
('Matter and Its Properties', '2', 'Acids, bases and salts', 2, @intSciId, @grade9Id, @cbcId, 1, @now, @now),
('Energy', '3', 'Waves, optics, nuclear energy and radioactivity', 3, @intSciId, @grade9Id, @cbcId, 1, @now, @now),
('Earth and Space Science', '4', 'Earth and the universe', 4, @intSciId, @grade9Id, @cbcId, 1, @now, @now);

-- ============================================
-- SOCIAL STUDIES - Grades 4-9
-- ============================================

-- Grade 4
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Our Environment', '1', 'Kenya''s physical environment and climate', 1, @socId, @grade4Id, @cbcId, 1, @now, @now),
('People and their Activities', '2', 'Population, settlement and economic activities', 2, @socId, @grade4Id, @cbcId, 1, @now, @now),
('Governance', '3', 'Government and citizenship', 3, @socId, @grade4Id, @cbcId, 1, @now, @now);

-- Grade 5
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Our Environment', '1', 'East Africa', 1, @socId, @grade5Id, @cbcId, 1, @now, @now),
('People and their Activities', '2', 'Economic integration in East Africa', 2, @socId, @grade5Id, @cbcId, 1, @now, @now),
('Governance', '3', 'Historical development', 3, @socId, @grade5Id, @cbcId, 1, @now, @now);

-- Grade 6
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Our Environment', '1', 'Africa', 1, @socId, @grade6Id, @cbcId, 1, @now, @now),
('People and their Activities', '2', 'Development and trade', 2, @socId, @grade6Id, @cbcId, 1, @now, @now),
('Governance', '3', 'Global citizenship', 3, @socId, @grade6Id, @cbcId, 1, @now, @now);

-- Grade 7
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Our Environment', '1', 'Our county and country', 1, @socId, @grade7Id, @cbcId, 1, @now, @now),
('People and their Activities', '2', 'People and economic activities', 2, @socId, @grade7Id, @cbcId, 1, @now, @now),
('Governance', '3', 'Governance and citizenship', 3, @socId, @grade7Id, @cbcId, 1, @now, @now);

-- Grade 8
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Our Environment', '1', 'Africa', 1, @socId, @grade8Id, @cbcId, 1, @now, @now),
('People and their Activities', '2', 'People and economic development', 2, @socId, @grade8Id, @cbcId, 1, @now, @now),
('Governance', '3', 'Historical development', 3, @socId, @grade8Id, @cbcId, 1, @now, @now);

-- Grade 9
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Our Environment', '1', 'Global issues', 1, @socId, @grade9Id, @cbcId, 1, @now, @now),
('People and their Activities', '2', 'People and global development', 2, @socId, @grade9Id, @cbcId, 1, @now, @now),
('Governance', '3', 'Global governance', 3, @socId, @grade9Id, @cbcId, 1, @now, @now);

-- ============================================
-- AGRICULTURE - Grades 4-9
-- ============================================

-- Grade 4
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Crop Production', '1', 'Soil preparation, planting and crop management', 1, @agrId, @grade4Id, @cbcId, 1, @now, @now),
('Animal Production', '2', 'Livestock keeping', 2, @agrId, @grade4Id, @cbcId, 1, @now, @now);

-- Grade 5
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Crop Production', '1', 'Crop husbandry', 1, @agrId, @grade5Id, @cbcId, 1, @now, @now),
('Animal Production', '2', 'Poultry and dairy farming', 2, @agrId, @grade5Id, @cbcId, 1, @now, @now),
('Farm Management', '3', 'Farm records and marketing', 3, @agrId, @grade5Id, @cbcId, 1, @now, @now);

-- Grade 6
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Crop Production', '1', 'Cash and food crops', 1, @agrId, @grade6Id, @cbcId, 1, @now, @now),
('Animal Production', '2', 'Apiculture and fish farming', 2, @agrId, @grade6Id, @cbcId, 1, @now, @now),
('Farm Management', '3', 'Agribusiness', 3, @agrId, @grade6Id, @cbcId, 1, @now, @now);

-- Grade 7
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Crop Production', '1', 'Crop production', 1, @agrId, @grade7Id, @cbcId, 1, @now, @now),
('Animal Production', '2', 'Animal production', 2, @agrId, @grade7Id, @cbcId, 1, @now, @now),
('Farm Management', '3', 'Farm management', 3, @agrId, @grade7Id, @cbcId, 1, @now, @now);

-- Grade 8
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Crop Production', '1', 'Advanced crop production', 1, @agrId, @grade8Id, @cbcId, 1, @now, @now),
('Animal Production', '2', 'Advanced animal production', 2, @agrId, @grade8Id, @cbcId, 1, @now, @now),
('Farm Management', '3', 'Advanced farm management', 3, @agrId, @grade8Id, @cbcId, 1, @now, @now);

-- Grade 9
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Crop Production', '1', 'Sustainable crop production', 1, @agrId, @grade9Id, @cbcId, 1, @now, @now),
('Animal Production', '2', 'Sustainable animal production', 2, @agrId, @grade9Id, @cbcId, 1, @now, @now),
('Farm Management', '3', 'Agribusiness and food security', 3, @agrId, @grade9Id, @cbcId, 1, @now, @now);

-- ============================================
-- PRE-TECHNICAL STUDIES - Grades 7-9
-- ============================================

-- Grade 7
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Technical Drawing', '1', 'Technical drawing fundamentals', 1, @preTechId, @grade7Id, @cbcId, 1, @now, @now),
('Electronics', '2', 'Basic electronics', 2, @preTechId, @grade7Id, @cbcId, 1, @now, @now),
('Materials and Technology', '3', 'Materials and technology', 3, @preTechId, @grade7Id, @cbcId, 1, @now, @now);

-- Grade 8
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Technical Drawing', '1', 'Intermediate technical drawing', 1, @preTechId, @grade8Id, @cbcId, 1, @now, @now),
('Electronics', '2', 'Intermediate electronics', 2, @preTechId, @grade8Id, @cbcId, 1, @now, @now),
('Mechanisms', '3', 'Mechanisms and motion', 3, @preTechId, @grade8Id, @cbcId, 1, @now, @now);

-- Grade 9
INSERT INTO strands (Name, Code, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified) VALUES
('Design and Technology', '1', 'Design and technology', 1, @preTechId, @grade9Id, @cbcId, 1, @now, @now),
('Electronics', '2', 'Advanced electronics', 2, @preTechId, @grade9Id, @cbcId, 1, @now, @now),
('Materials and Fabrication', '3', 'Materials and fabrication', 3, @preTechId, @grade9Id, @cbcId, 1, @now, @now);

-- ============================================================
-- STEP 4: INSERT SUB-STRANDS
-- ============================================================

-- ============================================
-- MATHEMATICS Sub-Strands - Grade 1
-- ============================================

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Number Concept', '1.1', 'Sorting, grouping, pairing, ordering and counting', 1, s.Id, @cbcId, @mathId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Whole Numbers', '1.2', 'Counting, place value, reading and writing numbers', 2, s.Id, @cbcId, @mathId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Addition', '1.3', 'Adding numbers', 3, s.Id, @cbcId, @mathId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Subtraction', '1.4', 'Subtracting numbers', 4, s.Id, @cbcId, @mathId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Length', '2.1', 'Comparing and measuring length', 1, s.Id, @cbcId, @mathId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mass', '2.2', 'Comparing and measuring mass', 2, s.Id, @cbcId, @mathId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Capacity', '2.3', 'Comparing and measuring capacity', 3, s.Id, @cbcId, @mathId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Time', '2.4', 'Daily activities and days of the week', 4, s.Id, @cbcId, @mathId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Money', '2.5', 'Kenyan currency and goods and services', 5, s.Id, @cbcId, @mathId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Lines', '3.1', 'Straight and curved lines', 1, s.Id, @cbcId, @mathId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Shapes', '3.2', 'Rectangles, circles and triangles', 2, s.Id, @cbcId, @mathId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade1Id;

-- ============================================
-- MATHEMATICS Sub-Strands - Grade 2
-- ============================================

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Number Concept', '1.1', 'Reading and representing numbers 1-100', 1, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Whole Numbers', '1.2', 'Counting, place value and number patterns', 2, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Addition', '1.3', 'Adding 2-digit numbers', 3, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Subtraction', '1.4', 'Subtracting 2-digit numbers', 4, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Multiplication', '1.5', 'Multiplication as repeated addition', 5, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Division', '1.6', 'Division as sharing equally', 6, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Length', '2.1', 'Measuring length using non-standard units', 1, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mass', '2.2', 'Measuring mass using non-standard units', 2, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Capacity', '2.3', 'Measuring capacity using non-standard units', 3, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Time', '2.4', 'Reading time and calendar', 4, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Money', '2.5', 'Kenyan currency and transactions', 5, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Lines', '3.1', 'Horizontal, vertical and slanted lines', 1, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Shapes', '3.2', 'Squares, rectangles, triangles and circles', 2, s.Id, @cbcId, @mathId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade2Id;

-- ============================================
-- MATHEMATICS Sub-Strands - Grade 3
-- ============================================

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Number Concept', '1.1', 'Reading and representing numbers up to 999', 1, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Whole Numbers', '1.2', 'Counting, place value and number patterns', 2, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Addition', '1.3', 'Adding 3-digit numbers', 3, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Subtraction', '1.4', 'Subtracting 3-digit numbers', 4, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Multiplication', '1.5', 'Multiplication tables and multiplication', 5, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Division', '1.6', 'Division using tables', 6, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Fractions', '1.7', 'Simple fractions', 7, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Length', '2.1', 'Measuring length using standard units', 1, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mass', '2.2', 'Measuring mass using standard units', 2, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Capacity', '2.3', 'Measuring capacity using standard units', 3, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Time', '2.4', 'Reading time and calendar', 4, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Money', '2.5', 'Money operations and word problems', 5, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Lines', '3.1', 'Parallel and perpendicular lines', 1, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Shapes', '3.2', '3D shapes', 2, s.Id, @cbcId, @mathId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade3Id;

-- ============================================
-- MATHEMATICS Sub-Strands - Grade 4
-- ============================================

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Whole Numbers', '1.1', 'Place value, total value, reading and writing numbers up to millions', 1, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Fractions', '1.2', 'Proper, improper and mixed fractions', 2, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Decimals', '1.3', 'Decimals up to thousandths', 3, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Percentages', '1.4', 'Fractions, decimals and percentages', 4, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Length', '2.1', 'Converting units and calculating perimeter', 1, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Area', '2.2', 'Area of squares and rectangles', 2, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mass', '2.3', 'Converting between grams and kilograms', 3, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Capacity', '2.4', 'Converting between millilitres and litres', 4, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Time', '2.5', 'Time in 12-hour and 24-hour format', 5, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Money', '2.6', 'Money operations, profit and loss', 6, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Lines and Angles', '3.1', 'Types of angles and lines', 1, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Plane Shapes', '3.2', 'Properties of triangles, quadrilaterals and circles', 2, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Symmetry', '3.3', 'Lines of symmetry in plane shapes', 3, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Data Collection and Representation', '4.1', 'Tallies, pictograms and bar graphs', 1, s.Id, @cbcId, @mathId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Data Handling' AND s.Code = '4' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade4Id;

-- ============================================
-- MATHEMATICS Sub-Strands - Grade 5
-- ============================================

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Whole Numbers', '1.1', 'Place value up to tens of millions, LCM and GCD', 1, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Fractions', '1.2', 'Operations with fractions and mixed numbers', 2, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Decimals', '1.3', 'Operations with decimals', 3, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Percentages', '1.4', 'Percentage increase and decrease', 4, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Ratio and Proportion', '1.5', 'Ratios and direct proportion', 5, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Length and Perimeter', '2.1', 'Perimeter of plane shapes', 1, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Area', '2.2', 'Area of triangles, parallelograms and trapezoids', 2, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Volume', '2.3', 'Volume of cubes and cuboids', 3, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Time Speed and Distance', '2.4', 'Calculating speed, distance and time', 4, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Money and Financial Literacy', '2.5', 'Simple interest, tax and discount', 5, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Angles', '3.1', 'Angles in triangles and quadrilaterals', 1, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Plane Figures', '3.2', 'Constructing triangles and quadrilaterals, circles', 2, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT '3D Shapes', '3.3', '3D shapes, nets and surface area', 3, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Statistics', '4.1', 'Frequency tables, bar graphs, line graphs, mean, median, mode', 1, s.Id, @cbcId, @mathId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Data Handling' AND s.Code = '4' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade5Id;

-- ============================================
-- MATHEMATICS Sub-Strands - Grade 6
-- ============================================

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Whole Numbers', '1.1', 'Place value up to hundreds of millions', 1, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Multiplication', '1.2', 'Multiplying up to 4-digit by 2-digit numbers', 2, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Division', '1.3', 'Dividing up to 4-digit by 3-digit numbers', 3, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Fractions', '1.4', 'Fractions using LCM, mixed numbers, reciprocals', 4, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Decimals', '1.5', 'Operations with decimals, conversions', 5, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Inequalities', '1.6', 'Inequality symbols and number line', 6, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Length', '2.1', 'Units of length and perimeter', 1, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Area', '2.2', 'Area of rectangles, triangles and circles', 2, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Capacity', '2.3', 'Converting units of capacity', 3, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mass', '2.4', 'Converting units of mass', 4, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Time', '2.5', 'Time formats and time zones', 5, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Money', '2.6', 'Profit, loss and simple interest', 6, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurement' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Lines', '3.1', 'Parallel, perpendicular, intersecting and transversal lines', 1, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Angles', '3.2', 'Classifying and calculating angles in polygons', 2, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT '3-D Objects', '3.3', 'Properties, nets, surface area and volume', 3, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Bar Graphs', '4.1', 'Drawing and interpreting bar graphs', 1, s.Id, @cbcId, @mathId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Data Handling' AND s.Code = '4' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade6Id;

-- ============================================
-- MATHEMATICS Sub-Strands - Grade 7
-- ============================================

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Whole Numbers', '1.1', 'Whole numbers and operations', 1, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Fractions', '1.2', 'Fractions and operations', 2, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Squares and Square Roots', '1.3', 'Squares and square roots', 3, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Integers', '1.4', 'Integer operations', 4, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Algebraic Expressions', '2.1', 'Algebraic expressions', 1, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Algebra' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Linear Equations', '2.2', 'Linear equations', 2, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Algebra' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Length and Area', '3.1', 'Length and area', 1, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Volume and Capacity', '3.2', 'Volume and capacity', 2, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mass and Density', '3.3', 'Mass and density', 3, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Time and Speed', '3.4', 'Time and speed', 4, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Money and Financial Arithmetic', '3.5', 'Money and financial arithmetic', 5, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Angles and Lines', '4.1', 'Angles and lines', 1, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '4' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Plane Figures', '4.2', 'Plane figures', 2, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '4' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Scale Drawing', '4.3', 'Scale drawing', 3, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '4' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Statistics', '5.1', 'Statistics', 1, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Data Handling and Probability' AND s.Code = '5' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Probability', '5.2', 'Probability', 2, s.Id, @cbcId, @mathId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Data Handling and Probability' AND s.Code = '5' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade7Id;

-- ============================================
-- MATHEMATICS Sub-Strands - Grade 8
-- ============================================

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Further Number Work', '1.1', 'Further number work', 1, s.Id, @cbcId, @mathId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Cubes and Cube Roots', '1.2', 'Cubes and cube roots', 2, s.Id, @cbcId, @mathId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Simultaneous Equations', '2.1', 'Simultaneous equations', 1, s.Id, @cbcId, @mathId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Algebra' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Quadratic Expressions', '2.2', 'Quadratic expressions', 2, s.Id, @cbcId, @mathId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Algebra' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Area of Plane Shapes', '3.1', 'Area of plane shapes', 1, s.Id, @cbcId, @mathId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Surface Area and Volume', '3.2', 'Surface area and volume', 2, s.Id, @cbcId, @mathId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Money and Finance', '3.3', 'Money and finance', 3, s.Id, @cbcId, @mathId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Transformation', '4.1', 'Transformation', 1, s.Id, @cbcId, @mathId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '4' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Circles', '4.2', 'Circles', 2, s.Id, @cbcId, @mathId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '4' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Data Interpretation', '5.1', 'Data interpretation', 1, s.Id, @cbcId, @mathId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Data Handling and Probability' AND s.Code = '5' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Probability', '5.2', 'Probability', 2, s.Id, @cbcId, @mathId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Data Handling and Probability' AND s.Code = '5' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade8Id;

-- ============================================
-- MATHEMATICS Sub-Strands - Grade 9
-- ============================================

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Integers', '1.1', 'Integers', 1, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Cubes and Cube Roots', '1.2', 'Cubes and cube roots', 2, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Indices and Logarithms', '1.3', 'Indices and logarithms', 3, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Compound Proportions and Rates of Work', '1.4', 'Compound proportions and rates of work', 4, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Numbers' AND s.Code = '1' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Matrices', '2.1', 'Matrices', 1, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Algebra' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Equation of a Straight Line', '2.2', 'Equation of a straight line', 2, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Algebra' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Linear Inequalities', '2.3', 'Linear inequalities', 3, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Algebra' AND s.Code = '2' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Area', '3.1', 'Area', 1, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Volume of Solids', '3.2', 'Volume of solids', 2, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mass Volume Weight and Density', '3.3', 'Mass, volume, weight and density', 3, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Time Distance and Speed', '3.4', 'Time, distance and speed', 4, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Money', '3.5', 'Money', 5, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Approximations and Errors', '3.6', 'Approximations and errors', 6, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Measurements' AND s.Code = '3' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Coordinates and Graphs', '4.1', 'Coordinates and graphs', 1, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '4' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Scale Drawing', '4.2', 'Scale drawing', 2, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '4' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Similarity and Enlargement', '4.3', 'Similarity and enlargement', 3, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '4' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Trigonometry', '4.4', 'Trigonometry', 4, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Geometry' AND s.Code = '4' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Data Interpretation (Grouped Data)', '5.1', 'Data interpretation with grouped data', 1, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Data Handling and Probability' AND s.Code = '5' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Probability', '5.2', 'Probability', 2, s.Id, @cbcId, @mathId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Data Handling and Probability' AND s.Code = '5' AND s.SubjectId = @mathId AND s.LearningLevelId = @grade9Id;

-- ============================================
-- LITERACY ACTIVITIES Sub-Strands - Grades 1-3
-- ============================================

-- Grade 1
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Listening', '1.1', 'Listening skills', 1, s.Id, @cbcId, @litId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @litId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Speaking', '1.2', 'Speaking skills', 2, s.Id, @cbcId, @litId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @litId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Phonological Awareness', '2.1', 'Phonological awareness', 1, s.Id, @cbcId, @litId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @litId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Phonics', '2.2', 'Phonics', 2, s.Id, @cbcId, @litId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @litId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Reading Comprehension', '2.3', 'Reading comprehension', 3, s.Id, @cbcId, @litId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @litId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Pre-writing', '3.1', 'Pre-writing skills', 1, s.Id, @cbcId, @litId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @litId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Writing Sentences', '3.2', 'Writing sentences', 2, s.Id, @cbcId, @litId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @litId AND s.LearningLevelId = @grade1Id;

-- Literacy Grade 2
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Oral Communication', '1.1', 'Oral communication', 1, s.Id, @cbcId, @litId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @litId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Word Recognition', '2.1', 'Word recognition', 1, s.Id, @cbcId, @litId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @litId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Reading Comprehension', '2.2', 'Reading comprehension', 2, s.Id, @cbcId, @litId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @litId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Guided Writing', '3.1', 'Guided writing', 1, s.Id, @cbcId, @litId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @litId AND s.LearningLevelId = @grade2Id;

-- Literacy Grade 3
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Oral Communication', '1.1', 'Oral communication', 1, s.Id, @cbcId, @litId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @litId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Fluency and Comprehension', '2.1', 'Fluency and comprehension', 1, s.Id, @cbcId, @litId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @litId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Vocabulary', '2.2', 'Vocabulary development', 2, s.Id, @cbcId, @litId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @litId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Creative Writing', '3.1', 'Creative writing', 1, s.Id, @cbcId, @litId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @litId AND s.LearningLevelId = @grade3Id;

-- ============================================
-- ENGLISH LANGUAGE ACTIVITIES Sub-Strands - Grades 1-3
-- ============================================

-- Grade 1
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Listening', '1.1', 'Listening in English', 1, s.Id, @cbcId, @engLangId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Speaking', '1.2', 'Speaking in English', 2, s.Id, @cbcId, @engLangId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Phonics and Word Study', '2.1', 'Phonics and word study', 1, s.Id, @cbcId, @engLangId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Reading Comprehension', '2.2', 'Reading comprehension', 2, s.Id, @cbcId, @engLangId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Early Writing', '3.1', 'Early writing', 1, s.Id, @cbcId, @engLangId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Grammar and Vocabulary', '4.1', 'Grammar and vocabulary', 1, s.Id, @cbcId, @engLangId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Language Use' AND s.Code = '4' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade1Id;

-- English Lang Grade 2
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Oral Communication', '1.1', 'Oral communication', 1, s.Id, @cbcId, @engLangId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Reading Fluency', '2.1', 'Reading fluency', 1, s.Id, @cbcId, @engLangId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Comprehension', '2.2', 'Comprehension', 2, s.Id, @cbcId, @engLangId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Guided Writing', '3.1', 'Guided writing', 1, s.Id, @cbcId, @engLangId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Grammar', '4.1', 'Grammar', 1, s.Id, @cbcId, @engLangId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Language Use' AND s.Code = '4' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade2Id;

-- English Lang Grade 3
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Oral Communication', '1.1', 'Oral communication', 1, s.Id, @cbcId, @engLangId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Comprehension and Critical Reading', '2.1', 'Comprehension and critical reading', 1, s.Id, @cbcId, @engLangId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Composition', '3.1', 'Composition', 1, s.Id, @cbcId, @engLangId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Grammar', '4.1', 'Grammar', 1, s.Id, @cbcId, @engLangId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Language Use' AND s.Code = '4' AND s.SubjectId = @engLangId AND s.LearningLevelId = @grade3Id;

-- ============================================
-- ENGLISH Sub-Strands - Grades 4-9
-- ============================================

-- Grade 4
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Listening Comprehension', '1.1', 'Listening comprehension', 1, s.Id, @cbcId, @engId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @engId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Oral Communication', '1.2', 'Oral communication', 2, s.Id, @cbcId, @engId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @engId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Reading Fluency and Comprehension', '2.1', 'Reading fluency and comprehension', 1, s.Id, @cbcId, @engId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @engId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Vocabulary and Word Study', '2.2', 'Vocabulary and word study', 2, s.Id, @cbcId, @engId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @engId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Functional Writing', '3.1', 'Functional writing', 1, s.Id, @cbcId, @engId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @engId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Creative Writing', '3.2', 'Creative writing', 2, s.Id, @cbcId, @engId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @engId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Grammar', '4.1', 'Grammar', 1, s.Id, @cbcId, @engId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Language Use' AND s.Code = '4' AND s.SubjectId = @engId AND s.LearningLevelId = @grade4Id;

-- English Grade 5
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Listening and Speaking', '1.1', 'Listening and speaking', 1, s.Id, @cbcId, @engId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @engId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Reading and Comprehension', '2.1', 'Reading and comprehension', 1, s.Id, @cbcId, @engId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @engId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Writing', '3.1', 'Writing', 1, s.Id, @cbcId, @engId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @engId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Grammar and Usage', '4.1', 'Grammar and usage', 1, s.Id, @cbcId, @engId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Language Use' AND s.Code = '4' AND s.SubjectId = @engId AND s.LearningLevelId = @grade5Id;

-- English Grade 6
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Oral Skills', '1.1', 'Advanced oral skills', 1, s.Id, @cbcId, @engId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @engId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Critical Reading', '2.1', 'Critical reading', 1, s.Id, @cbcId, @engId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @engId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Composition and Functional Writing', '3.1', 'Composition and functional writing', 1, s.Id, @cbcId, @engId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @engId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Grammar', '4.1', 'Grammar', 1, s.Id, @cbcId, @engId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Language Use' AND s.Code = '4' AND s.SubjectId = @engId AND s.LearningLevelId = @grade6Id;

-- English Grade 7
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Listening and Speaking', '1.1', 'Listening and speaking', 1, s.Id, @cbcId, @engId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @engId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Reading and Comprehension', '2.1', 'Reading and comprehension', 1, s.Id, @cbcId, @engId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @engId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Writing', '3.1', 'Writing', 1, s.Id, @cbcId, @engId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @engId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Grammar and Usage', '4.1', 'Grammar and usage', 1, s.Id, @cbcId, @engId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Language Use' AND s.Code = '4' AND s.SubjectId = @engId AND s.LearningLevelId = @grade7Id;

-- English Grade 8
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Oral Communication', '1.1', 'Advanced oral communication', 1, s.Id, @cbcId, @engId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @engId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Critical and Analytical Reading', '2.1', 'Critical and analytical reading', 1, s.Id, @cbcId, @engId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @engId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Writing', '3.1', 'Advanced writing', 1, s.Id, @cbcId, @engId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @engId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Grammar', '4.1', 'Advanced grammar', 1, s.Id, @cbcId, @engId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Language Use' AND s.Code = '4' AND s.SubjectId = @engId AND s.LearningLevelId = @grade8Id;

-- English Grade 9
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mastery of Oral Communication', '1.1', 'Mastery of oral communication', 1, s.Id, @cbcId, @engId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Listening and Speaking' AND s.Code = '1' AND s.SubjectId = @engId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Critical Reading', '2.1', 'Advanced critical reading', 1, s.Id, @cbcId, @engId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Reading' AND s.Code = '2' AND s.SubjectId = @engId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Composition', '3.1', 'Advanced composition', 1, s.Id, @cbcId, @engId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Writing' AND s.Code = '3' AND s.SubjectId = @engId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Language Use', '4.1', 'Advanced language use', 1, s.Id, @cbcId, @engId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Language Use' AND s.Code = '4' AND s.SubjectId = @engId AND s.LearningLevelId = @grade9Id;

-- ============================================
-- KISWAHILI Sub-Strands - Grades 1-9
-- ============================================

-- Grade 1
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Kusikiliza', '1.1', 'Kusikiliza', 1, s.Id, @cbcId, @kisId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusikiliza na Kuzungumza' AND s.Code = '1' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Kuzungumza', '1.2', 'Kuzungumza', 2, s.Id, @cbcId, @kisId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusikiliza na Kuzungumza' AND s.Code = '1' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Fonetiki', '2.1', 'Fonetiki', 1, s.Id, @cbcId, @kisId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusoma' AND s.Code = '2' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Ufahamu', '2.2', 'Ufahamu', 2, s.Id, @cbcId, @kisId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusoma' AND s.Code = '2' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Uandishi wa Awali', '3.1', 'Uandishi wa awali', 1, s.Id, @cbcId, @kisId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kuandika' AND s.Code = '3' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Msamiati na Sarufi', '4.1', 'Msamiati na sarufi', 1, s.Id, @cbcId, @kisId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Sarufi' AND s.Code = '4' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade1Id;

-- Kiswahili Grade 2
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mawasiliano ya Mdomo', '1.1', 'Mawasiliano ya mdomo', 1, s.Id, @cbcId, @kisId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusikiliza na Kuzungumza' AND s.Code = '1' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Ufasaha wa Kusoma', '2.1', 'Ufasaha wa kusoma', 1, s.Id, @cbcId, @kisId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusoma' AND s.Code = '2' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Ufahamu', '2.2', 'Ufahamu', 2, s.Id, @cbcId, @kisId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusoma' AND s.Code = '2' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Uandishi wa Mwongozo', '3.1', 'Uandishi wa mwongozo', 1, s.Id, @cbcId, @kisId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kuandika' AND s.Code = '3' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sarufi na Msamiati', '4.1', 'Sarufi na msamiati', 1, s.Id, @cbcId, @kisId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Sarufi' AND s.Code = '4' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade2Id;

-- Kiswahili Grade 3
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mawasiliano ya Mdomo', '1.1', 'Mawasiliano ya mdomo', 1, s.Id, @cbcId, @kisId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusikiliza na Kuzungumza' AND s.Code = '1' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Ufahamu na Usomaji wa Kina', '2.1', 'Ufahamu na usomaji wa kina', 1, s.Id, @cbcId, @kisId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusoma' AND s.Code = '2' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Insha', '3.1', 'Insha', 1, s.Id, @cbcId, @kisId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kuandika' AND s.Code = '3' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sarufi ya Kina', '4.1', 'Sarufi ya kina', 1, s.Id, @cbcId, @kisId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Sarufi' AND s.Code = '4' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade3Id;

-- Kiswahili Grade 4
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Kusikiliza na Kuzungumza', '1.1', 'Kusikiliza na kuzungumza', 1, s.Id, @cbcId, @kisId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusikiliza na Kuzungumza' AND s.Code = '1' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Kusoma na Ufahamu', '2.1', 'Kusoma na ufahamu', 1, s.Id, @cbcId, @kisId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusoma' AND s.Code = '2' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Uandishi', '3.1', 'Uandishi', 1, s.Id, @cbcId, @kisId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kuandika' AND s.Code = '3' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sarufi', '4.1', 'Sarufi', 1, s.Id, @cbcId, @kisId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Sarufi' AND s.Code = '4' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade4Id;

-- Kiswahili Grade 5
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mawasiliano ya Hali ya Juu', '1.1', 'Mawasiliano ya hali ya juu', 1, s.Id, @cbcId, @kisId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusikiliza na Kuzungumza' AND s.Code = '1' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Usomaji wa Kina', '2.1', 'Usomaji wa kina', 1, s.Id, @cbcId, @kisId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusoma' AND s.Code = '2' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Uandishi wa Kina', '3.1', 'Uandishi wa kina', 1, s.Id, @cbcId, @kisId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kuandika' AND s.Code = '3' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sarufi ya Kina', '4.1', 'Sarufi ya kina', 1, s.Id, @cbcId, @kisId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Sarufi' AND s.Code = '4' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade5Id;

-- Kiswahili Grade 6
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mawasiliano ya Hali ya Juu Zaidi', '1.1', 'Mawasiliano ya hali ya juu zaidi', 1, s.Id, @cbcId, @kisId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusikiliza na Kuzungumza' AND s.Code = '1' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Usomaji wa Kina na Uchanganuzi', '2.1', 'Usomaji wa kina na uchanganuzi', 1, s.Id, @cbcId, @kisId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusoma' AND s.Code = '2' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Uandishi wa Kina Zaidi', '3.1', 'Uandishi wa kina zaidi', 1, s.Id, @cbcId, @kisId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kuandika' AND s.Code = '3' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sarufi ya Hali ya Juu', '4.1', 'Sarufi ya hali ya juu', 1, s.Id, @cbcId, @kisId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Sarufi' AND s.Code = '4' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade6Id;

-- Kiswahili Grade 7
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mawasiliano ya Juu', '1.1', 'Mawasiliano ya juu', 1, s.Id, @cbcId, @kisId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusikiliza na Kuzungumza' AND s.Code = '1' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Usomaji wa Kina', '2.1', 'Usomaji wa kina', 1, s.Id, @cbcId, @kisId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusoma' AND s.Code = '2' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Uandishi wa Hali ya Juu', '3.1', 'Uandishi wa hali ya juu', 1, s.Id, @cbcId, @kisId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kuandika' AND s.Code = '3' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sarufi ya Kina', '4.1', 'Sarufi ya kina', 1, s.Id, @cbcId, @kisId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Sarufi' AND s.Code = '4' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade7Id;

-- Kiswahili Grade 8
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mawasiliano ya Hali ya Juu Zaidi', '1.1', 'Mawasiliano ya hali ya juu zaidi', 1, s.Id, @cbcId, @kisId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusikiliza na Kuzungumza' AND s.Code = '1' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Usomaji Makini na Uchanganuzi', '2.1', 'Usomaji makini na uchanganuzi', 1, s.Id, @cbcId, @kisId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusoma' AND s.Code = '2' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Uandishi wa Kina na Uchanganuzi', '3.1', 'Uandishi wa kina na uchanganuzi', 1, s.Id, @cbcId, @kisId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kuandika' AND s.Code = '3' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade8Id;

-- Kiswahili Grade 9
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Umahiri wa Mawasiliano ya Mdomo', '1.1', 'Umahiri wa mawasiliano ya mdomo', 1, s.Id, @cbcId, @kisId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusikiliza na Kuzungumza' AND s.Code = '1' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Usomaji wa Kina wa Hali ya Juu', '2.1', 'Usomaji wa kina wa hali ya juu', 1, s.Id, @cbcId, @kisId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kusoma' AND s.Code = '2' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Uandishi wa Hali ya Juu Zaidi', '3.1', 'Uandishi wa hali ya juu zaidi', 1, s.Id, @cbcId, @kisId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Kuandika' AND s.Code = '3' AND s.SubjectId = @kisId AND s.LearningLevelId = @grade9Id;

-- ============================================
-- ENVIRONMENTAL ACTIVITIES Sub-Strands - Grades 1-3
-- ============================================

-- Grade 1
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'My Home', '1.1', 'My home', 1, s.Id, @cbcId, @envId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Home and School' AND s.Code = '1' AND s.SubjectId = @envId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'My School', '1.2', 'My school', 2, s.Id, @cbcId, @envId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Home and School' AND s.Code = '1' AND s.SubjectId = @envId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'People Who Help Us', '1.3', 'People who help us', 3, s.Id, @cbcId, @envId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Home and School' AND s.Code = '1' AND s.SubjectId = @envId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Plants', '2.1', 'Plants', 1, s.Id, @cbcId, @envId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Plants and Animals' AND s.Code = '2' AND s.SubjectId = @envId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Animals', '2.2', 'Animals', 2, s.Id, @cbcId, @envId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Plants and Animals' AND s.Code = '2' AND s.SubjectId = @envId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Soil', '3.1', 'Soil', 1, s.Id, @cbcId, @envId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Environment' AND s.Code = '3' AND s.SubjectId = @envId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Water', '3.2', 'Water', 2, s.Id, @cbcId, @envId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Environment' AND s.Code = '3' AND s.SubjectId = @envId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Air', '3.3', 'Air', 3, s.Id, @cbcId, @envId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Environment' AND s.Code = '3' AND s.SubjectId = @envId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Weather', '4.1', 'Weather', 1, s.Id, @cbcId, @envId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Weather and Climate' AND s.Code = '4' AND s.SubjectId = @envId AND s.LearningLevelId = @grade1Id;

-- Environmental Grade 2
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Home Environment', '1.1', 'Home environment', 1, s.Id, @cbcId, @envId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Home and School' AND s.Code = '1' AND s.SubjectId = @envId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'School Environment', '1.2', 'School environment', 2, s.Id, @cbcId, @envId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Home and School' AND s.Code = '1' AND s.SubjectId = @envId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Plants', '2.1', 'Plants', 1, s.Id, @cbcId, @envId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Plants and Animals' AND s.Code = '2' AND s.SubjectId = @envId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Animals', '2.2', 'Animals', 2, s.Id, @cbcId, @envId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Plants and Animals' AND s.Code = '2' AND s.SubjectId = @envId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Soil', '3.1', 'Soil', 1, s.Id, @cbcId, @envId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Environment' AND s.Code = '3' AND s.SubjectId = @envId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Water', '3.2', 'Water', 2, s.Id, @cbcId, @envId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Environment' AND s.Code = '3' AND s.SubjectId = @envId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Rocks and Minerals', '3.3', 'Rocks and minerals', 3, s.Id, @cbcId, @envId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Environment' AND s.Code = '3' AND s.SubjectId = @envId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Seasons', '4.1', 'Seasons', 1, s.Id, @cbcId, @envId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Weather and Climate' AND s.Code = '4' AND s.SubjectId = @envId AND s.LearningLevelId = @grade2Id;

-- Environmental Grade 3
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Types of Communities', '1.1', 'Types of communities', 1, s.Id, @cbcId, @envId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Our Community' AND s.Code = '1' AND s.SubjectId = @envId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Transport and Communication', '1.2', 'Transport and communication', 2, s.Id, @cbcId, @envId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Our Community' AND s.Code = '1' AND s.SubjectId = @envId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Plants', '2.1', 'Plants', 1, s.Id, @cbcId, @envId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Plants and Animals' AND s.Code = '2' AND s.SubjectId = @envId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Animals', '2.2', 'Animals', 2, s.Id, @cbcId, @envId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Plants and Animals' AND s.Code = '2' AND s.SubjectId = @envId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Soil', '3.1', 'Soil', 1, s.Id, @cbcId, @envId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Environment' AND s.Code = '3' AND s.SubjectId = @envId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Water', '3.2', 'Water', 2, s.Id, @cbcId, @envId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Environment' AND s.Code = '3' AND s.SubjectId = @envId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Simple Maps', '3.3', 'Simple maps', 3, s.Id, @cbcId, @envId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Environment' AND s.Code = '3' AND s.SubjectId = @envId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sources of Energy', '4.1', 'Sources of energy', 1, s.Id, @cbcId, @envId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Energy' AND s.Code = '4' AND s.SubjectId = @envId AND s.LearningLevelId = @grade3Id;

-- ============================================
-- HYGIENE AND NUTRITION Sub-Strands - Grades 1-3
-- ============================================

-- Grade 1
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Personal Hygiene', '1.1', 'Personal hygiene', 1, s.Id, @cbcId, @hygId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Hygiene' AND s.Code = '1' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Environmental Hygiene', '1.2', 'Environmental hygiene', 2, s.Id, @cbcId, @hygId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Hygiene' AND s.Code = '1' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Food Groups', '2.1', 'Food groups', 1, s.Id, @cbcId, @hygId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Nutrition' AND s.Code = '2' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Safe Food Handling', '2.2', 'Safe food handling', 2, s.Id, @cbcId, @hygId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Nutrition' AND s.Code = '2' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Personal Safety', '3.1', 'Personal safety', 1, s.Id, @cbcId, @hygId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Safety' AND s.Code = '3' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade1Id;

-- Hygiene Grade 2
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Personal Hygiene', '1.1', 'Personal hygiene', 1, s.Id, @cbcId, @hygId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Hygiene' AND s.Code = '1' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Environmental Hygiene', '1.2', 'Environmental hygiene', 2, s.Id, @cbcId, @hygId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Hygiene' AND s.Code = '1' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Meal Planning', '2.1', 'Meal planning', 1, s.Id, @cbcId, @hygId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Nutrition' AND s.Code = '2' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Water and Beverages', '2.2', 'Water and beverages', 2, s.Id, @cbcId, @hygId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Nutrition' AND s.Code = '2' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Home and School Safety', '3.1', 'Home and school safety', 1, s.Id, @cbcId, @hygId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Safety' AND s.Code = '3' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade2Id;

-- Hygiene Grade 3
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Personal and Environmental Hygiene', '1.1', 'Personal and environmental hygiene', 1, s.Id, @cbcId, @hygId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Hygiene' AND s.Code = '1' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Reproductive Health', '1.2', 'Reproductive health', 2, s.Id, @cbcId, @hygId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Hygiene' AND s.Code = '1' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Nutrients', '2.1', 'Nutrients', 1, s.Id, @cbcId, @hygId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Nutrition' AND s.Code = '2' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Food Preparation and Storage', '2.2', 'Food preparation and storage', 2, s.Id, @cbcId, @hygId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Nutrition' AND s.Code = '2' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'First Aid', '3.1', 'First aid', 1, s.Id, @cbcId, @hygId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Safety' AND s.Code = '3' AND s.SubjectId = @hygId AND s.LearningLevelId = @grade3Id;

-- ============================================
-- CRE Sub-Strands - Grades 1-9
-- ============================================

-- CRE Grade 1
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Self-Awareness', '1.1', 'Self-awareness', 1, s.Id, @cbcId, @creId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creation' AND s.Code = '1' AND s.SubjectId = @creId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'My Family', '1.2', 'My family', 2, s.Id, @cbcId, @creId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creation' AND s.Code = '1' AND s.SubjectId = @creId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Creation of Plants and Animals', '1.3', 'Creation of plants and animals', 3, s.Id, @cbcId, @creId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creation' AND s.Code = '1' AND s.SubjectId = @creId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Physical Handling of the Holy Bible', '2.1', 'Physical handling of the Holy Bible', 1, s.Id, @cbcId, @creId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Holy Bible' AND s.Code = '2' AND s.SubjectId = @creId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Bible Story David and Goliath', '2.2', 'Bible story of David and Goliath', 2, s.Id, @cbcId, @creId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Holy Bible' AND s.Code = '2' AND s.SubjectId = @creId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Bible Story Joseph and his Coat of Many Colours', '2.3', 'Bible story of Joseph', 3, s.Id, @cbcId, @creId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Holy Bible' AND s.Code = '2' AND s.SubjectId = @creId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'The Birth of Jesus Christ', '3.1', 'The birth of Jesus Christ', 1, s.Id, @cbcId, @creId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Early Life of Jesus Christ' AND s.Code = '3' AND s.SubjectId = @creId AND s.LearningLevelId = @grade1Id;

-- CRE Grade 2
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'God''s Creation', '1.1', 'God''s creation', 1, s.Id, @cbcId, @creId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creation' AND s.Code = '1' AND s.SubjectId = @creId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Books of the Bible', '2.1', 'Books of the Bible', 1, s.Id, @cbcId, @creId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Holy Bible' AND s.Code = '2' AND s.SubjectId = @creId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Bible Stories', '2.2', 'Bible stories', 2, s.Id, @cbcId, @creId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Holy Bible' AND s.Code = '2' AND s.SubjectId = @creId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Teachings of Jesus', '3.1', 'Teachings of Jesus', 1, s.Id, @cbcId, @creId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Life of Jesus Christ' AND s.Code = '3' AND s.SubjectId = @creId AND s.LearningLevelId = @grade2Id;

-- CRE Grade 3
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Stewardship of God''s Creation', '1.1', 'Stewardship of God''s creation', 1, s.Id, @cbcId, @creId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creation and Stewardship' AND s.Code = '1' AND s.SubjectId = @creId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'The Bible and Christian Living', '2.1', 'The Bible and Christian living', 1, s.Id, @cbcId, @creId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Holy Bible' AND s.Code = '2' AND s.SubjectId = @creId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Miracles of Jesus', '3.1', 'Miracles of Jesus', 1, s.Id, @cbcId, @creId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Life and Teachings of Jesus' AND s.Code = '3' AND s.SubjectId = @creId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'The Passion Death and Resurrection of Jesus', '3.2', 'The passion, death and resurrection of Jesus', 2, s.Id, @cbcId, @creId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Life and Teachings of Jesus' AND s.Code = '3' AND s.SubjectId = @creId AND s.LearningLevelId = @grade3Id;

-- CRE Grades 4-9 (single substrand per strand)
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Creation and Human Dignity', '1.1', 'Creation and human dignity', 1, s.Id, @cbcId, @creId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creation and Stewardship' AND s.Code = '1' AND s.SubjectId = @creId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Values for Positive Living', '2.1', 'Values for positive living', 1, s.Id, @cbcId, @creId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Moral and Religious Values' AND s.Code = '2' AND s.SubjectId = @creId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sacred Texts', '1.1', 'Sacred texts', 1, s.Id, @cbcId, @creId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Religious Texts and Teachings' AND s.Code = '1' AND s.SubjectId = @creId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Family and Social Relationships', '2.1', 'Family and social relationships', 1, s.Id, @cbcId, @creId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Moral and Religious Values' AND s.Code = '2' AND s.SubjectId = @creId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Religion and National Development', '1.1', 'Religion and national development', 1, s.Id, @cbcId, @creId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Religion and Society' AND s.Code = '1' AND s.SubjectId = @creId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Contemporary Issues', '2.1', 'Contemporary issues', 1, s.Id, @cbcId, @creId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Moral and Religious Values' AND s.Code = '2' AND s.SubjectId = @creId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'The Bible and Christian Faith', '1.1', 'The Bible and Christian faith', 1, s.Id, @cbcId, @creId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Religious Texts and Teachings' AND s.Code = '1' AND s.SubjectId = @creId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Christian Values in Society', '2.1', 'Christian values in society', 1, s.Id, @cbcId, @creId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Moral and Religious Values' AND s.Code = '2' AND s.SubjectId = @creId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'New Testament Studies', '1.1', 'New Testament studies', 1, s.Id, @cbcId, @creId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Religious Texts and Teachings' AND s.Code = '1' AND s.SubjectId = @creId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Ethics and Social Justice', '2.1', 'Ethics and social justice', 1, s.Id, @cbcId, @creId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Moral and Religious Values' AND s.Code = '2' AND s.SubjectId = @creId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Church History and Theology', '1.1', 'Church history and theology', 1, s.Id, @cbcId, @creId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Religious Texts and Teachings' AND s.Code = '1' AND s.SubjectId = @creId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Christian Response to Contemporary Challenges', '2.1', 'Christian response to contemporary challenges', 1, s.Id, @cbcId, @creId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Moral and Religious Values' AND s.Code = '2' AND s.SubjectId = @creId AND s.LearningLevelId = @grade9Id;

-- ============================================
-- IRE Sub-Strands - Grades 1-9
-- ============================================

-- IRE Grade 1
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Self and Family', '1.1', 'Self and family', 1, s.Id, @cbcId, @ireId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creation' AND s.Code = '1' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Introduction to the Quran', '2.1', 'Introduction to the Quran', 1, s.Id, @cbcId, @ireId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Holy Quran' AND s.Code = '2' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Shahadah and Salah', '3.1', 'Shahadah and Salah', 1, s.Id, @cbcId, @ireId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Pillars of Islam' AND s.Code = '3' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade1Id;

-- IRE Grade 2
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'God''s Creation', '1.1', 'God''s creation', 1, s.Id, @cbcId, @ireId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creation' AND s.Code = '1' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Reading the Quran', '2.1', 'Reading the Quran', 1, s.Id, @cbcId, @ireId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Holy Quran' AND s.Code = '2' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Acts of Worship', '3.1', 'Acts of worship', 1, s.Id, @cbcId, @ireId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Pillars of Islam' AND s.Code = '3' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade2Id;

-- IRE Grade 3
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Stewardship', '1.1', 'Stewardship', 1, s.Id, @cbcId, @ireId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creation' AND s.Code = '1' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Quranic Recitation and Meaning', '2.1', 'Quranic recitation and meaning', 1, s.Id, @cbcId, @ireId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'The Holy Quran' AND s.Code = '2' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Saum and Hajj', '3.1', 'Saum and Hajj', 1, s.Id, @cbcId, @ireId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Pillars of Islam' AND s.Code = '3' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade3Id;

-- IRE Grades 4-9 (single substrand per strand)
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Islamic Beliefs and Creation', '1.1', 'Islamic beliefs and creation', 1, s.Id, @cbcId, @ireId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Islamic Teachings' AND s.Code = '1' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Islamic Values for Positive Living', '2.1', 'Islamic values for positive living', 1, s.Id, @cbcId, @ireId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Moral and Religious Values' AND s.Code = '2' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Quranic Studies', '1.1', 'Quranic studies', 1, s.Id, @cbcId, @ireId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Islamic Texts and Teachings' AND s.Code = '1' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Family and Social Relationships in Islam', '2.1', 'Family and social relationships in Islam', 1, s.Id, @cbcId, @ireId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Moral and Religious Values' AND s.Code = '2' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Islam and National Development', '1.1', 'Islam and national development', 1, s.Id, @cbcId, @ireId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Islam and Society' AND s.Code = '1' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Contemporary Issues in Islam', '2.1', 'Contemporary issues in Islam', 1, s.Id, @cbcId, @ireId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Moral and Religious Values' AND s.Code = '2' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Quran and Hadith Studies', '1.1', 'Quran and Hadith studies', 1, s.Id, @cbcId, @ireId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Islamic Texts and Teachings' AND s.Code = '1' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Islamic Values in Society', '2.1', 'Islamic values in society', 1, s.Id, @cbcId, @ireId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Moral and Religious Values' AND s.Code = '2' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Islamic Jurisprudence', '1.1', 'Islamic jurisprudence', 1, s.Id, @cbcId, @ireId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Islamic Texts and Teachings' AND s.Code = '1' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Ethics and Social Justice in Islam', '2.1', 'Ethics and social justice in Islam', 1, s.Id, @cbcId, @ireId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Moral and Religious Values' AND s.Code = '2' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Islamic History and Thought', '1.1', 'Islamic history and thought', 1, s.Id, @cbcId, @ireId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Islamic Texts and Teachings' AND s.Code = '1' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Islamic Response to Contemporary Challenges', '2.1', 'Islamic response to contemporary challenges', 1, s.Id, @cbcId, @ireId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Moral and Religious Values' AND s.Code = '2' AND s.SubjectId = @ireId AND s.LearningLevelId = @grade9Id;

-- ============================================
-- MOVEMENT AND CREATIVE ACTIVITIES Sub-Strands - Grades 1-3
-- ============================================

-- Grade 1
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Locomotor Skills', '1.1', 'Locomotor skills', 1, s.Id, @cbcId, @artId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Education' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Non-locomotor Skills', '1.2', 'Non-locomotor skills', 2, s.Id, @cbcId, @artId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Education' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Ball Skills', '1.3', 'Ball skills', 3, s.Id, @cbcId, @artId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Education' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Visual Arts', '2.1', 'Visual arts', 1, s.Id, @cbcId, @artId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creative Arts' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Music', '2.2', 'Music', 2, s.Id, @cbcId, @artId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creative Arts' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade1Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Drama', '2.3', 'Drama', 3, s.Id, @cbcId, @artId, @grade1Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creative Arts' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade1Id;

-- Movement Grade 2
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Locomotor and Non-locomotor Skills', '1.1', 'Locomotor and non-locomotor skills', 1, s.Id, @cbcId, @artId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Education' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Games and Sports', '1.2', 'Games and sports', 2, s.Id, @cbcId, @artId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Education' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Visual Arts', '2.1', 'Visual arts', 1, s.Id, @cbcId, @artId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creative Arts' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Music', '2.2', 'Music', 2, s.Id, @cbcId, @artId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creative Arts' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade2Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Dance', '2.3', 'Dance', 3, s.Id, @cbcId, @artId, @grade2Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creative Arts' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade2Id;

-- Movement Grade 3
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Fitness and Athletics', '1.1', 'Fitness and athletics', 1, s.Id, @cbcId, @artId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Education' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Games and Sports', '1.2', 'Games and sports', 2, s.Id, @cbcId, @artId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical Education' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Visual Arts and Craft', '2.1', 'Visual arts and craft', 1, s.Id, @cbcId, @artId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creative Arts' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Music', '2.2', 'Music', 2, s.Id, @cbcId, @artId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creative Arts' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade3Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Drama and Dance', '2.3', 'Drama and dance', 3, s.Id, @cbcId, @artId, @grade3Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Creative Arts' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade3Id;

-- ============================================
-- CREATIVE ARTS AND SPORTS Sub-Strands - Grades 4-9
-- ============================================

-- Grade 4
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Drawing and Painting', '1.1', 'Drawing and painting', 1, s.Id, @cbcId, @artId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Visual Arts' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mixed Media and Technology', '1.2', 'Mixed media and technology', 2, s.Id, @cbcId, @artId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Visual Arts' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Singing and Rhythm', '2.1', 'Singing and rhythm', 1, s.Id, @cbcId, @artId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Music' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Athletics and Games', '3.1', 'Athletics and games', 1, s.Id, @cbcId, @artId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical and Health Education' AND s.Code = '3' AND s.SubjectId = @artId AND s.LearningLevelId = @grade4Id;

-- Creative Arts Grade 5
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Indigenous Kenyan Crafts', '1.1', 'Indigenous Kenyan crafts', 1, s.Id, @cbcId, @artId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Visual Arts' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Music Theory and Performance', '2.1', 'Music theory and performance', 1, s.Id, @cbcId, @artId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Music' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Games and Sports', '3.1', 'Games and sports', 1, s.Id, @cbcId, @artId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical and Health Education' AND s.Code = '3' AND s.SubjectId = @artId AND s.LearningLevelId = @grade5Id;

-- Creative Arts Grade 6
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Graphic Design and Photography', '1.1', 'Graphic design and photography', 1, s.Id, @cbcId, @artId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Visual Arts' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Composition and Performance', '2.1', 'Composition and performance', 1, s.Id, @cbcId, @artId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Music' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sports Leadership and Health', '3.1', 'Sports leadership and health', 1, s.Id, @cbcId, @artId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical and Health Education' AND s.Code = '3' AND s.SubjectId = @artId AND s.LearningLevelId = @grade6Id;

-- Creative Arts Grades 7-9
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Visual Arts and Design', '1.1', 'Visual arts and design', 1, s.Id, @cbcId, @artId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Visual Arts' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Music Performance and Appreciation', '2.1', 'Music performance and appreciation', 1, s.Id, @cbcId, @artId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Music' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sports and Fitness', '3.1', 'Sports and fitness', 1, s.Id, @cbcId, @artId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical and Health Education' AND s.Code = '3' AND s.SubjectId = @artId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Visual Arts', '1.1', 'Advanced visual arts', 1, s.Id, @cbcId, @artId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Visual Arts' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Music', '2.1', 'Advanced music', 1, s.Id, @cbcId, @artId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Music' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Sports and Fitness', '3.1', 'Advanced sports and fitness', 1, s.Id, @cbcId, @artId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical and Health Education' AND s.Code = '3' AND s.SubjectId = @artId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Professional Visual Arts', '1.1', 'Professional visual arts', 1, s.Id, @cbcId, @artId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Visual Arts' AND s.Code = '1' AND s.SubjectId = @artId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Music Mastery', '2.1', 'Music mastery', 1, s.Id, @cbcId, @artId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Music' AND s.Code = '2' AND s.SubjectId = @artId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sports Mastery and Leadership', '3.1', 'Sports mastery and leadership', 1, s.Id, @cbcId, @artId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Physical and Health Education' AND s.Code = '3' AND s.SubjectId = @artId AND s.LearningLevelId = @grade9Id;

-- ============================================
-- SCIENCE AND TECHNOLOGY Sub-Strands - Grades 4-6
-- ============================================

-- Grade 4
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Cells', '1.1', 'Cells', 1, s.Id, @cbcId, @sciId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Living Things and Their Environment' AND s.Code = '1' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Classification of Living Things', '1.2', 'Classification of living things', 2, s.Id, @cbcId, @sciId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Living Things and Their Environment' AND s.Code = '1' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'States of Matter', '2.1', 'States of matter', 1, s.Id, @cbcId, @sciId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Matter' AND s.Code = '2' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Management of Solid Waste', '2.2', 'Management of solid waste', 2, s.Id, @cbcId, @sciId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Matter' AND s.Code = '2' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Water Conservation', '2.3', 'Water conservation', 3, s.Id, @cbcId, @sciId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Matter' AND s.Code = '2' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Forms of Energy', '3.1', 'Forms of energy', 1, s.Id, @cbcId, @sciId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Energy' AND s.Code = '3' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Electricity', '3.2', 'Electricity', 2, s.Id, @cbcId, @sciId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Energy' AND s.Code = '3' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'ICT and Digital Literacy', '4.1', 'ICT and digital literacy', 1, s.Id, @cbcId, @sciId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Technology' AND s.Code = '4' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade4Id;

-- Science Grade 5
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Nutrition in Plants and Animals', '1.1', 'Nutrition in plants and animals', 1, s.Id, @cbcId, @sciId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Living Things and Their Environment' AND s.Code = '1' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Reproduction', '1.2', 'Reproduction', 2, s.Id, @cbcId, @sciId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Living Things and Their Environment' AND s.Code = '1' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mixtures', '2.1', 'Mixtures', 1, s.Id, @cbcId, @sciId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Matter' AND s.Code = '2' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Acids and Bases', '2.2', 'Acids and bases', 2, s.Id, @cbcId, @sciId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Matter' AND s.Code = '2' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Heat Energy', '3.1', 'Heat energy', 1, s.Id, @cbcId, @sciId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Energy' AND s.Code = '3' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Magnetism', '3.2', 'Magnetism', 2, s.Id, @cbcId, @sciId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Energy' AND s.Code = '3' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Simple Machines', '4.1', 'Simple machines', 1, s.Id, @cbcId, @sciId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Technology' AND s.Code = '4' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade5Id;

-- Science Grade 6
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Ecosystems', '1.1', 'Ecosystems', 1, s.Id, @cbcId, @sciId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Living Things and Their Environment' AND s.Code = '1' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Health and Disease', '1.2', 'Health and disease', 2, s.Id, @cbcId, @sciId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Living Things and Their Environment' AND s.Code = '1' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Chemical Reactions', '2.1', 'Chemical reactions', 1, s.Id, @cbcId, @sciId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Matter' AND s.Code = '2' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sound and Light', '3.1', 'Sound and light', 1, s.Id, @cbcId, @sciId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Energy' AND s.Code = '3' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Renewable and Non-renewable Energy', '3.2', 'Renewable and non-renewable energy', 2, s.Id, @cbcId, @sciId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Energy' AND s.Code = '3' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Innovation and Invention', '4.1', 'Innovation and invention', 1, s.Id, @cbcId, @sciId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Technology' AND s.Code = '4' AND s.SubjectId = @sciId AND s.LearningLevelId = @grade6Id;

-- ============================================
-- INTEGRATED SCIENCE Sub-Strands - Grades 7-9
-- ============================================

-- Grade 7
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Scientific Method', '1.1', 'Scientific method', 1, s.Id, @cbcId, @intSciId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Scientific Investigation' AND s.Code = '1' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Cell Biology', '2.1', 'Cell biology', 1, s.Id, @cbcId, @intSciId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Living Things and Their Environment' AND s.Code = '2' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Nutrition and Digestion', '2.2', 'Nutrition and digestion', 2, s.Id, @cbcId, @intSciId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Living Things and Their Environment' AND s.Code = '2' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Atomic Structure', '3.1', 'Atomic structure', 1, s.Id, @cbcId, @intSciId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Matter and Its Properties' AND s.Code = '3' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Chemical and Physical Changes', '3.2', 'Chemical and physical changes', 2, s.Id, @cbcId, @intSciId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Matter and Its Properties' AND s.Code = '3' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Heat and Temperature', '4.1', 'Heat and temperature', 1, s.Id, @cbcId, @intSciId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Energy' AND s.Code = '4' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Electricity and Magnetism', '4.2', 'Electricity and magnetism', 2, s.Id, @cbcId, @intSciId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Energy' AND s.Code = '4' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade7Id;

-- Integrated Science Grade 8
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Reproduction and Genetics', '1.1', 'Reproduction and genetics', 1, s.Id, @cbcId, @intSciId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Living Things and Their Environment' AND s.Code = '1' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'The Periodic Table', '2.1', 'The periodic table', 1, s.Id, @cbcId, @intSciId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Matter and Its Properties' AND s.Code = '2' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Chemical Reactions', '2.2', 'Chemical reactions', 2, s.Id, @cbcId, @intSciId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Matter and Its Properties' AND s.Code = '2' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mechanics', '3.1', 'Mechanics', 1, s.Id, @cbcId, @intSciId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Force and Motion' AND s.Code = '3' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Ecology and Conservation', '4.1', 'Ecology and conservation', 1, s.Id, @cbcId, @intSciId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Environment and Sustainability' AND s.Code = '4' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade8Id;

-- Integrated Science Grade 9
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Evolution and Classification', '1.1', 'Evolution and classification', 1, s.Id, @cbcId, @intSciId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Living Things and Their Environment' AND s.Code = '1' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Acids Bases and Salts', '2.1', 'Acids, bases and salts', 1, s.Id, @cbcId, @intSciId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Matter and Its Properties' AND s.Code = '2' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Waves and Optics', '3.1', 'Waves and optics', 1, s.Id, @cbcId, @intSciId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Energy' AND s.Code = '3' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Nuclear Energy and Radioactivity', '3.2', 'Nuclear energy and radioactivity', 2, s.Id, @cbcId, @intSciId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Energy' AND s.Code = '3' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Earth and the Universe', '4.1', 'Earth and the universe', 1, s.Id, @cbcId, @intSciId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Earth and Space Science' AND s.Code = '4' AND s.SubjectId = @intSciId AND s.LearningLevelId = @grade9Id;

-- ============================================
-- SOCIAL STUDIES Sub-Strands - Grades 4-9
-- ============================================

-- Grade 4
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Kenya''s Physical Environment', '1.1', 'Kenya''s physical environment', 1, s.Id, @cbcId, @socId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Our Environment' AND s.Code = '1' AND s.SubjectId = @socId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Climate and Vegetation', '1.2', 'Climate and vegetation', 2, s.Id, @cbcId, @socId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Our Environment' AND s.Code = '1' AND s.SubjectId = @socId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Population and Settlement', '2.1', 'Population and settlement', 1, s.Id, @cbcId, @socId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'People and their Activities' AND s.Code = '2' AND s.SubjectId = @socId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Economic Activities', '2.2', 'Economic activities', 2, s.Id, @cbcId, @socId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'People and their Activities' AND s.Code = '2' AND s.SubjectId = @socId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Government and Citizenship', '3.1', 'Government and citizenship', 1, s.Id, @cbcId, @socId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Governance' AND s.Code = '3' AND s.SubjectId = @socId AND s.LearningLevelId = @grade4Id;

-- Social Studies Grades 5-9 (single substrand per strand)
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'East Africa', '1.1', 'East Africa', 1, s.Id, @cbcId, @socId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Our Environment' AND s.Code = '1' AND s.SubjectId = @socId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Economic Integration in East Africa', '2.1', 'Economic integration in East Africa', 1, s.Id, @cbcId, @socId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'People and their Activities' AND s.Code = '2' AND s.SubjectId = @socId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Historical Development', '3.1', 'Historical development', 1, s.Id, @cbcId, @socId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Governance' AND s.Code = '3' AND s.SubjectId = @socId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Africa', '1.1', 'Africa', 1, s.Id, @cbcId, @socId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Our Environment' AND s.Code = '1' AND s.SubjectId = @socId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Development and Trade', '2.1', 'Development and trade', 1, s.Id, @cbcId, @socId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'People and their Activities' AND s.Code = '2' AND s.SubjectId = @socId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Global Citizenship', '3.1', 'Global citizenship', 1, s.Id, @cbcId, @socId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Governance' AND s.Code = '3' AND s.SubjectId = @socId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Our County and Country', '1.1', 'Our county and country', 1, s.Id, @cbcId, @socId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Our Environment' AND s.Code = '1' AND s.SubjectId = @socId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'People and Economic Activities', '2.1', 'People and economic activities', 1, s.Id, @cbcId, @socId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'People and their Activities' AND s.Code = '2' AND s.SubjectId = @socId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Governance and Citizenship', '3.1', 'Governance and citizenship', 1, s.Id, @cbcId, @socId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Governance' AND s.Code = '3' AND s.SubjectId = @socId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Africa', '1.1', 'Africa', 1, s.Id, @cbcId, @socId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Our Environment' AND s.Code = '1' AND s.SubjectId = @socId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'People and Economic Development', '2.1', 'People and economic development', 1, s.Id, @cbcId, @socId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'People and their Activities' AND s.Code = '2' AND s.SubjectId = @socId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Historical Development', '3.1', 'Historical development', 1, s.Id, @cbcId, @socId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Governance' AND s.Code = '3' AND s.SubjectId = @socId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Global Issues', '1.1', 'Global issues', 1, s.Id, @cbcId, @socId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Our Environment' AND s.Code = '1' AND s.SubjectId = @socId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'People and Global Development', '2.1', 'People and global development', 1, s.Id, @cbcId, @socId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'People and their Activities' AND s.Code = '2' AND s.SubjectId = @socId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Global Governance', '3.1', 'Global governance', 1, s.Id, @cbcId, @socId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Governance' AND s.Code = '3' AND s.SubjectId = @socId AND s.LearningLevelId = @grade9Id;

-- ============================================
-- AGRICULTURE Sub-Strands - Grades 4-9
-- ============================================

-- Grade 4
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Soil Preparation and Planting', '1.1', 'Soil preparation and planting', 1, s.Id, @cbcId, @agrId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Crop Production' AND s.Code = '1' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Crop Management', '1.2', 'Crop management', 2, s.Id, @cbcId, @agrId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Crop Production' AND s.Code = '1' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade4Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Livestock Keeping', '2.1', 'Livestock keeping', 1, s.Id, @cbcId, @agrId, @grade4Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Animal Production' AND s.Code = '2' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade4Id;

-- Agriculture Grades 5-9 (single substrand per strand)
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Crop Husbandry', '1.1', 'Crop husbandry', 1, s.Id, @cbcId, @agrId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Crop Production' AND s.Code = '1' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Poultry and Dairy Farming', '2.1', 'Poultry and dairy farming', 1, s.Id, @cbcId, @agrId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Animal Production' AND s.Code = '2' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Farm Records and Marketing', '3.1', 'Farm records and marketing', 1, s.Id, @cbcId, @agrId, @grade5Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Farm Management' AND s.Code = '3' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade5Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Cash and Food Crops', '1.1', 'Cash and food crops', 1, s.Id, @cbcId, @agrId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Crop Production' AND s.Code = '1' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Apiculture and Fish Farming', '2.1', 'Apiculture and fish farming', 1, s.Id, @cbcId, @agrId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Animal Production' AND s.Code = '2' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade6Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Agribusiness', '3.1', 'Agribusiness', 1, s.Id, @cbcId, @agrId, @grade6Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Farm Management' AND s.Code = '3' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade6Id;

-- Agriculture Grades 7-9
INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Crop Production', '1.1', 'Crop production', 1, s.Id, @cbcId, @agrId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Crop Production' AND s.Code = '1' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Animal Production', '2.1', 'Animal production', 1, s.Id, @cbcId, @agrId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Animal Production' AND s.Code = '2' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Farm Management', '3.1', 'Farm management', 1, s.Id, @cbcId, @agrId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Farm Management' AND s.Code = '3' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Crop Production', '1.1', 'Advanced crop production', 1, s.Id, @cbcId, @agrId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Crop Production' AND s.Code = '1' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Animal Production', '2.1', 'Advanced animal production', 1, s.Id, @cbcId, @agrId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Animal Production' AND s.Code = '2' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Farm Management', '3.1', 'Advanced farm management', 1, s.Id, @cbcId, @agrId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Farm Management' AND s.Code = '3' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sustainable Crop Production', '1.1', 'Sustainable crop production', 1, s.Id, @cbcId, @agrId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Crop Production' AND s.Code = '1' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Sustainable Animal Production', '2.1', 'Sustainable animal production', 1, s.Id, @cbcId, @agrId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Animal Production' AND s.Code = '2' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Agribusiness and Food Security', '3.1', 'Agribusiness and food security', 1, s.Id, @cbcId, @agrId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Farm Management' AND s.Code = '3' AND s.SubjectId = @agrId AND s.LearningLevelId = @grade9Id;

-- ============================================
-- PRE-TECHNICAL STUDIES Sub-Strands - Grades 7-9
-- ============================================

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Technical Drawing Fundamentals', '1.1', 'Technical drawing fundamentals', 1, s.Id, @cbcId, @preTechId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Technical Drawing' AND s.Code = '1' AND s.SubjectId = @preTechId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Basic Electronics', '2.1', 'Basic electronics', 1, s.Id, @cbcId, @preTechId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Electronics' AND s.Code = '2' AND s.SubjectId = @preTechId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Materials and Technology', '3.1', 'Materials and technology', 1, s.Id, @cbcId, @preTechId, @grade7Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Materials and Technology' AND s.Code = '3' AND s.SubjectId = @preTechId AND s.LearningLevelId = @grade7Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Intermediate Technical Drawing', '1.1', 'Intermediate technical drawing', 1, s.Id, @cbcId, @preTechId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Technical Drawing' AND s.Code = '1' AND s.SubjectId = @preTechId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Intermediate Electronics', '2.1', 'Intermediate electronics', 1, s.Id, @cbcId, @preTechId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Electronics' AND s.Code = '2' AND s.SubjectId = @preTechId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Mechanisms and Motion', '3.1', 'Mechanisms and motion', 1, s.Id, @cbcId, @preTechId, @grade8Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Mechanisms' AND s.Code = '3' AND s.SubjectId = @preTechId AND s.LearningLevelId = @grade8Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Design and Technology', '1.1', 'Design and technology', 1, s.Id, @cbcId, @preTechId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Design and Technology' AND s.Code = '1' AND s.SubjectId = @preTechId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Advanced Electronics', '2.1', 'Advanced electronics', 1, s.Id, @cbcId, @preTechId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Electronics' AND s.Code = '2' AND s.SubjectId = @preTechId AND s.LearningLevelId = @grade9Id;

INSERT INTO substrands (Name, Code, Description, `Rank`, StrandId, CurriculumId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Materials and Fabrication', '3.1', 'Materials and fabrication', 1, s.Id, @cbcId, @preTechId, @grade9Id, 1, 3, @now, @now
FROM strands s WHERE s.Name = 'Materials and Fabrication' AND s.Code = '3' AND s.SubjectId = @preTechId AND s.LearningLevelId = @grade9Id;

-- ============================================================
-- STEP 5: INSERT SPECIFIC LEARNING OUTCOMES
-- ============================================================
-- Using a stored procedure pattern for efficiency

DELIMITER //
DROP PROCEDURE IF EXISTS InsertOutcome //
CREATE PROCEDURE InsertOutcome(
    IN p_name VARCHAR(500),
    IN p_rank INT,
    IN p_ss_code VARCHAR(10),
    IN p_s_code VARCHAR(10),
    IN p_subjectId INT,
    IN p_gradeId INT
)
BEGIN
    INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
    SELECT p_name, '', p_rank, ss.Id, @defaultBroadOutcomeId, @defaultGeneralOutcomeId, @now, @now
    FROM substrands ss
    JOIN strands s ON ss.StrandId = s.Id
    WHERE ss.Code = p_ss_code AND s.Code = p_s_code AND s.SubjectId = p_subjectId AND s.LearningLevelId = p_gradeId
    LIMIT 1;
END //
DELIMITER ;

-- ============================================
-- MATHEMATICS Grade 1 - Specific Outcomes
-- ============================================

-- SS 1.1 Number Concept
CALL InsertOutcome('Sort and group objects according to different attributes', 1, '1.1', '1', @mathId, @grade1Id);
CALL InsertOutcome('Pair and match objects in the environment', 2, '1.1', '1', @mathId, @grade1Id);
CALL InsertOutcome('Order and sequence objects in ascending and descending order', 3, '1.1', '1', @mathId, @grade1Id);
CALL InsertOutcome('Make patterns using real objects', 4, '1.1', '1', @mathId, @grade1Id);
CALL InsertOutcome('Recite number names in order up to 50', 5, '1.1', '1', @mathId, @grade1Id);
CALL InsertOutcome('Represent numbers 1-30 using concrete objects', 6, '1.1', '1', @mathId, @grade1Id);
CALL InsertOutcome('Demonstrate through counting that a group has only one count', 7, '1.1', '1', @mathId, @grade1Id);
CALL InsertOutcome('Appreciate the use of sorting and grouping items in day to day activities', 8, '1.1', '1', @mathId, @grade1Id);

-- SS 1.2 Whole Numbers
CALL InsertOutcome('Count numbers forward and backward up to 100', 1, '1.2', '1', @mathId, @grade1Id);
CALL InsertOutcome('Represent numbers 1-50 using concrete objects', 2, '1.2', '1', @mathId, @grade1Id);
CALL InsertOutcome('Identify place value of ones and tens', 3, '1.2', '1', @mathId, @grade1Id);
CALL InsertOutcome('Read and write numbers 1-50 in symbols', 4, '1.2', '1', @mathId, @grade1Id);
CALL InsertOutcome('Write numbers 1-10 in words', 5, '1.2', '1', @mathId, @grade1Id);
CALL InsertOutcome('Identify missing numbers in number patterns up to 20', 6, '1.2', '1', @mathId, @grade1Id);
CALL InsertOutcome('Appreciate number patterns by creating and extending patterns', 7, '1.2', '1', @mathId, @grade1Id);

-- SS 1.3 Addition
CALL InsertOutcome('Model addition as putting objects together', 1, '1.3', '1', @mathId, @grade1Id);
CALL InsertOutcome('Use + and = signs in writing addition sentences', 2, '1.3', '1', @mathId, @grade1Id);
CALL InsertOutcome('Add 2 single digit numbers up to a sum of 10', 3, '1.3', '1', @mathId, @grade1Id);
CALL InsertOutcome('Add 3 single digit numbers up to a sum of 10', 4, '1.3', '1', @mathId, @grade1Id);
CALL InsertOutcome('Add a 2-digit number to a 1-digit number without regrouping with sum not exceeding 100', 5, '1.3', '1', @mathId, @grade1Id);
CALL InsertOutcome('Add multiples of 10 up to 100 vertically', 6, '1.3', '1', @mathId, @grade1Id);
CALL InsertOutcome('Work out missing numbers in addition patterns up to 100', 7, '1.3', '1', @mathId, @grade1Id);

-- SS 1.4 Subtraction
CALL InsertOutcome('Model subtraction as taking away using concrete objects', 1, '1.4', '1', @mathId, @grade1Id);
CALL InsertOutcome('Use - and = signs in subtraction sentences', 2, '1.4', '1', @mathId, @grade1Id);
CALL InsertOutcome('Subtract single digit numbers', 3, '1.4', '1', @mathId, @grade1Id);
CALL InsertOutcome('Subtract a 1-digit number from a 2-digit number based on addition facts', 4, '1.4', '1', @mathId, @grade1Id);
CALL InsertOutcome('Use the relationship between addition and subtraction', 5, '1.4', '1', @mathId, @grade1Id);
CALL InsertOutcome('Subtract multiples of 10 up to 90', 6, '1.4', '1', @mathId, @grade1Id);
CALL InsertOutcome('Work out missing numbers in subtraction patterns up to 100', 7, '1.4', '1', @mathId, @grade1Id);

-- SS 2.1 Length
CALL InsertOutcome('Compare length of objects directly', 1, '2.1', '2', @mathId, @grade1Id);
CALL InsertOutcome('Conserve length through manipulation', 2, '2.1', '2', @mathId, @grade1Id);
CALL InsertOutcome('Measure length using arbitrary units', 3, '2.1', '2', @mathId, @grade1Id);

-- SS 2.2 Mass
CALL InsertOutcome('Compare mass of objects directly', 1, '2.2', '2', @mathId, @grade1Id);
CALL InsertOutcome('Conserve mass through manipulation', 2, '2.2', '2', @mathId, @grade1Id);
CALL InsertOutcome('Measure mass using arbitrary units', 3, '2.2', '2', @mathId, @grade1Id);

-- SS 2.3 Capacity
CALL InsertOutcome('Compare capacity of containers directly', 1, '2.3', '2', @mathId, @grade1Id);
CALL InsertOutcome('Conserve capacity through manipulation', 2, '2.3', '2', @mathId, @grade1Id);
CALL InsertOutcome('Measure capacity using arbitrary units', 3, '2.3', '2', @mathId, @grade1Id);

-- SS 2.4 Time
CALL InsertOutcome('Relate daily activities to time', 1, '2.4', '2', @mathId, @grade1Id);
CALL InsertOutcome('Relate days of the week with various activities', 2, '2.4', '2', @mathId, @grade1Id);

-- SS 2.5 Money
CALL InsertOutcome('Identify Kenyan currency coins and notes up to Ksh 100', 1, '2.5', '2', @mathId, @grade1Id);
CALL InsertOutcome('Relate money to goods and services up to Ksh 100', 2, '2.5', '2', @mathId, @grade1Id);
CALL InsertOutcome('Differentiate between needs and wants', 3, '2.5', '2', @mathId, @grade1Id);
CALL InsertOutcome('Appreciate spending and saving in real life situations', 4, '2.5', '2', @mathId, @grade1Id);

-- SS 3.1 Lines
CALL InsertOutcome('Draw straight lines for application in real life', 1, '3.1', '3', @mathId, @grade1Id);
CALL InsertOutcome('Draw curved lines for application in real life', 2, '3.1', '3', @mathId, @grade1Id);

-- SS 3.2 Shapes
CALL InsertOutcome('Identify rectangles circles and triangles in the environment', 1, '3.2', '3', @mathId, @grade1Id);
CALL InsertOutcome('Make patterns involving rectangles circles and triangles', 2, '3.2', '3', @mathId, @grade1Id);
CALL InsertOutcome('Appreciate the beauty of patterns in the environment', 3, '3.2', '3', @mathId, @grade1Id);

-- ============================================
-- MATHEMATICS Grade 2 - Specific Outcomes
-- ============================================

-- SS 1.1 Number Concept
CALL InsertOutcome('Read numbers 1-100 in symbols', 1, '1.1', '1', @mathId, @grade2Id);
CALL InsertOutcome('Represent numbers 1-100 using concrete objects from the environment', 2, '1.1', '1', @mathId, @grade2Id);
CALL InsertOutcome('Play number games using number cards or digital devices', 3, '1.1', '1', @mathId, @grade2Id);
CALL InsertOutcome('Appreciate the use of numbers in real life situations', 4, '1.1', '1', @mathId, @grade2Id);

-- SS 1.2 Whole Numbers
CALL InsertOutcome('Count numbers forward and backward up to 100', 1, '1.2', '1', @mathId, @grade2Id);
CALL InsertOutcome('Identify place value up to hundreds', 2, '1.2', '1', @mathId, @grade2Id);
CALL InsertOutcome('Read numbers 1-100 in symbols', 3, '1.2', '1', @mathId, @grade2Id);
CALL InsertOutcome('Read and write numbers 1-20 in words', 4, '1.2', '1', @mathId, @grade2Id);
CALL InsertOutcome('Work out missing numbers in number patterns up to 100', 5, '1.2', '1', @mathId, @grade2Id);
CALL InsertOutcome('Appreciate number patterns as they skip on the number line', 6, '1.2', '1', @mathId, @grade2Id);

-- SS 1.3 Addition
CALL InsertOutcome('Add a 2-digit number and a 1-digit number with regrouping', 1, '1.3', '1', @mathId, @grade2Id);
CALL InsertOutcome('Add two 2-digit numbers horizontally and vertically', 2, '1.3', '1', @mathId, @grade2Id);
CALL InsertOutcome('Add three 2-digit numbers', 3, '1.3', '1', @mathId, @grade2Id);
CALL InsertOutcome('Work out missing numbers in addition patterns up to 100', 4, '1.3', '1', @mathId, @grade2Id);

-- SS 1.4 Subtraction
CALL InsertOutcome('Subtract a 1-digit number from a 2-digit number with regrouping', 1, '1.4', '1', @mathId, @grade2Id);
CALL InsertOutcome('Subtract a 2-digit number from a 2-digit number', 2, '1.4', '1', @mathId, @grade2Id);
CALL InsertOutcome('Work out missing numbers in subtraction patterns up to 100', 3, '1.4', '1', @mathId, @grade2Id);

-- SS 1.5 Multiplication
CALL InsertOutcome('Identify multiplication as repeated addition', 1, '1.5', '1', @mathId, @grade2Id);
CALL InsertOutcome('Use x and = signs in multiplication sentences', 2, '1.5', '1', @mathId, @grade2Id);
CALL InsertOutcome('Develop multiplication tables of 2 3 4 and 5', 3, '1.5', '1', @mathId, @grade2Id);
CALL InsertOutcome('Multiply single digit numbers using tables of 2 3 4 and 5', 4, '1.5', '1', @mathId, @grade2Id);
CALL InsertOutcome('Appreciate multiplication in real life situations', 5, '1.5', '1', @mathId, @grade2Id);

-- SS 1.6 Division
CALL InsertOutcome('Identify division as sharing equally', 1, '1.6', '1', @mathId, @grade2Id);
CALL InsertOutcome('Use / and = signs in division sentences', 2, '1.6', '1', @mathId, @grade2Id);
CALL InsertOutcome('Divide objects into equal groups using tables of 2 3 4 and 5', 3, '1.6', '1', @mathId, @grade2Id);
CALL InsertOutcome('Appreciate division in real life situations', 4, '1.6', '1', @mathId, @grade2Id);

-- SS 2.1 Length
CALL InsertOutcome('Measure length using non-standard units', 1, '2.1', '2', @mathId, @grade2Id);
CALL InsertOutcome('Estimate length of objects using non-standard units', 2, '2.1', '2', @mathId, @grade2Id);
CALL InsertOutcome('Appreciate the use of length in real life', 3, '2.1', '2', @mathId, @grade2Id);

-- SS 2.2 Mass
CALL InsertOutcome('Measure mass using non-standard units', 1, '2.2', '2', @mathId, @grade2Id);
CALL InsertOutcome('Estimate mass of objects', 2, '2.2', '2', @mathId, @grade2Id);
CALL InsertOutcome('Appreciate the use of mass in real life', 3, '2.2', '2', @mathId, @grade2Id);

-- SS 2.3 Capacity
CALL InsertOutcome('Measure capacity using non-standard units', 1, '2.3', '2', @mathId, @grade2Id);
CALL InsertOutcome('Estimate capacity of containers', 2, '2.3', '2', @mathId, @grade2Id);
CALL InsertOutcome('Appreciate the use of capacity in real life', 3, '2.3', '2', @mathId, @grade2Id);

-- SS 2.4 Time
CALL InsertOutcome('Read time on a clock to the hour and half hour', 1, '2.4', '2', @mathId, @grade2Id);
CALL InsertOutcome('Relate months of the year to activities', 2, '2.4', '2', @mathId, @grade2Id);
CALL InsertOutcome('Identify events using a calendar', 3, '2.4', '2', @mathId, @grade2Id);

-- SS 2.5 Money
CALL InsertOutcome('Identify Kenyan currency up to Ksh 500', 1, '2.5', '2', @mathId, @grade2Id);
CALL InsertOutcome('Add and subtract money amounts up to Ksh 100', 2, '2.5', '2', @mathId, @grade2Id);
CALL InsertOutcome('Relate money to goods and services in shopping', 3, '2.5', '2', @mathId, @grade2Id);
CALL InsertOutcome('Appreciate saving and spending money', 4, '2.5', '2', @mathId, @grade2Id);

-- SS 3.1 Lines
CALL InsertOutcome('Draw horizontal vertical and slanted lines', 1, '3.1', '3', @mathId, @grade2Id);
CALL InsertOutcome('Identify horizontal vertical and slanted lines in the environment', 2, '3.1', '3', @mathId, @grade2Id);

-- SS 3.2 Shapes
CALL InsertOutcome('Identify squares rectangles triangles and circles in the environment', 1, '3.2', '3', @mathId, @grade2Id);
CALL InsertOutcome('Describe properties of squares rectangles triangles and circles', 2, '3.2', '3', @mathId, @grade2Id);
CALL InsertOutcome('Make patterns using squares rectangles triangles and circles', 3, '3.2', '3', @mathId, @grade2Id);

-- ============================================
-- MATHEMATICS Grade 3 - Specific Outcomes
-- ============================================

CALL InsertOutcome('Read numbers up to 999 in symbols', 1, '1.1', '1', @mathId, @grade3Id);
CALL InsertOutcome('Represent numbers up to 999 using concrete objects', 2, '1.1', '1', @mathId, @grade3Id);
CALL InsertOutcome('Order numbers up to 999 in ascending and descending order', 3, '1.1', '1', @mathId, @grade3Id);
CALL InsertOutcome('Appreciate the use of numbers in real life', 4, '1.1', '1', @mathId, @grade3Id);

CALL InsertOutcome('Count numbers forward and backward up to 1000', 1, '1.2', '1', @mathId, @grade3Id);
CALL InsertOutcome('Identify place value of ones tens and hundreds', 2, '1.2', '1', @mathId, @grade3Id);
CALL InsertOutcome('Read and write numbers up to 1000 in symbols', 3, '1.2', '1', @mathId, @grade3Id);
CALL InsertOutcome('Read and write numbers up to 100 in words', 4, '1.2', '1', @mathId, @grade3Id);
CALL InsertOutcome('Identify odd and even numbers up to 30', 5, '1.2', '1', @mathId, @grade3Id);
CALL InsertOutcome('Work out missing numbers in patterns up to 1000', 6, '1.2', '1', @mathId, @grade3Id);

CALL InsertOutcome('Add 3-digit numbers without regrouping', 1, '1.3', '1', @mathId, @grade3Id);
CALL InsertOutcome('Add 3-digit numbers with regrouping', 2, '1.3', '1', @mathId, @grade3Id);
CALL InsertOutcome('Add three 2-digit numbers with regrouping', 3, '1.3', '1', @mathId, @grade3Id);

CALL InsertOutcome('Subtract 3-digit numbers without regrouping', 1, '1.4', '1', @mathId, @grade3Id);
CALL InsertOutcome('Subtract 3-digit numbers with regrouping', 2, '1.4', '1', @mathId, @grade3Id);
CALL InsertOutcome('Work out missing numbers in subtraction patterns', 3, '1.4', '1', @mathId, @grade3Id);

CALL InsertOutcome('Develop multiplication tables of 6 7 8 9 and 10', 1, '1.5', '1', @mathId, @grade3Id);
CALL InsertOutcome('Multiply numbers using tables of 6 7 8 9 and 10', 2, '1.5', '1', @mathId, @grade3Id);
CALL InsertOutcome('Multiply a 2-digit number by a 1-digit number without regrouping', 3, '1.5', '1', @mathId, @grade3Id);

CALL InsertOutcome('Divide using tables of 6 7 8 9 and 10', 1, '1.6', '1', @mathId, @grade3Id);
CALL InsertOutcome('Divide a 2-digit number by a 1-digit number without remainder', 2, '1.6', '1', @mathId, @grade3Id);
CALL InsertOutcome('Appreciate division in real life', 3, '1.6', '1', @mathId, @grade3Id);

CALL InsertOutcome('Identify halves quarters thirds and eighths of a whole', 1, '1.7', '1', @mathId, @grade3Id);
CALL InsertOutcome('Represent fractions using diagrams and concrete objects', 2, '1.7', '1', @mathId, @grade3Id);
CALL InsertOutcome('Compare simple fractions', 3, '1.7', '1', @mathId, @grade3Id);
CALL InsertOutcome('Appreciate the use of fractions in everyday life', 4, '1.7', '1', @mathId, @grade3Id);

CALL InsertOutcome('Measure length using the metre as a standard unit', 1, '2.1', '2', @mathId, @grade3Id);
CALL InsertOutcome('Convert between metres and centimetres', 2, '2.1', '2', @mathId, @grade3Id);
CALL InsertOutcome('Estimate length in metres and centimetres', 3, '2.1', '2', @mathId, @grade3Id);

CALL InsertOutcome('Measure mass using the kilogram as a standard unit', 1, '2.2', '2', @mathId, @grade3Id);
CALL InsertOutcome('Estimate mass in kilograms', 2, '2.2', '2', @mathId, @grade3Id);
CALL InsertOutcome('Appreciate use of standard units of mass', 3, '2.2', '2', @mathId, @grade3Id);

CALL InsertOutcome('Measure capacity using the litre as a standard unit', 1, '2.3', '2', @mathId, @grade3Id);
CALL InsertOutcome('Estimate capacity in litres', 2, '2.3', '2', @mathId, @grade3Id);
CALL InsertOutcome('Appreciate use of standard units of capacity', 3, '2.3', '2', @mathId, @grade3Id);

CALL InsertOutcome('Read time to the nearest 5 minutes on a clock face', 1, '2.4', '2', @mathId, @grade3Id);
CALL InsertOutcome('Convert between hours and minutes', 2, '2.4', '2', @mathId, @grade3Id);
CALL InsertOutcome('Work out duration of activities', 3, '2.4', '2', @mathId, @grade3Id);
CALL InsertOutcome('Read and interpret a simple calendar', 4, '2.4', '2', @mathId, @grade3Id);

CALL InsertOutcome('Add and subtract money amounts up to Ksh 1000', 1, '2.5', '2', @mathId, @grade3Id);
CALL InsertOutcome('Solve word problems involving money', 2, '2.5', '2', @mathId, @grade3Id);
CALL InsertOutcome('Appreciate the importance of saving and wise spending', 3, '2.5', '2', @mathId, @grade3Id);

CALL InsertOutcome('Identify and draw parallel and perpendicular lines', 1, '3.1', '3', @mathId, @grade3Id);
CALL InsertOutcome('Identify parallel and perpendicular lines in the environment', 2, '3.1', '3', @mathId, @grade3Id);

CALL InsertOutcome('Identify 3D shapes cubes cuboids cylinders cones and spheres', 1, '3.2', '3', @mathId, @grade3Id);
CALL InsertOutcome('Describe properties of 3D shapes', 2, '3.2', '3', @mathId, @grade3Id);
CALL InsertOutcome('Make patterns using 3D shapes', 3, '3.2', '3', @mathId, @grade3Id);
CALL InsertOutcome('Identify 3D shapes in the environment', 4, '3.2', '3', @mathId, @grade3Id);

-- ============================================
-- MATHEMATICS Grade 4 - Specific Outcomes
-- ============================================

CALL InsertOutcome('Use place value and total value of digits up to millions', 1, '1.1', '1', @mathId, @grade4Id);
CALL InsertOutcome('Read and write numbers up to millions in symbols', 2, '1.1', '1', @mathId, @grade4Id);
CALL InsertOutcome('Read and write numbers up to hundred thousands in words', 3, '1.1', '1', @mathId, @grade4Id);
CALL InsertOutcome('Order numbers up to 100000', 4, '1.1', '1', @mathId, @grade4Id);
CALL InsertOutcome('Round off numbers up to 100000 to the nearest thousand', 5, '1.1', '1', @mathId, @grade4Id);
CALL InsertOutcome('Apply squares of whole numbers up to 100', 6, '1.1', '1', @mathId, @grade4Id);
CALL InsertOutcome('Apply square roots of perfect squares up to 10000', 7, '1.1', '1', @mathId, @grade4Id);
CALL InsertOutcome('Appreciate use of whole numbers in real life', 8, '1.1', '1', @mathId, @grade4Id);

CALL InsertOutcome('Identify proper improper and mixed fractions', 1, '1.2', '1', @mathId, @grade4Id);
CALL InsertOutcome('Compare and order fractions', 2, '1.2', '1', @mathId, @grade4Id);
CALL InsertOutcome('Add and subtract fractions', 3, '1.2', '1', @mathId, @grade4Id);
CALL InsertOutcome('Multiply fractions', 4, '1.2', '1', @mathId, @grade4Id);
CALL InsertOutcome('Divide fractions', 5, '1.2', '1', @mathId, @grade4Id);
CALL InsertOutcome('Appreciate the use of fractions in real life', 6, '1.2', '1', @mathId, @grade4Id);

CALL InsertOutcome('Read and write decimals up to thousandths', 1, '1.3', '1', @mathId, @grade4Id);
CALL InsertOutcome('Compare and order decimals', 2, '1.3', '1', @mathId, @grade4Id);
CALL InsertOutcome('Add and subtract decimals', 3, '1.3', '1', @mathId, @grade4Id);
CALL InsertOutcome('Multiply and divide decimals', 4, '1.3', '1', @mathId, @grade4Id);
CALL InsertOutcome('Convert fractions to decimals and vice versa', 5, '1.3', '1', @mathId, @grade4Id);

CALL InsertOutcome('Express fractions and decimals as percentages', 1, '1.4', '1', @mathId, @grade4Id);
CALL InsertOutcome('Find the percentage of a quantity', 2, '1.4', '1', @mathId, @grade4Id);
CALL InsertOutcome('Solve problems involving percentage increase and decrease', 3, '1.4', '1', @mathId, @grade4Id);

CALL InsertOutcome('Convert between millimetres centimetres metres and kilometres', 1, '2.1', '2', @mathId, @grade4Id);
CALL InsertOutcome('Calculate perimeter of squares rectangles and triangles', 2, '2.1', '2', @mathId, @grade4Id);
CALL InsertOutcome('Solve problems involving length', 3, '2.1', '2', @mathId, @grade4Id);

CALL InsertOutcome('Calculate area of squares and rectangles', 1, '2.2', '2', @mathId, @grade4Id);
CALL InsertOutcome('Estimate area of irregular shapes', 2, '2.2', '2', @mathId, @grade4Id);
CALL InsertOutcome('Solve problems involving area', 3, '2.2', '2', @mathId, @grade4Id);

CALL InsertOutcome('Convert between grams and kilograms', 1, '2.3', '2', @mathId, @grade4Id);
CALL InsertOutcome('Solve problems involving mass', 2, '2.3', '2', @mathId, @grade4Id);
CALL InsertOutcome('Appreciate the use of mass in real life', 3, '2.3', '2', @mathId, @grade4Id);

CALL InsertOutcome('Convert between millilitres and litres', 1, '2.4', '2', @mathId, @grade4Id);
CALL InsertOutcome('Solve problems involving capacity', 2, '2.4', '2', @mathId, @grade4Id);
CALL InsertOutcome('Appreciate the use of capacity in real life', 3, '2.4', '2', @mathId, @grade4Id);

CALL InsertOutcome('Read and interpret time in 12-hour and 24-hour format', 1, '2.5', '2', @mathId, @grade4Id);
CALL InsertOutcome('Calculate duration of activities', 2, '2.5', '2', @mathId, @grade4Id);
CALL InsertOutcome('Convert between units of time', 3, '2.5', '2', @mathId, @grade4Id);
CALL InsertOutcome('Use timetables', 4, '2.5', '2', @mathId, @grade4Id);

CALL InsertOutcome('Add and subtract money amounts', 1, '2.6', '2', @mathId, @grade4Id);
CALL InsertOutcome('Calculate profit and loss', 2, '2.6', '2', @mathId, @grade4Id);
CALL InsertOutcome('Solve word problems involving money', 3, '2.6', '2', @mathId, @grade4Id);

CALL InsertOutcome('Identify types of angles acute right obtuse and reflex', 1, '3.1', '3', @mathId, @grade4Id);
CALL InsertOutcome('Measure angles using a protractor', 2, '3.1', '3', @mathId, @grade4Id);
CALL InsertOutcome('Identify parallel and perpendicular lines', 3, '3.1', '3', @mathId, @grade4Id);
CALL InsertOutcome('Draw lines and angles', 4, '3.1', '3', @mathId, @grade4Id);

CALL InsertOutcome('Identify and describe properties of triangles quadrilaterals and circles', 1, '3.2', '3', @mathId, @grade4Id);
CALL InsertOutcome('Calculate perimeter and area of plane shapes', 2, '3.2', '3', @mathId, @grade4Id);
CALL InsertOutcome('Construct simple plane shapes', 3, '3.2', '3', @mathId, @grade4Id);

CALL InsertOutcome('Identify lines of symmetry in plane shapes', 1, '3.3', '3', @mathId, @grade4Id);
CALL InsertOutcome('Draw lines of symmetry', 2, '3.3', '3', @mathId, @grade4Id);
CALL InsertOutcome('Create symmetrical patterns', 3, '3.3', '3', @mathId, @grade4Id);

CALL InsertOutcome('Collect data using tallies', 1, '4.1', '4', @mathId, @grade4Id);
CALL InsertOutcome('Draw and interpret pictograms', 2, '4.1', '4', @mathId, @grade4Id);
CALL InsertOutcome('Draw and interpret bar graphs', 3, '4.1', '4', @mathId, @grade4Id);
CALL InsertOutcome('Solve problems using data', 4, '4.1', '4', @mathId, @grade4Id);

-- ============================================
-- MATHEMATICS Grade 5 - Specific Outcomes
-- ============================================

CALL InsertOutcome('Use place value and total value of digits up to tens of millions', 1, '1.1', '1', @mathId, @grade5Id);
CALL InsertOutcome('Read and write numbers up to tens of millions', 2, '1.1', '1', @mathId, @grade5Id);
CALL InsertOutcome('Order and compare numbers up to millions', 3, '1.1', '1', @mathId, @grade5Id);
CALL InsertOutcome('Round off numbers', 4, '1.1', '1', @mathId, @grade5Id);
CALL InsertOutcome('Find LCM and GCD of numbers', 5, '1.1', '1', @mathId, @grade5Id);
CALL InsertOutcome('Identify prime numbers and composite numbers', 6, '1.1', '1', @mathId, @grade5Id);

CALL InsertOutcome('Add and subtract fractions including mixed numbers', 1, '1.2', '1', @mathId, @grade5Id);
CALL InsertOutcome('Multiply fractions and mixed numbers', 2, '1.2', '1', @mathId, @grade5Id);
CALL InsertOutcome('Divide fractions and mixed numbers', 3, '1.2', '1', @mathId, @grade5Id);
CALL InsertOutcome('Solve problems involving fractions', 4, '1.2', '1', @mathId, @grade5Id);

CALL InsertOutcome('Add subtract multiply and divide decimals', 1, '1.3', '1', @mathId, @grade5Id);
CALL InsertOutcome('Solve problems involving decimals in real life', 2, '1.3', '1', @mathId, @grade5Id);

CALL InsertOutcome('Express quantities as percentages', 1, '1.4', '1', @mathId, @grade5Id);
CALL InsertOutcome('Calculate percentage increase and decrease', 2, '1.4', '1', @mathId, @grade5Id);
CALL InsertOutcome('Solve word problems involving percentages', 3, '1.4', '1', @mathId, @grade5Id);

CALL InsertOutcome('Express ratios in simplest form', 1, '1.5', '1', @mathId, @grade5Id);
CALL InsertOutcome('Divide quantities in given ratios', 2, '1.5', '1', @mathId, @grade5Id);
CALL InsertOutcome('Solve direct proportion problems', 3, '1.5', '1', @mathId, @grade5Id);
CALL InsertOutcome('Apply ratio and proportion in real life', 4, '1.5', '1', @mathId, @grade5Id);

CALL InsertOutcome('Calculate perimeter of various plane shapes', 1, '2.1', '2', @mathId, @grade5Id);
CALL InsertOutcome('Solve problems involving length', 2, '2.1', '2', @mathId, @grade5Id);

CALL InsertOutcome('Calculate area of triangles parallelograms and trapezoids', 1, '2.2', '2', @mathId, @grade5Id);
CALL InsertOutcome('Solve problems involving area of composite shapes', 2, '2.2', '2', @mathId, @grade5Id);

CALL InsertOutcome('Calculate volume of cubes and cuboids', 1, '2.3', '2', @mathId, @grade5Id);
CALL InsertOutcome('Solve problems involving volume in real life', 2, '2.3', '2', @mathId, @grade5Id);

CALL InsertOutcome('Calculate speed distance and time', 1, '2.4', '2', @mathId, @grade5Id);
CALL InsertOutcome('Convert units of speed', 2, '2.4', '2', @mathId, @grade5Id);
CALL InsertOutcome('Solve problems involving time speed and distance', 3, '2.4', '2', @mathId, @grade5Id);

CALL InsertOutcome('Calculate simple interest', 1, '2.5', '2', @mathId, @grade5Id);
CALL InsertOutcome('Solve problems involving tax and discount', 2, '2.5', '2', @mathId, @grade5Id);
CALL InsertOutcome('Appreciate financial literacy in real life', 3, '2.5', '2', @mathId, @grade5Id);

CALL InsertOutcome('Identify angles in triangles and their properties', 1, '3.1', '3', @mathId, @grade5Id);
CALL InsertOutcome('Calculate missing angles in triangles and quadrilaterals', 2, '3.1', '3', @mathId, @grade5Id);
CALL InsertOutcome('Construct angles using a protractor', 3, '3.1', '3', @mathId, @grade5Id);

CALL InsertOutcome('Construct triangles and quadrilaterals', 1, '3.2', '3', @mathId, @grade5Id);
CALL InsertOutcome('Identify properties of circles radius diameter circumference', 2, '3.2', '3', @mathId, @grade5Id);
CALL InsertOutcome('Solve problems involving plane figures', 3, '3.2', '3', @mathId, @grade5Id);

CALL InsertOutcome('Identify 3D shapes and their properties', 1, '3.3', '3', @mathId, @grade5Id);
CALL InsertOutcome('Draw nets of 3D shapes', 2, '3.3', '3', @mathId, @grade5Id);
CALL InsertOutcome('Calculate surface area of cubes and cuboids', 3, '3.3', '3', @mathId, @grade5Id);

CALL InsertOutcome('Collect and organise data in frequency tables', 1, '4.1', '4', @mathId, @grade5Id);
CALL InsertOutcome('Draw and interpret bar graphs and line graphs', 2, '4.1', '4', @mathId, @grade5Id);
CALL InsertOutcome('Calculate mean median and mode', 3, '4.1', '4', @mathId, @grade5Id);
CALL InsertOutcome('Solve problems using statistical data', 4, '4.1', '4', @mathId, @grade5Id);

-- ============================================
-- MATHEMATICS Grade 6 - Specific Outcomes
-- ============================================

CALL InsertOutcome('Use place value and total value of digits up to hundreds of millions', 1, '1.1', '1', @mathId, @grade6Id);
CALL InsertOutcome('Read and write numbers up to hundreds of millions', 2, '1.1', '1', @mathId, @grade6Id);
CALL InsertOutcome('Order numbers up to 100000', 3, '1.1', '1', @mathId, @grade6Id);
CALL InsertOutcome('Round off numbers up to 100000 to nearest thousand', 4, '1.1', '1', @mathId, @grade6Id);
CALL InsertOutcome('Apply squares up to 100 and square roots of perfect squares up to 10000', 5, '1.1', '1', @mathId, @grade6Id);
CALL InsertOutcome('Appreciate use of whole numbers in real life', 6, '1.1', '1', @mathId, @grade6Id);

CALL InsertOutcome('Multiply up to a 4-digit number by a 2-digit number', 1, '1.2', '1', @mathId, @grade6Id);
CALL InsertOutcome('Estimate products by rounding off numbers', 2, '1.2', '1', @mathId, @grade6Id);
CALL InsertOutcome('Make patterns involving multiplication', 3, '1.2', '1', @mathId, @grade6Id);
CALL InsertOutcome('Appreciate use of multiplication in real life', 4, '1.2', '1', @mathId, @grade6Id);

CALL InsertOutcome('Divide up to a 4-digit number by up to a 3-digit number', 1, '1.3', '1', @mathId, @grade6Id);
CALL InsertOutcome('Estimate quotients by rounding off', 2, '1.3', '1', @mathId, @grade6Id);
CALL InsertOutcome('Perform combined operations', 3, '1.3', '1', @mathId, @grade6Id);
CALL InsertOutcome('Appreciate use of division in real life', 4, '1.3', '1', @mathId, @grade6Id);

CALL InsertOutcome('Add fractions using LCM', 1, '1.4', '1', @mathId, @grade6Id);
CALL InsertOutcome('Subtract fractions using LCM', 2, '1.4', '1', @mathId, @grade6Id);
CALL InsertOutcome('Add mixed numbers', 3, '1.4', '1', @mathId, @grade6Id);
CALL InsertOutcome('Subtract mixed numbers', 4, '1.4', '1', @mathId, @grade6Id);
CALL InsertOutcome('Identify reciprocals of proper fractions', 5, '1.4', '1', @mathId, @grade6Id);
CALL InsertOutcome('Work out squares of fractions', 6, '1.4', '1', @mathId, @grade6Id);
CALL InsertOutcome('Express fractions as percentages', 7, '1.4', '1', @mathId, @grade6Id);

CALL InsertOutcome('Add and subtract decimals up to thousandths', 1, '1.5', '1', @mathId, @grade6Id);
CALL InsertOutcome('Multiply decimals', 2, '1.5', '1', @mathId, @grade6Id);
CALL InsertOutcome('Divide decimals', 3, '1.5', '1', @mathId, @grade6Id);
CALL InsertOutcome('Convert between fractions decimals and percentages', 4, '1.5', '1', @mathId, @grade6Id);

CALL InsertOutcome('Identify and use inequality symbols', 1, '1.6', '1', @mathId, @grade6Id);
CALL InsertOutcome('Solve simple inequalities', 2, '1.6', '1', @mathId, @grade6Id);
CALL InsertOutcome('Represent inequalities on a number line', 3, '1.6', '1', @mathId, @grade6Id);

CALL InsertOutcome('Convert between units of length', 1, '2.1', '2', @mathId, @grade6Id);
CALL InsertOutcome('Calculate perimeter of plane shapes', 2, '2.1', '2', @mathId, @grade6Id);
CALL InsertOutcome('Solve problems involving length in real life', 3, '2.1', '2', @mathId, @grade6Id);

CALL InsertOutcome('Calculate area of rectangles triangles and circles', 1, '2.2', '2', @mathId, @grade6Id);
CALL InsertOutcome('Solve problems involving area of composite shapes', 2, '2.2', '2', @mathId, @grade6Id);

CALL InsertOutcome('Convert between millilitres and litres', 1, '2.3', '2', @mathId, @grade6Id);
CALL InsertOutcome('Solve problems involving capacity', 2, '2.3', '2', @mathId, @grade6Id);

CALL InsertOutcome('Convert between units of mass', 1, '2.4', '2', @mathId, @grade6Id);
CALL InsertOutcome('Solve problems involving mass', 2, '2.4', '2', @mathId, @grade6Id);

CALL InsertOutcome('Calculate time in 12-hour and 24-hour formats', 1, '2.5', '2', @mathId, @grade6Id);
CALL InsertOutcome('Work out time zones', 2, '2.5', '2', @mathId, @grade6Id);
CALL InsertOutcome('Solve problems involving time', 3, '2.5', '2', @mathId, @grade6Id);

CALL InsertOutcome('Calculate profit and loss', 1, '2.6', '2', @mathId, @grade6Id);
CALL InsertOutcome('Calculate simple interest', 2, '2.6', '2', @mathId, @grade6Id);
CALL InsertOutcome('Solve problems involving money transactions', 3, '2.6', '2', @mathId, @grade6Id);

CALL InsertOutcome('Identify parallel perpendicular intersecting and transversal lines', 1, '3.1', '3', @mathId, @grade6Id);
CALL InsertOutcome('Calculate angles formed by transversals', 2, '3.1', '3', @mathId, @grade6Id);

CALL InsertOutcome('Classify angles', 1, '3.2', '3', @mathId, @grade6Id);
CALL InsertOutcome('Calculate angles in polygons', 2, '3.2', '3', @mathId, @grade6Id);
CALL InsertOutcome('Construct angles using a compass and ruler', 3, '3.2', '3', @mathId, @grade6Id);

CALL InsertOutcome('Identify properties of 3D objects', 1, '3.3', '3', @mathId, @grade6Id);
CALL InsertOutcome('Draw nets of 3D objects', 2, '3.3', '3', @mathId, @grade6Id);
CALL InsertOutcome('Calculate surface area and volume of 3D objects', 3, '3.3', '3', @mathId, @grade6Id);

CALL InsertOutcome('Collect and organise data', 1, '4.1', '4', @mathId, @grade6Id);
CALL InsertOutcome('Draw vertical and horizontal bar graphs', 2, '4.1', '4', @mathId, @grade6Id);
CALL InsertOutcome('Interpret bar graphs', 3, '4.1', '4', @mathId, @grade6Id);
CALL InsertOutcome('Solve problems using data from bar graphs', 4, '4.1', '4', @mathId, @grade6Id);

-- ============================================
-- LITERACY ACTIVITIES Grades 1-3 - Specific Outcomes
-- ============================================

-- Grade 1
CALL InsertOutcome('Listen attentively to stories poems and instructions', 1, '1.1', '1', @litId, @grade1Id);
CALL InsertOutcome('Respond appropriately to instructions heard', 2, '1.1', '1', @litId, @grade1Id);
CALL InsertOutcome('Identify sounds in the environment', 3, '1.1', '1', @litId, @grade1Id);

CALL InsertOutcome('Speak clearly using simple sentences', 1, '1.2', '1', @litId, @grade1Id);
CALL InsertOutcome('Share ideas through oral communication', 2, '1.2', '1', @litId, @grade1Id);
CALL InsertOutcome('Use polite language in conversations', 3, '1.2', '1', @litId, @grade1Id);

CALL InsertOutcome('Identify rhyming words', 1, '2.1', '2', @litId, @grade1Id);
CALL InsertOutcome('Segment words into syllables', 2, '2.1', '2', @litId, @grade1Id);
CALL InsertOutcome('Blend sounds to form words', 3, '2.1', '2', @litId, @grade1Id);

CALL InsertOutcome('Match letters to their sounds', 1, '2.2', '2', @litId, @grade1Id);
CALL InsertOutcome('Blend consonant-vowel-consonant (CVC) words', 2, '2.2', '2', @litId, @grade1Id);
CALL InsertOutcome('Read simple decodable texts', 3, '2.2', '2', @litId, @grade1Id);

CALL InsertOutcome('Read simple sentences with understanding', 1, '2.3', '2', @litId, @grade1Id);
CALL InsertOutcome('Answer questions about a text read', 2, '2.3', '2', @litId, @grade1Id);
CALL InsertOutcome('Identify characters and setting in a story', 3, '2.3', '2', @litId, @grade1Id);

CALL InsertOutcome('Trace and copy letters correctly', 1, '3.1', '3', @litId, @grade1Id);
CALL InsertOutcome('Hold a pencil correctly', 2, '3.1', '3', @litId, @grade1Id);
CALL InsertOutcome('Write on the line', 3, '3.1', '3', @litId, @grade1Id);

CALL InsertOutcome('Copy simple sentences', 1, '3.2', '3', @litId, @grade1Id);
CALL InsertOutcome('Write simple sentences from dictation', 2, '3.2', '3', @litId, @grade1Id);
CALL InsertOutcome('Appreciate the importance of writing', 3, '3.2', '3', @litId, @grade1Id);

-- Literacy Grade 2
CALL InsertOutcome('Listen to and retell a story heard', 1, '1.1', '1', @litId, @grade2Id);
CALL InsertOutcome('Discuss topics of interest', 2, '1.1', '1', @litId, @grade2Id);
CALL InsertOutcome('Express opinions in a group discussion', 3, '1.1', '1', @litId, @grade2Id);

CALL InsertOutcome('Read high-frequency words fluently', 1, '2.1', '2', @litId, @grade2Id);
CALL InsertOutcome('Use context clues to find meaning of new words', 2, '2.1', '2', @litId, @grade2Id);
CALL InsertOutcome('Read simple texts fluently', 3, '2.1', '2', @litId, @grade2Id);

CALL InsertOutcome('Read a passage and answer literal questions', 1, '2.2', '2', @litId, @grade2Id);
CALL InsertOutcome('Identify the main idea in a passage', 2, '2.2', '2', @litId, @grade2Id);
CALL InsertOutcome('Retell a story in own words', 3, '2.2', '2', @litId, @grade2Id);

CALL InsertOutcome('Write simple sentences using given words', 1, '3.1', '3', @litId, @grade2Id);
CALL InsertOutcome('Punctuate sentences correctly using capital letter and full stop', 2, '3.1', '3', @litId, @grade2Id);
CALL InsertOutcome('Write a short paragraph with guidance', 3, '3.1', '3', @litId, @grade2Id);

-- Literacy Grade 3
CALL InsertOutcome('Give and follow multi-step instructions', 1, '1.1', '1', @litId, @grade3Id);
CALL InsertOutcome('Present information to the class', 2, '1.1', '1', @litId, @grade3Id);
CALL InsertOutcome('Participate in group discussions and debates', 3, '1.1', '1', @litId, @grade3Id);

CALL InsertOutcome('Read grade-level texts fluently', 1, '2.1', '2', @litId, @grade3Id);
CALL InsertOutcome('Identify main idea supporting details and conclusion', 2, '2.1', '2', @litId, @grade3Id);
CALL InsertOutcome('Make inferences from a text', 3, '2.1', '2', @litId, @grade3Id);
CALL InsertOutcome('Identify different text types', 4, '2.1', '2', @litId, @grade3Id);

CALL InsertOutcome('Use a dictionary to find meanings of words', 1, '2.2', '2', @litId, @grade3Id);
CALL InsertOutcome('Identify synonyms and antonyms', 2, '2.2', '2', @litId, @grade3Id);
CALL InsertOutcome('Use new vocabulary in sentences', 3, '2.2', '2', @litId, @grade3Id);

CALL InsertOutcome('Write a short story with beginning middle and end', 1, '3.1', '3', @litId, @grade3Id);
CALL InsertOutcome('Write a simple poem', 2, '3.1', '3', @litId, @grade3Id);
CALL InsertOutcome('Use punctuation marks correctly', 3, '3.1', '3', @litId, @grade3Id);
CALL InsertOutcome('Edit and improve own writing', 4, '3.1', '3', @litId, @grade3Id);

-- ============================================
-- ENGLISH LANGUAGE ACTIVITIES Grades 1-3 - Specific Outcomes
-- ============================================

-- Grade 1
CALL InsertOutcome('Listen attentively to simple English stories', 1, '1.1', '1', @engLangId, @grade1Id);
CALL InsertOutcome('Follow simple oral instructions in English', 2, '1.1', '1', @engLangId, @grade1Id);
CALL InsertOutcome('Identify common sounds words and expressions', 3, '1.1', '1', @engLangId, @grade1Id);

CALL InsertOutcome('Respond to simple questions in English', 1, '1.2', '1', @engLangId, @grade1Id);
CALL InsertOutcome('Use common greetings and courtesies in English', 2, '1.2', '1', @engLangId, @grade1Id);
CALL InsertOutcome('Introduce self using simple English sentences', 3, '1.2', '1', @engLangId, @grade1Id);

CALL InsertOutcome('Identify and pronounce letters of the alphabet', 1, '2.1', '2', @engLangId, @grade1Id);
CALL InsertOutcome('Blend simple CVC words in English', 2, '2.1', '2', @engLangId, @grade1Id);
CALL InsertOutcome('Read simple English words and sentences', 3, '2.1', '2', @engLangId, @grade1Id);

CALL InsertOutcome('Read simple English sentences', 1, '2.2', '2', @engLangId, @grade1Id);
CALL InsertOutcome('Answer questions about a short text', 2, '2.2', '2', @engLangId, @grade1Id);
CALL InsertOutcome('Match words to pictures', 3, '2.2', '2', @engLangId, @grade1Id);

CALL InsertOutcome('Trace and write letters of the English alphabet', 1, '3.1', '3', @engLangId, @grade1Id);
CALL InsertOutcome('Write simple English words correctly', 2, '3.1', '3', @engLangId, @grade1Id);
CALL InsertOutcome('Copy simple English sentences', 3, '3.1', '3', @engLangId, @grade1Id);

CALL InsertOutcome('Use singular and plural nouns', 1, '4.1', '4', @engLangId, @grade1Id);
CALL InsertOutcome('Use simple present tense verbs', 2, '4.1', '4', @engLangId, @grade1Id);
CALL InsertOutcome('Identify common and proper nouns', 3, '4.1', '4', @engLangId, @grade1Id);
CALL InsertOutcome('Build vocabulary through songs poems and stories', 4, '4.1', '4', @engLangId, @grade1Id);

-- English Lang Grade 2
CALL InsertOutcome('Listen to and retell simple English stories', 1, '1.1', '1', @engLangId, @grade2Id);
CALL InsertOutcome('Ask and answer questions in English', 2, '1.1', '1', @engLangId, @grade2Id);
CALL InsertOutcome('Participate in show-and-tell activities', 3, '1.1', '1', @engLangId, @grade2Id);

CALL InsertOutcome('Read simple English texts aloud fluently', 1, '2.1', '2', @engLangId, @grade2Id);
CALL InsertOutcome('Use punctuation cues when reading', 2, '2.1', '2', @engLangId, @grade2Id);
CALL InsertOutcome('Read familiar texts with expression', 3, '2.1', '2', @engLangId, @grade2Id);

CALL InsertOutcome('Identify characters and events in a story', 1, '2.2', '2', @engLangId, @grade2Id);
CALL InsertOutcome('Answer comprehension questions in full sentences', 2, '2.2', '2', @engLangId, @grade2Id);
CALL InsertOutcome('Retell a story in correct sequence', 3, '2.2', '2', @engLangId, @grade2Id);

CALL InsertOutcome('Write sentences using given words', 1, '3.1', '3', @engLangId, @grade2Id);
CALL InsertOutcome('Write a short paragraph about a familiar topic', 2, '3.1', '3', @engLangId, @grade2Id);
CALL InsertOutcome('Use capital letters and full stops correctly', 3, '3.1', '3', @engLangId, @grade2Id);

CALL InsertOutcome('Use pronouns I you he she it we they', 1, '4.1', '4', @engLangId, @grade2Id);
CALL InsertOutcome('Use present continuous tense', 2, '4.1', '4', @engLangId, @grade2Id);
CALL InsertOutcome('Use adjectives to describe nouns', 3, '4.1', '4', @engLangId, @grade2Id);
CALL InsertOutcome('Use conjunctions and but or', 4, '4.1', '4', @engLangId, @grade2Id);

-- English Lang Grade 3
CALL InsertOutcome('Listen to and summarise information from audio materials', 1, '1.1', '1', @engLangId, @grade3Id);
CALL InsertOutcome('Give oral presentations on familiar topics', 2, '1.1', '1', @engLangId, @grade3Id);
CALL InsertOutcome('Use appropriate vocabulary for different audiences', 3, '1.1', '1', @engLangId, @grade3Id);

CALL InsertOutcome('Read and understand a variety of texts', 1, '2.1', '2', @engLangId, @grade3Id);
CALL InsertOutcome('Identify purpose and audience of a text', 2, '2.1', '2', @engLangId, @grade3Id);
CALL InsertOutcome('Use reading strategies skimming and scanning', 3, '2.1', '2', @engLangId, @grade3Id);
CALL InsertOutcome('Identify figurative language in poems and stories', 4, '2.1', '2', @engLangId, @grade3Id);

CALL InsertOutcome('Write a story with a clear beginning middle and end', 1, '3.1', '3', @engLangId, @grade3Id);
CALL InsertOutcome('Write a simple letter', 2, '3.1', '3', @engLangId, @grade3Id);
CALL InsertOutcome('Write a descriptive paragraph', 3, '3.1', '3', @engLangId, @grade3Id);
CALL InsertOutcome('Use a range of sentence types', 4, '3.1', '3', @engLangId, @grade3Id);

CALL InsertOutcome('Use past simple and past continuous tenses', 1, '4.1', '4', @engLangId, @grade3Id);
CALL InsertOutcome('Use degrees of comparison', 2, '4.1', '4', @engLangId, @grade3Id);
CALL InsertOutcome('Use prepositions of place and time', 3, '4.1', '4', @engLangId, @grade3Id);
CALL InsertOutcome('Use articles a an the', 4, '4.1', '4', @engLangId, @grade3Id);

-- ============================================
-- ENGLISH Grades 4-6 - Specific Outcomes
-- ============================================

-- English Grade 4
CALL InsertOutcome('Listen to and comprehend extended spoken texts', 1, '1.1', '1', @engId, @grade4Id);
CALL InsertOutcome('Identify main ideas and supporting details from audio', 2, '1.1', '1', @engId, @grade4Id);
CALL InsertOutcome('Follow multi-step oral instructions', 3, '1.1', '1', @engId, @grade4Id);
CALL InsertOutcome('Take notes from spoken texts', 4, '1.1', '1', @engId, @grade4Id);

CALL InsertOutcome('Participate in structured discussions and debates', 1, '1.2', '1', @engId, @grade4Id);
CALL InsertOutcome('Give oral presentations on various topics', 2, '1.2', '1', @engId, @grade4Id);
CALL InsertOutcome('Use appropriate tone register and vocabulary', 3, '1.2', '1', @engId, @grade4Id);
CALL InsertOutcome('Appreciate diverse perspectives in discussions', 4, '1.2', '1', @engId, @grade4Id);

CALL InsertOutcome('Read varied texts fluently and with expression', 1, '2.1', '2', @engId, @grade4Id);
CALL InsertOutcome('Identify themes main ideas and supporting details', 2, '2.1', '2', @engId, @grade4Id);
CALL InsertOutcome('Make inferences and draw conclusions from texts', 3, '2.1', '2', @engId, @grade4Id);
CALL InsertOutcome('Distinguish between fact and opinion', 4, '2.1', '2', @engId, @grade4Id);

CALL InsertOutcome('Use context clues to determine meaning of unfamiliar words', 1, '2.2', '2', @engId, @grade4Id);
CALL InsertOutcome('Identify prefixes suffixes and root words', 2, '2.2', '2', @engId, @grade4Id);
CALL InsertOutcome('Build vocabulary through wide reading', 3, '2.2', '2', @engId, @grade4Id);

CALL InsertOutcome('Write letters emails and notices', 1, '3.1', '3', @engId, @grade4Id);
CALL InsertOutcome('Write reports and summaries', 2, '3.1', '3', @engId, @grade4Id);
CALL InsertOutcome('Use appropriate format for different writing purposes', 3, '3.1', '3', @engId, @grade4Id);

CALL InsertOutcome('Write narratives with well-developed characters and plot', 1, '3.2', '3', @engId, @grade4Id);
CALL InsertOutcome('Write descriptive texts using vivid language', 2, '3.2', '3', @engId, @grade4Id);
CALL InsertOutcome('Write poems using various poetic forms', 3, '3.2', '3', @engId, @grade4Id);

CALL InsertOutcome('Use various types of nouns and pronouns', 1, '4.1', '4', @engId, @grade4Id);
CALL InsertOutcome('Use tenses accurately simple present past future perfect', 2, '4.1', '4', @engId, @grade4Id);
CALL InsertOutcome('Use conjunctions prepositions and interjections', 3, '4.1', '4', @engId, @grade4Id);
CALL InsertOutcome('Use active and passive voice', 4, '4.1', '4', @engId, @grade4Id);

-- English Grade 5
CALL InsertOutcome('Listen to and evaluate spoken texts', 1, '1.1', '1', @engId, @grade5Id);
CALL InsertOutcome('Participate in formal and informal discussions', 2, '1.1', '1', @engId, @grade5Id);
CALL InsertOutcome('Deliver structured oral presentations', 3, '1.1', '1', @engId, @grade5Id);
CALL InsertOutcome('Use persuasive language effectively', 4, '1.1', '1', @engId, @grade5Id);

CALL InsertOutcome('Read and analyse literary and non-literary texts', 1, '2.1', '2', @engId, @grade5Id);
CALL InsertOutcome('Evaluate author''s purpose and viewpoint', 2, '2.1', '2', @engId, @grade5Id);
CALL InsertOutcome('Identify literary devices simile metaphor alliteration', 3, '2.1', '2', @engId, @grade5Id);
CALL InsertOutcome('Appreciate diverse literary works', 4, '2.1', '2', @engId, @grade5Id);

CALL InsertOutcome('Write expository and argumentative essays', 1, '3.1', '3', @engId, @grade5Id);
CALL InsertOutcome('Write formal and informal letters', 2, '3.1', '3', @engId, @grade5Id);
CALL InsertOutcome('Use editing and proofreading skills to improve writing', 3, '3.1', '3', @engId, @grade5Id);

CALL InsertOutcome('Use complex and compound sentences', 1, '4.1', '4', @engId, @grade5Id);
CALL InsertOutcome('Use reported speech correctly', 2, '4.1', '4', @engId, @grade5Id);
CALL InsertOutcome('Apply rules of subject-verb agreement', 3, '4.1', '4', @engId, @grade5Id);
CALL InsertOutcome('Use punctuation accurately', 4, '4.1', '4', @engId, @grade5Id);

-- English Grade 6
CALL InsertOutcome('Evaluate and critically respond to spoken texts', 1, '1.1', '1', @engId, @grade6Id);
CALL InsertOutcome('Lead and participate in debates', 2, '1.1', '1', @engId, @grade6Id);
CALL InsertOutcome('Use formal language in presentations', 3, '1.1', '1', @engId, @grade6Id);
CALL InsertOutcome('Appreciate the power of effective communication', 4, '1.1', '1', @engId, @grade6Id);

CALL InsertOutcome('Read and critically analyse texts from different genres', 1, '2.1', '2', @engId, @grade6Id);
CALL InsertOutcome('Compare and contrast texts from different contexts', 2, '2.1', '2', @engId, @grade6Id);
CALL InsertOutcome('Identify bias and prejudice in texts', 3, '2.1', '2', @engId, @grade6Id);
CALL InsertOutcome('Appreciate world literature', 4, '2.1', '2', @engId, @grade6Id);

CALL InsertOutcome('Write well-structured essays', 1, '3.1', '3', @engId, @grade6Id);
CALL InsertOutcome('Write minutes agendas and formal reports', 2, '3.1', '3', @engId, @grade6Id);
CALL InsertOutcome('Use varied sentence structures and vocabulary', 3, '3.1', '3', @engId, @grade6Id);
CALL InsertOutcome('Edit and publish own writing', 4, '3.1', '3', @engId, @grade6Id);

CALL InsertOutcome('Use conditional sentences', 1, '4.1', '4', @engId, @grade6Id);
CALL InsertOutcome('Use phrasal verbs and idiomatic expressions', 2, '4.1', '4', @engId, @grade6Id);
CALL InsertOutcome('Apply advanced punctuation rules', 3, '4.1', '4', @engId, @grade6Id);
CALL InsertOutcome('Appreciate the role of language in society', 4, '4.1', '4', @engId, @grade6Id);

-- ============================================
-- KISWAHILI Grades 1-3 - Specific Outcomes
-- ============================================

-- Grade 1
CALL InsertOutcome('Kusikiliza kwa makini hadithi na mashairi', 1, '1.1', '1', @kisId, @grade1Id);
CALL InsertOutcome('Kutambua sauti za lugha ya Kiswahili', 2, '1.1', '1', @kisId, @grade1Id);
CALL InsertOutcome('Kufuata maelekezo rahisi', 3, '1.1', '1', @kisId, @grade1Id);

CALL InsertOutcome('Kujibu maswali kwa Kiswahili', 1, '1.2', '1', @kisId, @grade1Id);
CALL InsertOutcome('Kutumia maneno ya heshima katika mazungumzo', 2, '1.2', '1', @kisId, @grade1Id);
CALL InsertOutcome('Kujitambulisha kwa Kiswahili', 3, '1.2', '1', @kisId, @grade1Id);

CALL InsertOutcome('Kutambua herufi na sauti zake', 1, '2.1', '2', @kisId, @grade1Id);
CALL InsertOutcome('Kuunganisha silabi kuunda maneno', 2, '2.1', '2', @kisId, @grade1Id);
CALL InsertOutcome('Kusoma maneno na sentensi rahisi', 3, '2.1', '2', @kisId, @grade1Id);

CALL InsertOutcome('Kusoma sentensi rahisi za Kiswahili', 1, '2.2', '2', @kisId, @grade1Id);
CALL InsertOutcome('Kujibu maswali kuhusu maandishi', 2, '2.2', '2', @kisId, @grade1Id);
CALL InsertOutcome('Kuoanisha maneno na picha', 3, '2.2', '2', @kisId, @grade1Id);

CALL InsertOutcome('Kunakili herufi za alfabeti kwa Kiswahili', 1, '3.1', '3', @kisId, @grade1Id);
CALL InsertOutcome('Kuandika maneno rahisi ya Kiswahili', 2, '3.1', '3', @kisId, @grade1Id);
CALL InsertOutcome('Kunakili sentensi rahisi', 3, '3.1', '3', @kisId, @grade1Id);

CALL InsertOutcome('Kutambua majina ya kawaida', 1, '4.1', '4', @kisId, @grade1Id);
CALL InsertOutcome('Kutumia vitenzi rahisi vya wakati wa sasa', 2, '4.1', '4', @kisId, @grade1Id);
CALL InsertOutcome('Kujifunza msamiati kupitia nyimbo na hadithi', 3, '4.1', '4', @kisId, @grade1Id);

-- Kiswahili Grade 2
CALL InsertOutcome('Kusikiliza na kusimulia hadithi rahisi', 1, '1.1', '1', @kisId, @grade2Id);
CALL InsertOutcome('Kuuliza na kujibu maswali kwa Kiswahili', 2, '1.1', '1', @kisId, @grade2Id);
CALL InsertOutcome('Kushiriki katika mazungumzo ya kikundi', 3, '1.1', '1', @kisId, @grade2Id);

CALL InsertOutcome('Kusoma maandishi rahisi kwa ufasaha', 1, '2.1', '2', @kisId, @grade2Id);
CALL InsertOutcome('Kutumia alama za uandishi wakati wa kusoma', 2, '2.1', '2', @kisId, @grade2Id);
CALL InsertOutcome('Kusoma kwa hisia', 3, '2.1', '2', @kisId, @grade2Id);

CALL InsertOutcome('Kutambua wahusika na matukio katika hadithi', 1, '2.2', '2', @kisId, @grade2Id);
CALL InsertOutcome('Kujibu maswali ya ufahamu kwa sentensi kamili', 2, '2.2', '2', @kisId, @grade2Id);
CALL InsertOutcome('Kusimulia hadithi kwa mpangilio sahihi', 3, '2.2', '2', @kisId, @grade2Id);

CALL InsertOutcome('Kuandika sentensi kwa kutumia maneno yaliyopewa', 1, '3.1', '3', @kisId, @grade2Id);
CALL InsertOutcome('Kuandika aya fupi', 2, '3.1', '3', @kisId, @grade2Id);
CALL InsertOutcome('Kutumia herufi kubwa na nukta', 3, '3.1', '3', @kisId, @grade2Id);

CALL InsertOutcome('Kutumia viwakilishi', 1, '4.1', '4', @kisId, @grade2Id);
CALL InsertOutcome('Kutumia wakati wa sasa endelevu', 2, '4.1', '4', @kisId, @grade2Id);
CALL InsertOutcome('Kutumia vivumishi vya sifa', 3, '4.1', '4', @kisId, @grade2Id);
CALL InsertOutcome('Kutumia viunganishi na lakini au', 4, '4.1', '4', @kisId, @grade2Id);

-- Kiswahili Grade 3
CALL InsertOutcome('Kusikiliza na kufupisha habari', 1, '1.1', '1', @kisId, @grade3Id);
CALL InsertOutcome('Kutoa hotuba fupi mbele ya wenzake', 2, '1.1', '1', @kisId, @grade3Id);
CALL InsertOutcome('Kujadili mada kwa kutumia hoja sahihi', 3, '1.1', '1', @kisId, @grade3Id);

CALL InsertOutcome('Kusoma na kuelewa aina mbalimbali za maandishi', 1, '2.1', '2', @kisId, @grade3Id);
CALL InsertOutcome('Kutambua kusudi la maandishi', 2, '2.1', '2', @kisId, @grade3Id);
CALL InsertOutcome('Kutumia mbinu za usomaji', 3, '2.1', '2', @kisId, @grade3Id);
CALL InsertOutcome('Kutambua lugha ya sitiari', 4, '2.1', '2', @kisId, @grade3Id);

CALL InsertOutcome('Kuandika hadithi yenye mwanzo kati na mwisho', 1, '3.1', '3', @kisId, @grade3Id);
CALL InsertOutcome('Kuandika barua rahisi', 2, '3.1', '3', @kisId, @grade3Id);
CALL InsertOutcome('Kuandika aya ya maelezo', 3, '3.1', '3', @kisId, @grade3Id);
CALL InsertOutcome('Kutumia aina mbalimbali za sentensi', 4, '3.1', '3', @kisId, @grade3Id);

CALL InsertOutcome('Kutumia wakati uliopita', 1, '4.1', '4', @kisId, @grade3Id);
CALL InsertOutcome('Kutumia ngeli za nomino za Kiswahili', 2, '4.1', '4', @kisId, @grade3Id);
CALL InsertOutcome('Kutumia vielelezo vya mahali na wakati', 3, '4.1', '4', @kisId, @grade3Id);

-- ============================================
-- ENVIRONMENTAL ACTIVITIES Grades 1-3 - Specific Outcomes
-- ============================================

-- Grade 1
CALL InsertOutcome('Identify rooms and their functions at home', 1, '1.1', '1', @envId, @grade1Id);
CALL InsertOutcome('Identify members of the family', 2, '1.1', '1', @envId, @grade1Id);
CALL InsertOutcome('Describe duties of family members', 3, '1.1', '1', @envId, @grade1Id);
CALL InsertOutcome('Appreciate the importance of family', 4, '1.1', '1', @envId, @grade1Id);

CALL InsertOutcome('Identify parts of the school and their functions', 1, '1.2', '1', @envId, @grade1Id);
CALL InsertOutcome('Identify school community members and their roles', 2, '1.2', '1', @envId, @grade1Id);
CALL InsertOutcome('Describe duties of school community members', 3, '1.2', '1', @envId, @grade1Id);

CALL InsertOutcome('Identify people who help us in the community', 1, '1.3', '1', @envId, @grade1Id);
CALL InsertOutcome('Describe roles of community helpers', 2, '1.3', '1', @envId, @grade1Id);
CALL InsertOutcome('Appreciate the contribution of community helpers', 3, '1.3', '1', @envId, @grade1Id);

CALL InsertOutcome('Name common plants in the environment', 1, '2.1', '2', @envId, @grade1Id);
CALL InsertOutcome('Identify the parts of a plant', 2, '2.1', '2', @envId, @grade1Id);
CALL InsertOutcome('Describe uses of plants', 3, '2.1', '2', @envId, @grade1Id);
CALL InsertOutcome('Take care of plants', 4, '2.1', '2', @envId, @grade1Id);

CALL InsertOutcome('Name common animals in the environment', 1, '2.2', '2', @envId, @grade1Id);
CALL InsertOutcome('Classify animals as domestic or wild', 2, '2.2', '2', @envId, @grade1Id);
CALL InsertOutcome('Describe uses of domestic animals', 3, '2.2', '2', @envId, @grade1Id);
CALL InsertOutcome('Take care of domestic animals', 4, '2.2', '2', @envId, @grade1Id);

CALL InsertOutcome('Identify types of soil in the environment', 1, '3.1', '3', @envId, @grade1Id);
CALL InsertOutcome('Describe properties of different types of soil', 2, '3.1', '3', @envId, @grade1Id);
CALL InsertOutcome('Appreciate the importance of soil', 3, '3.1', '3', @envId, @grade1Id);

CALL InsertOutcome('Identify sources of water', 1, '3.2', '3', @envId, @grade1Id);
CALL InsertOutcome('Describe properties of water', 2, '3.2', '3', @envId, @grade1Id);
CALL InsertOutcome('Identify uses of water', 3, '3.2', '3', @envId, @grade1Id);
CALL InsertOutcome('Appreciate the importance of clean water', 4, '3.2', '3', @envId, @grade1Id);

CALL InsertOutcome('Identify properties of air', 1, '3.3', '3', @envId, @grade1Id);
CALL InsertOutcome('Describe uses of air', 2, '3.3', '3', @envId, @grade1Id);
CALL InsertOutcome('Appreciate the importance of clean air', 3, '3.3', '3', @envId, @grade1Id);

CALL InsertOutcome('Identify types of weather', 1, '4.1', '4', @envId, @grade1Id);
CALL InsertOutcome('Describe characteristics of different weather types', 2, '4.1', '4', @envId, @grade1Id);
CALL InsertOutcome('Relate weather to human activities', 3, '4.1', '4', @envId, @grade1Id);

-- Environmental Grade 2
CALL InsertOutcome('Describe the home environment', 1, '1.1', '1', @envId, @grade2Id);
CALL InsertOutcome('Identify materials used in building a house', 2, '1.1', '1', @envId, @grade2Id);
CALL InsertOutcome('Appreciate different types of houses', 3, '1.1', '1', @envId, @grade2Id);

CALL InsertOutcome('Describe the school environment', 1, '1.2', '1', @envId, @grade2Id);
CALL InsertOutcome('Identify facilities in the school', 2, '1.2', '1', @envId, @grade2Id);
CALL InsertOutcome('Appreciate the importance of school facilities', 3, '1.2', '1', @envId, @grade2Id);

CALL InsertOutcome('Classify plants as trees shrubs herbs and creepers', 1, '2.1', '2', @envId, @grade2Id);
CALL InsertOutcome('Describe uses of various plants', 2, '2.1', '2', @envId, @grade2Id);
CALL InsertOutcome('Grow plants from seeds', 3, '2.1', '2', @envId, @grade2Id);
CALL InsertOutcome('Appreciate the importance of plants', 4, '2.1', '2', @envId, @grade2Id);

CALL InsertOutcome('Classify animals by their body covering', 1, '2.2', '2', @envId, @grade2Id);
CALL InsertOutcome('Describe food eaten by different animals', 2, '2.2', '2', @envId, @grade2Id);
CALL InsertOutcome('Appreciate the importance of animals', 3, '2.2', '2', @envId, @grade2Id);

CALL InsertOutcome('Describe how soil is formed', 1, '3.1', '3', @envId, @grade2Id);
CALL InsertOutcome('Identify uses of soil', 2, '3.1', '3', @envId, @grade2Id);
CALL InsertOutcome('Carry out simple experiments on soil', 3, '3.1', '3', @envId, @grade2Id);
CALL InsertOutcome('Appreciate soil conservation', 4, '3.1', '3', @envId, @grade2Id);

CALL InsertOutcome('Classify sources of water as natural or man-made', 1, '3.2', '3', @envId, @grade2Id);
CALL InsertOutcome('Describe ways of conserving water', 2, '3.2', '3', @envId, @grade2Id);
CALL InsertOutcome('Appreciate the importance of clean water', 3, '3.2', '3', @envId, @grade2Id);

CALL InsertOutcome('Identify types of rocks in the environment', 1, '3.3', '3', @envId, @grade2Id);
CALL InsertOutcome('Describe properties and uses of rocks', 2, '3.3', '3', @envId, @grade2Id);

CALL InsertOutcome('Identify rainy and dry seasons', 1, '4.1', '4', @envId, @grade2Id);
CALL InsertOutcome('Describe effects of seasons on human activities', 2, '4.1', '4', @envId, @grade2Id);
CALL InsertOutcome('Appreciate the importance of different seasons', 3, '4.1', '4', @envId, @grade2Id);

-- Environmental Grade 3
CALL InsertOutcome('Describe rural and urban communities', 1, '1.1', '1', @envId, @grade3Id);
CALL InsertOutcome('Compare features of rural and urban communities', 2, '1.1', '1', @envId, @grade3Id);
CALL InsertOutcome('Appreciate diversity in communities', 3, '1.1', '1', @envId, @grade3Id);

CALL InsertOutcome('Identify means of transport in the community', 1, '1.2', '1', @envId, @grade3Id);
CALL InsertOutcome('Identify means of communication', 2, '1.2', '1', @envId, @grade3Id);
CALL InsertOutcome('Describe how transport and communication have changed', 3, '1.2', '1', @envId, @grade3Id);

CALL InsertOutcome('Describe photosynthesis in simple terms', 1, '2.1', '2', @envId, @grade3Id);
CALL InsertOutcome('Identify conditions necessary for plant growth', 2, '2.1', '2', @envId, @grade3Id);
CALL InsertOutcome('Grow plants under different conditions', 3, '2.1', '2', @envId, @grade3Id);

CALL InsertOutcome('Describe the life cycle of a butterfly and a frog', 1, '2.2', '2', @envId, @grade3Id);
CALL InsertOutcome('Identify characteristics of major animal groups', 2, '2.2', '2', @envId, @grade3Id);
CALL InsertOutcome('Appreciate interdependence of plants and animals', 3, '2.2', '2', @envId, @grade3Id);

CALL InsertOutcome('Describe types of soil and their uses', 1, '3.1', '3', @envId, @grade3Id);
CALL InsertOutcome('Identify effects of soil erosion', 2, '3.1', '3', @envId, @grade3Id);
CALL InsertOutcome('Describe ways of preventing soil erosion', 3, '3.1', '3', @envId, @grade3Id);

CALL InsertOutcome('Describe the water cycle', 1, '3.2', '3', @envId, @grade3Id);
CALL InsertOutcome('Identify effects of water pollution', 2, '3.2', '3', @envId, @grade3Id);
CALL InsertOutcome('Suggest ways of conserving water', 3, '3.2', '3', @envId, @grade3Id);

CALL InsertOutcome('Draw a simple map of the classroom', 1, '3.3', '3', @envId, @grade3Id);
CALL InsertOutcome('Draw a simple map of the school', 2, '3.3', '3', @envId, @grade3Id);
CALL InsertOutcome('Use cardinal directions North South East West', 3, '3.3', '3', @envId, @grade3Id);

CALL InsertOutcome('Identify sources of energy sun fire electricity wind water', 1, '4.1', '4', @envId, @grade3Id);
CALL InsertOutcome('Describe uses of different energy sources', 2, '4.1', '4', @envId, @grade3Id);
CALL InsertOutcome('Appreciate energy conservation', 3, '4.1', '4', @envId, @grade3Id);

-- ============================================
-- HYGIENE AND NUTRITION Grades 1-3 - Specific Outcomes
-- ============================================

-- Grade 1
CALL InsertOutcome('Demonstrate proper hand washing techniques', 1, '1.1', '1', @hygId, @grade1Id);
CALL InsertOutcome('Brush teeth correctly', 2, '1.1', '1', @hygId, @grade1Id);
CALL InsertOutcome('Keep body clean through bathing and grooming', 3, '1.1', '1', @hygId, @grade1Id);
CALL InsertOutcome('Appreciate the importance of personal hygiene', 4, '1.1', '1', @hygId, @grade1Id);

CALL InsertOutcome('Identify ways to keep the home clean', 1, '1.2', '1', @hygId, @grade1Id);
CALL InsertOutcome('Identify ways to keep the school clean', 2, '1.2', '1', @hygId, @grade1Id);
CALL InsertOutcome('Appreciate the importance of a clean environment', 3, '1.2', '1', @hygId, @grade1Id);

CALL InsertOutcome('Identify foods that help the body grow', 1, '2.1', '2', @hygId, @grade1Id);
CALL InsertOutcome('Identify foods that give the body energy', 2, '2.1', '2', @hygId, @grade1Id);
CALL InsertOutcome('Identify foods that protect the body from disease', 3, '2.1', '2', @hygId, @grade1Id);
CALL InsertOutcome('Appreciate the importance of a balanced diet', 4, '2.1', '2', @hygId, @grade1Id);

CALL InsertOutcome('Describe proper ways to handle food', 1, '2.2', '2', @hygId, @grade1Id);
CALL InsertOutcome('Identify signs of spoilt food', 2, '2.2', '2', @hygId, @grade1Id);
CALL InsertOutcome('Appreciate the importance of eating safe food', 3, '2.2', '2', @hygId, @grade1Id);

CALL InsertOutcome('Identify safety rules at home and school', 1, '3.1', '3', @hygId, @grade1Id);
CALL InsertOutcome('Describe dangers in the environment', 2, '3.1', '3', @hygId, @grade1Id);
CALL InsertOutcome('Apply safety rules in daily life', 3, '3.1', '3', @hygId, @grade1Id);

-- Hygiene Grade 2
CALL InsertOutcome('Demonstrate care of hair and nails', 1, '1.1', '1', @hygId, @grade2Id);
CALL InsertOutcome('Identify appropriate clothing for different weather', 2, '1.1', '1', @hygId, @grade2Id);
CALL InsertOutcome('Appreciate proper grooming', 3, '1.1', '1', @hygId, @grade2Id);

CALL InsertOutcome('Describe proper waste disposal methods', 1, '1.2', '1', @hygId, @grade2Id);
CALL InsertOutcome('Identify sources of pollution', 2, '1.2', '1', @hygId, @grade2Id);
CALL InsertOutcome('Suggest ways to keep the environment clean', 3, '1.2', '1', @hygId, @grade2Id);

CALL InsertOutcome('Plan a balanced meal using locally available foods', 1, '2.1', '2', @hygId, @grade2Id);
CALL InsertOutcome('Prepare simple foods', 2, '2.1', '2', @hygId, @grade2Id);
CALL InsertOutcome('Appreciate eating a variety of foods', 3, '2.1', '2', @hygId, @grade2Id);

CALL InsertOutcome('Identify safe sources of drinking water', 1, '2.2', '2', @hygId, @grade2Id);
CALL InsertOutcome('Describe ways of purifying water', 2, '2.2', '2', @hygId, @grade2Id);
CALL InsertOutcome('Appreciate the importance of drinking clean water', 3, '2.2', '2', @hygId, @grade2Id);

CALL InsertOutcome('Identify dangers at home and school', 1, '3.1', '3', @hygId, @grade2Id);
CALL InsertOutcome('Apply first aid for minor injuries', 2, '3.1', '3', @hygId, @grade2Id);
CALL InsertOutcome('Appreciate the importance of reporting accidents', 3, '3.1', '3', @hygId, @grade2Id);

-- Hygiene Grade 3
CALL InsertOutcome('Identify diseases caused by poor hygiene', 1, '1.1', '1', @hygId, @grade3Id);
CALL InsertOutcome('Describe ways of preventing disease through hygiene', 2, '1.1', '1', @hygId, @grade3Id);
CALL InsertOutcome('Practise proper hygiene habits consistently', 3, '1.1', '1', @hygId, @grade3Id);

CALL InsertOutcome('Identify changes that occur during puberty', 1, '1.2', '1', @hygId, @grade3Id);
CALL InsertOutcome('Appreciate the importance of hygiene during puberty', 2, '1.2', '1', @hygId, @grade3Id);

CALL InsertOutcome('Identify main nutrients and their functions', 1, '2.1', '2', @hygId, @grade3Id);
CALL InsertOutcome('Describe signs of nutritional deficiency diseases', 2, '2.1', '2', @hygId, @grade3Id);
CALL InsertOutcome('Appreciate a balanced diet in maintaining health', 3, '2.1', '2', @hygId, @grade3Id);

CALL InsertOutcome('Describe methods of food preparation', 1, '2.2', '2', @hygId, @grade3Id);
CALL InsertOutcome('Describe proper ways to store different foods', 2, '2.2', '2', @hygId, @grade3Id);
CALL InsertOutcome('Appreciate hygiene in food handling', 3, '2.2', '2', @hygId, @grade3Id);

CALL InsertOutcome('Describe first aid for burns cuts and fractures', 1, '3.1', '3', @hygId, @grade3Id);
CALL InsertOutcome('Describe first aid for insect bites', 2, '3.1', '3', @hygId, @grade3Id);
CALL InsertOutcome('Appreciate basic first aid knowledge', 3, '3.1', '3', @hygId, @grade3Id);

-- ============================================
-- CRE Grades 1-3 - Specific Outcomes
-- ============================================

-- CRE Grade 1
CALL InsertOutcome('Recognise themselves as uniquely created in the image and likeness of God', 1, '1.1', '1', @creId, @grade1Id);
CALL InsertOutcome('Mention their names for identification and self-awareness', 2, '1.1', '1', @creId, @grade1Id);
CALL InsertOutcome('Recognise that God knows them by their names', 3, '1.1', '1', @creId, @grade1Id);
CALL InsertOutcome('Appreciate themselves as unique and special creation', 4, '1.1', '1', @creId, @grade1Id);

CALL InsertOutcome('Name members of the nuclear family for a sense of belonging', 1, '1.2', '1', @creId, @grade1Id);
CALL InsertOutcome('Pray with family members to promote unity', 2, '1.2', '1', @creId, @grade1Id);
CALL InsertOutcome('Identify items shared at home', 3, '1.2', '1', @creId, @grade1Id);
CALL InsertOutcome('Appreciate the importance of sharing for family unity', 4, '1.2', '1', @creId, @grade1Id);

CALL InsertOutcome('Mention plants and animals that God created', 1, '1.3', '1', @creId, @grade1Id);
CALL InsertOutcome('Care for plants and animals as part of God''s creation', 2, '1.3', '1', @creId, @grade1Id);
CALL InsertOutcome('Appreciate plants and animals as God''s creation', 3, '1.3', '1', @creId, @grade1Id);

CALL InsertOutcome('Identify ways of handling the Holy Bible with respect', 1, '2.1', '2', @creId, @grade1Id);
CALL InsertOutcome('Recognise the Holy Bible as the word of God', 2, '2.1', '2', @creId, @grade1Id);
CALL InsertOutcome('Name the first two Gospel books', 3, '2.1', '2', @creId, @grade1Id);
CALL InsertOutcome('State the two divisions Old and New Testament', 4, '2.1', '2', @creId, @grade1Id);

CALL InsertOutcome('Narrate the story of David and Goliath', 1, '2.2', '2', @creId, @grade1Id);
CALL InsertOutcome('Appreciate the story and desire to depend on God', 2, '2.2', '2', @creId, @grade1Id);
CALL InsertOutcome('Apply lessons of faith from the story', 3, '2.2', '2', @creId, @grade1Id);

CALL InsertOutcome('Narrate the story of Joseph and relate it to daily life', 1, '2.3', '2', @creId, @grade1Id);
CALL InsertOutcome('Discuss Joseph''s brothers'' reaction to his dreams', 2, '2.3', '2', @creId, @grade1Id);
CALL InsertOutcome('Apply lessons of love and forgiveness', 3, '2.3', '2', @creId, @grade1Id);

CALL InsertOutcome('Identify the city of Jesus'' birth', 1, '3.1', '3', @creId, @grade1Id);
CALL InsertOutcome('Mention the parents of Jesus Christ', 2, '3.1', '3', @creId, @grade1Id);
CALL InsertOutcome('Analyse the joy of the shepherds and relate to Christmas', 3, '3.1', '3', @creId, @grade1Id);
CALL InsertOutcome('Explain the naming and dedication of Jesus Christ', 4, '3.1', '3', @creId, @grade1Id);

-- CRE Grade 2
CALL InsertOutcome('Describe God''s creation as good and beautiful', 1, '1.1', '1', @creId, @grade2Id);
CALL InsertOutcome('Identify their role as stewards of God''s creation', 2, '1.1', '1', @creId, @grade2Id);
CALL InsertOutcome('Appreciate diversity in God''s creation', 3, '1.1', '1', @creId, @grade2Id);
CALL InsertOutcome('Take responsibility for caring for creation', 4, '1.1', '1', @creId, @grade2Id);

CALL InsertOutcome('Name books of the Old Testament', 1, '2.1', '2', @creId, @grade2Id);
CALL InsertOutcome('Name books of the New Testament', 2, '2.1', '2', @creId, @grade2Id);
CALL InsertOutcome('Locate specific books in the Bible', 3, '2.1', '2', @creId, @grade2Id);
CALL InsertOutcome('Appreciate the Bible as a guide for life', 4, '2.1', '2', @creId, @grade2Id);

CALL InsertOutcome('Narrate the story of Moses and the burning bush', 1, '2.2', '2', @creId, @grade2Id);
CALL InsertOutcome('Narrate the story of Jonah and the big fish', 2, '2.2', '2', @creId, @grade2Id);
CALL InsertOutcome('Apply moral lessons from Bible stories', 3, '2.2', '2', @creId, @grade2Id);

CALL InsertOutcome('Identify key teachings of Jesus', 1, '3.1', '3', @creId, @grade2Id);
CALL InsertOutcome('Apply Jesus'' teachings in everyday life', 2, '3.1', '3', @creId, @grade2Id);
CALL InsertOutcome('Appreciate Jesus as a role model', 3, '3.1', '3', @creId, @grade2Id);

-- CRE Grade 3
CALL InsertOutcome('Define stewardship in a Christian context', 1, '1.1', '1', @creId, @grade3Id);
CALL InsertOutcome('Describe ways of being good stewards of the environment', 2, '1.1', '1', @creId, @grade3Id);
CALL InsertOutcome('Appreciate the responsibility entrusted by God', 3, '1.1', '1', @creId, @grade3Id);

CALL InsertOutcome('Explain how the Bible guides Christian living', 1, '2.1', '2', @creId, @grade3Id);
CALL InsertOutcome('Identify key commandments and their relevance today', 2, '2.1', '2', @creId, @grade3Id);
CALL InsertOutcome('Apply biblical principles in everyday challenges', 3, '2.1', '2', @creId, @grade3Id);

CALL InsertOutcome('Narrate selected miracles of Jesus', 1, '3.1', '3', @creId, @grade3Id);
CALL InsertOutcome('Relate miracles to the power and compassion of God', 2, '3.1', '3', @creId, @grade3Id);
CALL InsertOutcome('Apply lessons of faith and compassion', 3, '3.1', '3', @creId, @grade3Id);

CALL InsertOutcome('Narrate the events of the Last Supper crucifixion and resurrection', 1, '3.2', '3', @creId, @grade3Id);
CALL InsertOutcome('Explain the significance of Easter', 2, '3.2', '3', @creId, @grade3Id);
CALL InsertOutcome('Appreciate the sacrifice of Jesus for humanity', 3, '3.2', '3', @creId, @grade3Id);

-- ============================================
-- IRE Grades 1-3 - Specific Outcomes
-- ============================================

-- IRE Grade 1
CALL InsertOutcome('Describe themselves as creations of Allah', 1, '1.1', '1', @ireId, @grade1Id);
CALL InsertOutcome('Identify members of the family', 2, '1.1', '1', @ireId, @grade1Id);
CALL InsertOutcome('Appreciate the family as a gift from Allah', 3, '1.1', '1', @ireId, @grade1Id);

CALL InsertOutcome('Identify the Quran as the Holy Book of Islam', 1, '2.1', '2', @ireId, @grade1Id);
CALL InsertOutcome('Learn to handle the Quran with respect', 2, '2.1', '2', @ireId, @grade1Id);
CALL InsertOutcome('Recite short Surahs from the Quran', 3, '2.1', '2', @ireId, @grade1Id);

CALL InsertOutcome('State the Shahadah and its meaning', 1, '3.1', '3', @ireId, @grade1Id);
CALL InsertOutcome('Identify the times of daily prayers', 2, '3.1', '3', @ireId, @grade1Id);
CALL InsertOutcome('Appreciate the importance of prayer', 3, '3.1', '3', @ireId, @grade1Id);

-- IRE Grade 2
CALL InsertOutcome('Describe Allah as the creator of all things', 1, '1.1', '1', @ireId, @grade2Id);
CALL InsertOutcome('Identify the names and attributes of Allah', 2, '1.1', '1', @ireId, @grade2Id);
CALL InsertOutcome('Appreciate Allah''s creation', 3, '1.1', '1', @ireId, @grade2Id);

CALL InsertOutcome('Read the Arabic alphabet', 1, '2.1', '2', @ireId, @grade2Id);
CALL InsertOutcome('Recite additional short Surahs', 2, '2.1', '2', @ireId, @grade2Id);
CALL InsertOutcome('Appreciate the importance of reading the Quran', 3, '2.1', '2', @ireId, @grade2Id);

CALL InsertOutcome('Describe the five pillars of Islam', 1, '3.1', '3', @ireId, @grade2Id);
CALL InsertOutcome('Demonstrate correct way of performing Salah', 2, '3.1', '3', @ireId, @grade2Id);
CALL InsertOutcome('Appreciate the importance of Zakat and fasting', 3, '3.1', '3', @ireId, @grade2Id);

-- IRE Grade 3
CALL InsertOutcome('Describe the human role as Khalifah on earth', 1, '1.1', '1', @ireId, @grade3Id);
CALL InsertOutcome('Identify ways of caring for the environment', 2, '1.1', '1', @ireId, @grade3Id);
CALL InsertOutcome('Appreciate Islamic teachings on environmental stewardship', 3, '1.1', '1', @ireId, @grade3Id);

CALL InsertOutcome('Recite selected Surahs with correct Tajweed', 1, '2.1', '2', @ireId, @grade3Id);
CALL InsertOutcome('Explain the meaning of selected Surahs', 2, '2.1', '2', @ireId, @grade3Id);
CALL InsertOutcome('Apply Quranic teachings in daily life', 3, '2.1', '2', @ireId, @grade3Id);

CALL InsertOutcome('Explain the significance of Ramadan and fasting', 1, '3.1', '3', @ireId, @grade3Id);
CALL InsertOutcome('Describe the rites of Hajj', 2, '3.1', '3', @ireId, @grade3Id);
CALL InsertOutcome('Appreciate the spiritual significance of Hajj', 3, '3.1', '3', @ireId, @grade3Id);

-- ============================================
-- MOVEMENT AND CREATIVE ACTIVITIES Grades 1-3 - Specific Outcomes
-- ============================================

-- Grade 1
CALL InsertOutcome('Walk run jump hop skip gallop and slide', 1, '1.1', '1', @artId, @grade1Id);
CALL InsertOutcome('Combine locomotor skills in simple routines', 2, '1.1', '1', @artId, @grade1Id);
CALL InsertOutcome('Appreciate the importance of physical activity', 3, '1.1', '1', @artId, @grade1Id);

CALL InsertOutcome('Bend stretch twist turn and balance', 1, '1.2', '1', @artId, @grade1Id);
CALL InsertOutcome('Perform non-locomotor movements with a partner', 2, '1.2', '1', @artId, @grade1Id);
CALL InsertOutcome('Enjoy moving in different ways', 3, '1.2', '1', @artId, @grade1Id);

CALL InsertOutcome('Throw a ball underarm and overarm', 1, '1.3', '1', @artId, @grade1Id);
CALL InsertOutcome('Catch a ball with both hands', 2, '1.3', '1', @artId, @grade1Id);
CALL InsertOutcome('Kick a ball with control', 3, '1.3', '1', @artId, @grade1Id);

CALL InsertOutcome('Draw shapes and simple pictures from the environment', 1, '2.1', '2', @artId, @grade1Id);
CALL InsertOutcome('Colour within lines using different materials', 2, '2.1', '2', @artId, @grade1Id);
CALL InsertOutcome('Create patterns using simple shapes', 3, '2.1', '2', @artId, @grade1Id);

CALL InsertOutcome('Sing simple songs with correct rhythm', 1, '2.2', '2', @artId, @grade1Id);
CALL InsertOutcome('Clap and tap rhythms', 2, '2.2', '2', @artId, @grade1Id);
CALL InsertOutcome('Identify some musical instruments by name', 3, '2.2', '2', @artId, @grade1Id);

CALL InsertOutcome('Participate in simple role play', 1, '2.3', '2', @artId, @grade1Id);
CALL InsertOutcome('Act out short stories', 2, '2.3', '2', @artId, @grade1Id);
CALL InsertOutcome('Appreciate drama as a form of communication', 3, '2.3', '2', @artId, @grade1Id);

-- Movement Grade 2
CALL InsertOutcome('Combine locomotor and non-locomotor skills in sequences', 1, '1.1', '1', @artId, @grade2Id);
CALL InsertOutcome('Perform simple gymnastics', 2, '1.1', '1', @artId, @grade2Id);
CALL InsertOutcome('Demonstrate spatial awareness', 3, '1.1', '1', @artId, @grade2Id);

CALL InsertOutcome('Participate in simple team games', 1, '1.2', '1', @artId, @grade2Id);
CALL InsertOutcome('Apply basic rules of simple games', 2, '1.2', '1', @artId, @grade2Id);
CALL InsertOutcome('Appreciate fair play and teamwork', 3, '1.2', '1', @artId, @grade2Id);

CALL InsertOutcome('Create artwork using local materials', 1, '2.1', '2', @artId, @grade2Id);
CALL InsertOutcome('Identify and use primary colours', 2, '2.1', '2', @artId, @grade2Id);
CALL InsertOutcome('Create simple collages and models', 3, '2.1', '2', @artId, @grade2Id);

CALL InsertOutcome('Sing songs in two parts', 1, '2.2', '2', @artId, @grade2Id);
CALL InsertOutcome('Read simple musical notation', 2, '2.2', '2', @artId, @grade2Id);
CALL InsertOutcome('Create simple rhythms using body percussion', 3, '2.2', '2', @artId, @grade2Id);

CALL InsertOutcome('Perform simple traditional dances', 1, '2.3', '2', @artId, @grade2Id);
CALL InsertOutcome('Create simple dance sequences', 2, '2.3', '2', @artId, @grade2Id);
CALL InsertOutcome('Appreciate cultural dances from different communities', 3, '2.3', '2', @artId, @grade2Id);

-- Movement Grade 3
CALL InsertOutcome('Demonstrate basic athletic skills running jumping throwing', 1, '1.1', '1', @artId, @grade3Id);
CALL InsertOutcome('Measure improvement in physical fitness', 2, '1.1', '1', @artId, @grade3Id);
CALL InsertOutcome('Appreciate regular exercise', 3, '1.1', '1', @artId, @grade3Id);

CALL InsertOutcome('Apply rules of modified sports games', 1, '1.2', '1', @artId, @grade3Id);
CALL InsertOutcome('Demonstrate sportsmanship in games', 2, '1.2', '1', @artId, @grade3Id);
CALL InsertOutcome('Organise and lead simple games', 3, '1.2', '1', @artId, @grade3Id);

CALL InsertOutcome('Create functional craft items using local materials', 1, '2.1', '2', @artId, @grade3Id);
CALL InsertOutcome('Use different art techniques painting printing weaving', 2, '2.1', '2', @artId, @grade3Id);
CALL InsertOutcome('Appreciate art from Kenyan communities', 3, '2.1', '2', @artId, @grade3Id);

CALL InsertOutcome('Compose and perform simple songs', 1, '2.2', '2', @artId, @grade3Id);
CALL InsertOutcome('Play simple rhythms on classroom instruments', 2, '2.2', '2', @artId, @grade3Id);
CALL InsertOutcome('Identify features of different musical genres', 3, '2.2', '2', @artId, @grade3Id);

CALL InsertOutcome('Write and perform a short dramatic piece', 1, '2.3', '2', @artId, @grade3Id);
CALL InsertOutcome('Combine dance and drama in a performance', 2, '2.3', '2', @artId, @grade3Id);
CALL InsertOutcome('Appreciate the role of performing arts in culture', 3, '2.3', '2', @artId, @grade3Id);

-- ============================================
-- Drop the helper procedure and add verification
-- ============================================

DROP PROCEDURE IF EXISTS InsertOutcome;

-- ============================================================
-- STEP 6: Summary verification
-- ============================================================
SELECT 'Strands' AS EntityType, COUNT(*) AS Total FROM strands
UNION ALL
SELECT 'SubStrands', COUNT(*) FROM substrands
UNION ALL
SELECT 'SpecificOutcomes', COUNT(*) FROM specificoutcomes;

SELECT s.Name AS Subject, ll.Name AS Grade, COUNT(st.Id) AS StrandCount
FROM strands st
JOIN subjects s ON st.SubjectId = s.Id
JOIN learninglevels ll ON st.LearningLevelId = ll.Id
GROUP BY s.Name, ll.Name
ORDER BY s.Name, ll.Name;

SELECT s.Name AS Subject, ll.Name AS Grade, COUNT(ss.Id) AS SubStrandCount
FROM substrands ss
JOIN subjects s ON ss.SubjectId = s.Id
JOIN learninglevels ll ON ss.LearningLevelId = ll.Id
GROUP BY s.Name, ll.Name
ORDER BY s.Name, ll.Name;

SET SQL_SAFE_UPDATES = 1;


