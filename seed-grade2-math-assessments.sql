-- ============================================================
-- Seed Grade 2 Mathematics Term 1 CBC Assessment Data
-- Run this against your schoolwebappdb database
-- ============================================================

USE schoolwebappdb;

-- Step 1: Find required IDs
SET @mathSubjectId = (SELECT Id FROM subjects WHERE Name = 'Mathematics' LIMIT 1);
SET @grade2LevelId = (SELECT Id FROM learninglevels WHERE Name = 'Grade 2' LIMIT 1);
SET @cbeId = (SELECT Id FROM curricula WHERE Name = 'CBE' OR Name = 'CBC' LIMIT 1);
SET @year2025Id = (SELECT Id FROM academicyears WHERE Name = '2025' LIMIT 1);
SET @term1SessionId = (SELECT Id FROM sessions WHERE SessionName LIKE '%Term 1%' AND AcademicYearId = @year2025Id LIMIT 1);
SET @grade2ClassId = (SELECT Id FROM schoolclasses WHERE LearningLevelId = @grade2LevelId AND AcademicYearId = @year2025Id LIMIT 1);
SET @owenStudentId = (SELECT Id FROM person WHERE Upi = '1200' LIMIT 1);
SET @meGradeId = (SELECT Id FROM grades WHERE Abbr LIKE '%ME%' LIMIT 1);
SET @assessmentTypeId = (SELECT Id FROM assessmenttypes LIMIT 1);
SET @staffId = (SELECT Id FROM person WHERE Discriminator = 'StaffDetails' LIMIT 1);
SET @broadOutcomeId = (SELECT Id FROM broadoutcomes LIMIT 1);
SET @generalOutcomeId = (SELECT Id FROM generaloutcomes LIMIT 1);
SET @now = NOW();

-- Verify IDs found
SELECT 'Mathematics Subject ID' as Item, @mathSubjectId as Value
UNION ALL SELECT 'Grade 2 Level ID', @grade2LevelId
UNION ALL SELECT 'CBE Curriculum ID', @cbeId
UNION ALL SELECT 'Year 2025 ID', @year2025Id
UNION ALL SELECT 'Term 1 Session ID', @term1SessionId
UNION ALL SELECT 'Grade 2 Class ID', @grade2ClassId
UNION ALL SELECT 'Owen Student ID', @owenStudentId
UNION ALL SELECT 'ME Grade ID', @meGradeId
UNION ALL SELECT 'Assessment Type ID', @assessmentTypeId
UNION ALL SELECT 'Broad Outcome ID', @broadOutcomeId
UNION ALL SELECT 'General Outcome ID', @generalOutcomeId;

-- ============================================================
-- Step 2: Insert Strands for Grade 2 Mathematics
-- ============================================================

INSERT INTO strands (Name, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified)
SELECT 'Numbers', 'Whole numbers, fractions and decimals', 1, @mathSubjectId, @grade2LevelId, @cbeId, 1, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM strands WHERE Name = 'Numbers' AND SubjectId = @mathSubjectId AND LearningLevelId = @grade2LevelId);

INSERT INTO strands (Name, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified)
SELECT 'Measurement', 'Length, mass, capacity, time and money', 2, @mathSubjectId, @grade2LevelId, @cbeId, 1, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM strands WHERE Name = 'Measurement' AND SubjectId = @mathSubjectId AND LearningLevelId = @grade2LevelId);

INSERT INTO strands (Name, Description, `Rank`, SubjectId, LearningLevelId, CurriculumId, Version, Created, Modified)
SELECT 'Geometry', 'Lines, angles, shapes and spatial sense', 3, @mathSubjectId, @grade2LevelId, @cbeId, 1, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM strands WHERE Name = 'Geometry' AND SubjectId = @mathSubjectId AND LearningLevelId = @grade2LevelId);

-- Get strand IDs
SET @numbersStrandId = (SELECT Id FROM strands WHERE Name = 'Numbers' AND SubjectId = @mathSubjectId AND LearningLevelId = @grade2LevelId LIMIT 1);
SET @measurementStrandId = (SELECT Id FROM strands WHERE Name = 'Measurement' AND SubjectId = @mathSubjectId AND LearningLevelId = @grade2LevelId LIMIT 1);
SET @geometryStrandId = (SELECT Id FROM strands WHERE Name = 'Geometry' AND SubjectId = @mathSubjectId AND LearningLevelId = @grade2LevelId LIMIT 1);

