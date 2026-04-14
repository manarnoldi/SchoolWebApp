-- Kenya CBC Curriculum Seed Data
-- =================================

-- Fix Competency #5 (should be Digital Literacy)
UPDATE Competencies SET Name = 'Digital Literacy', Description = 'Use digital technology to access, manage, integrate, evaluate, create and communicate information', `Rank` = 5 WHERE Id = 5;

-- =============================================
-- CBC VALUES (8 core values from KICD)
-- =============================================
INSERT INTO `Values` (Name, Description, `Rank`, Created, Modified) VALUES
('Love', 'Showing genuine care, compassion and concern for others', 1, NOW(), NOW()),
('Responsibility', 'Being accountable for one''s actions and decisions', 2, NOW(), NOW()),
('Respect', 'Showing regard for self, others, property and the environment', 3, NOW(), NOW()),
('Unity', 'Working together for the common good regardless of diversity', 4, NOW(), NOW()),
('Peace', 'Promoting harmony, conflict resolution and peaceful coexistence', 5, NOW(), NOW()),
('Patriotism', 'Love and devotion to one''s country and its ideals', 6, NOW(), NOW()),
('Social Justice', 'Promoting fairness, equity and inclusion in society', 7, NOW(), NOW()),
('Integrity', 'Being honest, trustworthy and demonstrating moral uprightness', 8, NOW(), NOW());

-- =============================================
-- VALUE SCORES (CBC rubric for values assessment)
-- =============================================
INSERT INTO ValueScores (Name, Description, `Rank`, Created, Modified) VALUES
('Exceeds Expectations', 'Consistently demonstrates the value beyond what is expected', 1, NOW(), NOW()),
('Meets Expectations', 'Regularly demonstrates the value as expected', 2, NOW(), NOW()),
('Approaches Expectations', 'Sometimes demonstrates the value but needs improvement', 3, NOW(), NOW()),
('Below Expectations', 'Rarely demonstrates the value; needs significant support', 4, NOW(), NOW());

-- =============================================
-- RESPONSIBILITIES (Areas of responsibility in CBC)
-- =============================================
INSERT INTO Responsibilities (Name, Description, `Rank`, Created, Modified) VALUES
('Keeping the Environment Clean', 'Taking care of the school and home environment', 1, NOW(), NOW()),
('Personal Hygiene', 'Maintaining cleanliness and good health habits', 2, NOW(), NOW()),
('Class Duties', 'Fulfilling assigned roles and duties in the classroom', 3, NOW(), NOW()),
('School Leadership', 'Participating in school governance and leadership activities', 4, NOW(), NOW()),
('Peer Mentoring', 'Supporting and guiding fellow learners', 5, NOW(), NOW()),
('Community Service', 'Volunteering and contributing to community welfare', 6, NOW(), NOW());

-- =============================================
-- SOCIAL SKILLS (CBC social skills framework)
-- =============================================
INSERT INTO SocialSkills (Name, Description, `Rank`, Created, Modified) VALUES
('Teamwork', 'Working cooperatively with others towards a common goal', 1, NOW(), NOW()),
('Communication', 'Expressing ideas clearly and listening actively to others', 2, NOW(), NOW()),
('Conflict Resolution', 'Resolving disagreements peacefully and constructively', 3, NOW(), NOW()),
('Empathy', 'Understanding and sharing the feelings of others', 4, NOW(), NOW()),
('Leadership', 'Guiding and motivating others positively', 5, NOW(), NOW()),
('Respect for Diversity', 'Appreciating and valuing individual and cultural differences', 6, NOW(), NOW()),
('Sharing', 'Willingly sharing resources and knowledge with others', 7, NOW(), NOW()),
('Turn Taking', 'Waiting patiently for one''s turn in activities and discussions', 8, NOW(), NOW());

-- =============================================
-- CO-CURRICULUM ACTIVITIES (CBC extra-curricular)
-- =============================================
INSERT INTO CoCurriculumActivities (Name, Description, `Rank`, Created, Modified) VALUES
('Football', 'Association football (soccer)', 1, NOW(), NOW()),
('Netball', 'Netball team sport', 2, NOW(), NOW()),
('Athletics', 'Track and field events', 3, NOW(), NOW()),
('Swimming', 'Swimming activities and competitions', 4, NOW(), NOW()),
('Volleyball', 'Volleyball team sport', 5, NOW(), NOW()),
('Music', 'Music festivals and performances', 6, NOW(), NOW()),
('Drama', 'Drama and theatre performances', 7, NOW(), NOW()),
('Debate', 'Public speaking and debate competitions', 8, NOW(), NOW()),
('Scouting', 'Kenya Scouts Association activities', 9, NOW(), NOW()),
('Girl Guides', 'Girl Guides Association activities', 10, NOW(), NOW()),
('Chess', 'Chess club and tournaments', 11, NOW(), NOW()),
('Art Club', 'Visual arts and crafts activities', 12, NOW(), NOW());

-- =============================================
-- CO-CURRICULUM SCORE TYPES
-- =============================================
INSERT INTO CoCurriculumScoreTypes (Name, Description, `Rank`, Created, Modified) VALUES
('Participation', 'Level of learner participation in the activity', 1, NOW(), NOW()),
('Achievement', 'Level of achievement or performance in the activity', 2, NOW(), NOW()),
('Effort', 'Level of effort and commitment shown', 3, NOW(), NOW());

-- =============================================
-- CO-CURRICULUM SCORES
-- =============================================
INSERT INTO CoCurriculumScores (Name, Description, `Rank`, CoCurriculumScoreTypeId, Created, Modified) VALUES
('Outstanding', 'Exceptional participation/achievement', 1, 1, NOW(), NOW()),
('Good', 'Above average participation/achievement', 2, 1, NOW(), NOW()),
('Average', 'Satisfactory participation/achievement', 3, 1, NOW(), NOW()),
('Needs Improvement', 'Below satisfactory level', 4, 1, NOW(), NOW()),
('Outstanding', 'Exceptional achievement level', 1, 2, NOW(), NOW()),
('Good', 'Above average achievement level', 2, 2, NOW(), NOW()),
('Average', 'Satisfactory achievement level', 3, 2, NOW(), NOW()),
('Needs Improvement', 'Below satisfactory achievement level', 4, 2, NOW(), NOW()),
('Outstanding', 'Exceptional effort and commitment', 1, 3, NOW(), NOW()),
('Good', 'Above average effort', 2, 3, NOW(), NOW()),
('Average', 'Satisfactory effort', 3, 3, NOW(), NOW()),
('Needs Improvement', 'Needs to put in more effort', 4, 3, NOW(), NOW());

-- =============================================
-- COMMUNITY SERVICE ACTIVITIES (CBC CSL)
-- =============================================
INSERT INTO CommunityServiceActivities (Name, Description, `Rank`, Created, Modified) VALUES
('Environmental Conservation', 'Tree planting, clean-up campaigns, waste management', 1, NOW(), NOW()),
('School Beautification', 'Maintaining school gardens, painting, landscaping', 2, NOW(), NOW()),
('Visiting the Elderly', 'Visiting and assisting elderly members of the community', 3, NOW(), NOW()),
('Feeding Programme', 'Participating in community feeding programmes', 4, NOW(), NOW()),
('Health Awareness', 'Participating in health awareness campaigns', 5, NOW(), NOW()),
('Road Safety Campaign', 'Promoting road safety awareness in the community', 6, NOW(), NOW()),
('Library Assistance', 'Helping organize and maintain the school or community library', 7, NOW(), NOW()),
('Peer Tutoring', 'Assisting younger learners with their studies', 8, NOW(), NOW());

-- =============================================
-- EXAM TYPES
-- =============================================
INSERT INTO ExamTypes (Name, Description, `Rank`, Abbreviation, `Internal`, Created, Modified) VALUES
('Mid-Term Exam', 'Mid-term assessment examination', 1, 'MID', 1, NOW(), NOW()),
('End-Term Exam', 'End of term assessment examination', 2, 'END', 1, NOW(), NOW()),
('KPSEA', 'Kenya Primary School Education Assessment (Grade 6)', 3, 'KPSEA', 0, NOW(), NOW()),
('KJSEA', 'Kenya Junior School Education Assessment (Grade 9)', 4, 'KJSEA', 0, NOW(), NOW());