-- ============================================================
-- Step 3: Insert SubStrands under Numbers
-- ============================================================

INSERT INTO substrands (Name, Description, `Rank`, StrandId, CurriculumId, AcademicYearId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Number concept', 'Numbers in symbols, Number games', 1, @numbersStrandId, @cbeId, @year2025Id, @mathSubjectId, @grade2LevelId, 1, 5, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM substrands WHERE Name = 'Number concept' AND StrandId = @numbersStrandId);

INSERT INTO substrands (Name, Description, `Rank`, StrandId, CurriculumId, AcademicYearId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Whole numbers', 'Counting numbers forward and backward, Place value, Missing numbers', 2, @numbersStrandId, @cbeId, @year2025Id, @mathSubjectId, @grade2LevelId, 1, 7, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM substrands WHERE Name = 'Whole numbers' AND StrandId = @numbersStrandId);

INSERT INTO substrands (Name, Description, `Rank`, StrandId, CurriculumId, AcademicYearId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Addition', 'Addition of numbers, Missing numbers', 3, @numbersStrandId, @cbeId, @year2025Id, @mathSubjectId, @grade2LevelId, 1, 6, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM substrands WHERE Name = 'Addition' AND StrandId = @numbersStrandId);

INSERT INTO substrands (Name, Description, `Rank`, StrandId, CurriculumId, AcademicYearId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified)
SELECT 'Subtraction', 'Subtraction of numbers, Missing numbers', 4, @numbersStrandId, @cbeId, @year2025Id, @mathSubjectId, @grade2LevelId, 1, 6, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM substrands WHERE Name = 'Subtraction' AND StrandId = @numbersStrandId);

-- Get substrand IDs
SET @numberConceptSSId = (SELECT Id FROM substrands WHERE Name = 'Number concept' AND StrandId = @numbersStrandId LIMIT 1);
SET @wholeNumbersSSId = (SELECT Id FROM substrands WHERE Name = 'Whole numbers' AND StrandId = @numbersStrandId LIMIT 1);
SET @additionSSId = (SELECT Id FROM substrands WHERE Name = 'Addition' AND StrandId = @numbersStrandId LIMIT 1);
SET @subtractionSSId = (SELECT Id FROM substrands WHERE Name = 'Subtraction' AND StrandId = @numbersStrandId LIMIT 1);

-- ============================================================
-- Step 4: Insert Specific Outcomes (Learning Outcomes)
-- ============================================================

-- 1.1 Number concept specific outcomes
INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Identify numbers 1 to 100 in symbols in different situations', '', 1, @numberConceptSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Identify numbers 1 to 100 in symbols in different situations' AND SubStrandId = @numberConceptSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Represent numbers 1 to 100 using concrete objects from the environment', '', 2, @numberConceptSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Represent numbers 1 to 100 using concrete objects from the environment' AND SubStrandId = @numberConceptSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Play number games using number cards or digital devices', '', 3, @numberConceptSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Play number games using number cards or digital devices' AND SubStrandId = @numberConceptSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Appreciate the use of numbers in real life situations', '', 4, @numberConceptSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Appreciate the use of numbers in real life situations' AND SubStrandId = @numberConceptSSId);

-- 1.2 Whole numbers specific outcomes
INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Count numbers forward up to 100 in different situations', '', 1, @wholeNumbersSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Count numbers forward up to 100 in different situations' AND SubStrandId = @wholeNumbersSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Count numbers backward from number 50', '', 2, @wholeNumbersSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Count numbers backward from number 50' AND SubStrandId = @wholeNumbersSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Read and write numbers 1 to 100 in symbols in different situations', '', 3, @wholeNumbersSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Read and write numbers 1 to 100 in symbols in different situations' AND SubStrandId = @wholeNumbersSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Identify the place value of numbers in ones and tens', '', 4, @wholeNumbersSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Identify the place value of numbers in ones and tens' AND SubStrandId = @wholeNumbersSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Read and write numbers 1 to 20 in words', '', 5, @wholeNumbersSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Read and write numbers 1 to 20 in words' AND SubStrandId = @wholeNumbersSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Work out missing numbers in number patterns up to 100', '', 6, @wholeNumbersSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Work out missing numbers in number patterns up to 100' AND SubStrandId = @wholeNumbersSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Appreciate number patterns in playing number games', '', 7, @wholeNumbersSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Appreciate number patterns in playing number games' AND SubStrandId = @wholeNumbersSSId);