-- =============================================
-- SUB-STRANDS for Mathematics Grade 4 (SubjectId=11, LearningLevelId=6)
-- Using existing Strand IDs for Mathematics
-- =============================================

-- First, let's update existing strands with proper CBC names for Mathematics Grade 4
UPDATE Strands SET Name = 'Numbers', Description = 'Whole numbers, fractions and decimals', CurriculumId = 1 WHERE Id = 20;
UPDATE Strands SET Name = 'Measurement', Description = 'Length, mass, capacity, time and money', CurriculumId = 1 WHERE Id = 21;
UPDATE Strands SET Name = 'Geometry', Description = 'Lines, angles, shapes and spatial sense', CurriculumId = 1 WHERE Id = 22;

-- Sub-strands for Numbers strand (Id=20, Mathematics Grade 4)
INSERT INTO SubStrands (Name, Description, `Rank`, StrandId, CurriculumId, AcademicYearId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified) VALUES
('Whole Numbers', 'Reading, writing and representing whole numbers up to 100,000', 1, 20, 1, NULL, 11, 6, 1, 5, NOW(), NOW()),
('Place Value', 'Understanding place value of digits in numbers up to 100,000', 2, 20, 1, NULL, 11, 6, 1, 4, NOW(), NOW()),
('Addition', 'Adding whole numbers with and without regrouping', 3, 20, 1, NULL, 11, 6, 1, 5, NOW(), NOW()),
('Subtraction', 'Subtracting whole numbers with and without regrouping', 4, 20, 1, NULL, 11, 6, 1, 5, NOW(), NOW()),
('Multiplication', 'Multiplying whole numbers up to 3 digits by 1 digit', 5, 20, 1, NULL, 11, 6, 1, 6, NOW(), NOW()),
('Division', 'Dividing whole numbers up to 3 digits by 1 digit', 6, 20, 1, NULL, 11, 6, 1, 6, NOW(), NOW()),
('Fractions', 'Understanding and working with simple fractions', 7, 20, 1, NULL, 11, 6, 1, 5, NOW(), NOW());

-- Sub-strands for Measurement strand (Id=21)
INSERT INTO SubStrands (Name, Description, `Rank`, StrandId, CurriculumId, AcademicYearId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified) VALUES
('Length', 'Measuring length using standard units (mm, cm, m, km)', 1, 21, 1, NULL, 11, 6, 1, 4, NOW(), NOW()),
('Mass', 'Measuring mass using standard units (g, kg)', 2, 21, 1, NULL, 11, 6, 1, 3, NOW(), NOW()),
('Capacity', 'Measuring capacity using standard units (ml, l)', 3, 21, 1, NULL, 11, 6, 1, 3, NOW(), NOW()),
('Time', 'Telling time, duration and calendar', 4, 21, 1, NULL, 11, 6, 1, 4, NOW(), NOW()),
('Money', 'Kenyan currency, buying and selling, profit and loss', 5, 21, 1, NULL, 11, 6, 1, 3, NOW(), NOW());

-- Sub-strands for Geometry strand (Id=22)
INSERT INTO SubStrands (Name, Description, `Rank`, StrandId, CurriculumId, AcademicYearId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified) VALUES
('Lines and Angles', 'Identifying and drawing different types of lines and angles', 1, 22, 1, NULL, 11, 6, 1, 4, NOW(), NOW()),
('2D Shapes', 'Properties of circles, triangles, rectangles and squares', 2, 22, 1, NULL, 11, 6, 1, 4, NOW(), NOW()),
('3D Shapes', 'Properties of cubes, cuboids, cylinders and spheres', 3, 22, 1, NULL, 11, 6, 1, 3, NOW(), NOW()),
('Symmetry', 'Lines of symmetry in 2D shapes', 4, 22, 1, NULL, 11, 6, 1, 2, NOW(), NOW());

-- =============================================
-- Update English strands with proper CBC names (SubjectId=15, LearningLevelId=4)
-- =============================================
UPDATE Strands SET Name = 'Listening and Speaking', Description = 'Oral communication skills including listening comprehension and speaking', CurriculumId = 1 WHERE Id = 23;
UPDATE Strands SET Name = 'Reading', Description = 'Reading comprehension, fluency and vocabulary development', CurriculumId = 1 WHERE Id = 24;
UPDATE Strands SET Name = 'Writing', Description = 'Writing skills including composition, grammar and handwriting', CurriculumId = 1 WHERE Id = 25;

-- Sub-strands for English Listening & Speaking (Id=23, Grade 4)
INSERT INTO SubStrands (Name, Description, `Rank`, StrandId, CurriculumId, AcademicYearId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified) VALUES
('Pronunciation', 'Correct pronunciation of words and sounds', 1, 23, 1, NULL, 15, 4, 1, 3, NOW(), NOW()),
('Listening Comprehension', 'Understanding spoken messages and stories', 2, 23, 1, NULL, 15, 4, 1, 4, NOW(), NOW()),
('Oral Narratives', 'Telling and retelling stories orally', 3, 23, 1, NULL, 15, 4, 1, 3, NOW(), NOW()),
('Conversation', 'Engaging in meaningful conversations', 4, 23, 1, NULL, 15, 4, 1, 3, NOW(), NOW());

-- Sub-strands for English Reading (Id=24)
INSERT INTO SubStrands (Name, Description, `Rank`, StrandId, CurriculumId, AcademicYearId, SubjectId, LearningLevelId, Version, LessonNo, Created, Modified) VALUES
('Reading Aloud', 'Reading passages fluently and with expression', 1, 24, 1, NULL, 15, 4, 1, 3, NOW(), NOW()),
('Reading Comprehension', 'Understanding and interpreting written texts', 2, 24, 1, NULL, 15, 4, 1, 5, NOW(), NOW()),
('Vocabulary', 'Building and using new vocabulary in context', 3, 24, 1, NULL, 15, 4, 1, 3, NOW(), NOW()),
('Reading for Pleasure', 'Developing a love for reading through diverse texts', 4, 24, 1, NULL, 15, 4, 1, 2, NOW(), NOW());

-- =============================================
-- SPECIFIC OUTCOMES (for Mathematics Grade 4 - Numbers strand, Whole Numbers sub-strand)
-- We need SubStrand IDs - they were just inserted above
-- =============================================