-- 1.3 Addition specific outcomes
INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Add a 2 digit number to a 1 digit number without and with regrouping with sum not exceeding 100', '', 1, @additionSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Add a 2 digit number to a 1 digit number without and with regrouping with sum not exceeding 100' AND SubStrandId = @additionSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Add a 2 digit number to a 2 digit number without and with regrouping, with sum not exceeding 100', '', 2, @additionSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Add a 2 digit number to a 2 digit number without and with regrouping, with sum not exceeding 100' AND SubStrandId = @additionSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Work out missing numbers in patterns involving addition of whole numbers up to 100', '', 3, @additionSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Work out missing numbers in patterns involving addition of whole numbers up to 100' AND SubStrandId = @additionSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Add two multiples of 10 whose sum does not exceed 100', '', 4, @additionSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Add two multiples of 10 whose sum does not exceed 100' AND SubStrandId = @additionSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Appreciate the addition of numbers in real life situations', '', 5, @additionSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Appreciate the addition of numbers in real life situations' AND SubStrandId = @additionSSId);

-- 1.4 Subtraction specific outcomes
INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Subtract a 1 digit number from a 2 digit number without regrouping', '', 1, @subtractionSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Subtract a 1 digit number from a 2 digit number without regrouping' AND SubStrandId = @subtractionSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Subtract a 2 digit number from a 2 digit number without and with regrouping', '', 2, @subtractionSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Subtract a 2 digit number from a 2 digit number without and with regrouping' AND SubStrandId = @subtractionSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Subtract a lower multiple of 10 from a higher multiple of 10', '', 3, @subtractionSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Subtract a lower multiple of 10 from a higher multiple of 10' AND SubStrandId = @subtractionSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Work out missing numbers in patterns involving subtraction up to 100', '', 4, @subtractionSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Work out missing numbers in patterns involving subtraction up to 100' AND SubStrandId = @subtractionSSId);

INSERT INTO specificoutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified)
SELECT 'Appreciate subtraction of numbers in real life situations', '', 5, @subtractionSSId, @broadOutcomeId, @generalOutcomeId, @now, @now
FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM specificoutcomes WHERE Name = 'Appreciate subtraction of numbers in real life situations' AND SubStrandId = @subtractionSSId);

-- ============================================================
-- Step 5: Enter Student Assessments for Owen Kitonga (ME grade)
-- ============================================================

INSERT INTO studentassessments (StudentId, SchoolClassId, SpecificOutcomeId, GradeId, SessionId, AssessmentTypeId, StaffDetailsId, AssessmentDate, Description, Created, Modified)
SELECT @owenStudentId, @grade2ClassId, so.Id, @meGradeId, @term1SessionId, @assessmentTypeId, @staffId, @now, 'Meeting Expectations', @now, @now
FROM specificoutcomes so
WHERE so.SubStrandId IN (@numberConceptSSId, @wholeNumbersSSId, @additionSSId, @subtractionSSId)
AND NOT EXISTS (
    SELECT 1 FROM studentassessments sa
    WHERE sa.StudentId = @owenStudentId
    AND sa.SpecificOutcomeId = so.Id
    AND sa.SessionId = @term1SessionId
);

-- ============================================================
-- Verification
-- ============================================================
SELECT 'Strands' as Item, COUNT(*) as Count FROM strands WHERE SubjectId = @mathSubjectId AND LearningLevelId = @grade2LevelId
UNION ALL
SELECT 'SubStrands', COUNT(*) FROM substrands WHERE StrandId = @numbersStrandId
UNION ALL
SELECT 'Specific Outcomes', COUNT(*) FROM specificoutcomes WHERE SubStrandId IN (@numberConceptSSId, @wholeNumbersSSId, @additionSSId, @subtractionSSId)
UNION ALL
SELECT 'Owen Assessments', COUNT(*) FROM studentassessments WHERE StudentId = @owenStudentId AND SessionId = @term1SessionId AND SpecificOutcomeId IN (SELECT Id FROM specificoutcomes WHERE SubStrandId IN (@numberConceptSSId, @wholeNumbersSSId, @additionSSId, @subtractionSSId));