-- Get the first SubStrand ID for Whole Numbers (it'll be the first inserted)
SET @wholeNumbersId = (SELECT Id FROM SubStrands WHERE Name = 'Whole Numbers' AND StrandId = 20 LIMIT 1);
SET @placeValueId = (SELECT Id FROM SubStrands WHERE Name = 'Place Value' AND StrandId = 20 LIMIT 1);
SET @additionId = (SELECT Id FROM SubStrands WHERE Name = 'Addition' AND StrandId = 20 LIMIT 1);

-- Use existing BroadOutcomeId=5 (Mathematics Lower Primary) and GeneralOutcomeId=1 (Early Years)
INSERT INTO SpecificOutcomes (Name, Description, `Rank`, SubStrandId, BroadOutcomeId, GeneralOutcomeId, Created, Modified) VALUES
('Read and write numbers up to 100,000 in symbols and words', 'Learner should read, write and represent whole numbers up to 100,000', 1, @wholeNumbersId, 5, 11, NOW(), NOW()),
('Order and compare numbers up to 100,000', 'Learner should arrange numbers in ascending and descending order', 2, @wholeNumbersId, 5, 11, NOW(), NOW()),
('Round off numbers to the nearest 10, 100 and 1,000', 'Learner should round off whole numbers to specified place values', 3, @wholeNumbersId, 5, 11, NOW(), NOW()),
('Identify place value of digits in numbers up to 100,000', 'Learner should state the place value of each digit', 1, @placeValueId, 5, 11, NOW(), NOW()),
('Expand numbers up to 100,000 in terms of place value', 'Learner should write numbers in expanded form', 2, @placeValueId, 5, 11, NOW(), NOW()),
('Add up to 4-digit numbers with regrouping', 'Learner should add whole numbers with carrying', 1, @additionId, 5, 11, NOW(), NOW()),
('Solve word problems involving addition', 'Learner should apply addition skills to real-life situations', 2, @additionId, 5, 11, NOW(), NOW());

-- =============================================
-- KEY QUESTIONS for Whole Numbers sub-strand
-- =============================================
INSERT INTO KeyQuestion (Name, Description, `Rank`, SubStrandId, Created, Modified) VALUES
('How do we read and write large numbers?', 'Inquiry question for whole numbers reading/writing', 1, @wholeNumbersId, NOW(), NOW()),
('Why is it important to compare and order numbers?', 'Inquiry question for ordering numbers', 2, @wholeNumbersId, NOW(), NOW()),
('Where do we use large numbers in daily life?', 'Connecting numbers to real-life contexts', 3, @wholeNumbersId, NOW(), NOW());

-- =============================================
-- LEARNING EXPERIENCES for Whole Numbers sub-strand
-- =============================================
INSERT INTO LearningExperience (Name, Description, `Rank`, SubStrandId, Created, Modified) VALUES
('Counting and grouping objects in thousands', 'Use bundling sticks, bottle tops to count in groups', 1, @wholeNumbersId, NOW(), NOW()),
('Reading number charts and number lines', 'Use number charts to read and write numbers', 2, @wholeNumbersId, NOW(), NOW()),
('Playing number games', 'Use board games and digital apps with large numbers', 3, @wholeNumbersId, NOW(), NOW()),
('Market role play', 'Simulate buying and selling using large numbers', 4, @wholeNumbersId, NOW(), NOW());

-- =============================================
-- PCIs (Pertinent and Contemporary Issues) for Whole Numbers
-- =============================================
INSERT INTO PCI (Name, Description, `Rank`, SubStrandId, Created, Modified) VALUES
('Financial Literacy', 'Understanding money, budgeting and saving through number work', 1, @wholeNumbersId, NOW(), NOW()),
('Environmental Awareness', 'Using numbers to understand environmental data and conservation', 2, @wholeNumbersId, NOW(), NOW()),
('Health Education', 'Applying numbers in health contexts like counting medicine doses', 3, @wholeNumbersId, NOW(), NOW());

-- =============================================
-- LESSON ALLOCATIONS per subject per level (KICD recommended)
-- =============================================
INSERT INTO LessonAllocation (LessonsPerWeek, Description, SubjectId, LearningLevelId, Created, Modified) VALUES
-- Grade 4 (LearningLevelId=6)
(5, 'Mathematics - 5 lessons per week', 11, 6, NOW(), NOW()),
(5, 'English - 5 lessons per week', 15, 6, NOW(), NOW()),
(4, 'Kiswahili - 4 lessons per week', 9, 6, NOW(), NOW()),
(3, 'Science & Technology - 3 lessons per week', 16, 6, NOW(), NOW()),
(3, 'Social Studies - 3 lessons per week', 18, 6, NOW(), NOW()),
(3, 'Agriculture & Nutrition - 3 lessons per week', 17, 6, NOW(), NOW()),
(2, 'Creative Arts - 2 lessons per week', 19, 6, NOW(), NOW()),
(2, 'Christian Religious Education - 2 lessons per week', 12, 6, NOW(), NOW()),
-- Grade 5 (LearningLevelId=7)
(5, 'Mathematics - 5 lessons per week', 11, 7, NOW(), NOW()),
(5, 'English - 5 lessons per week', 15, 7, NOW(), NOW()),
(4, 'Kiswahili - 4 lessons per week', 9, 7, NOW(), NOW()),
(3, 'Science & Technology - 3 lessons per week', 16, 7, NOW(), NOW()),
(3, 'Social Studies - 3 lessons per week', 18, 7, NOW(), NOW()),
(3, 'Agriculture & Nutrition - 3 lessons per week', 17, 7, NOW(), NOW()),
(2, 'Creative Arts - 2 lessons per week', 19, 7, NOW(), NOW()),
(2, 'Christian Religious Education - 2 lessons per week', 12, 7, NOW(), NOW());
