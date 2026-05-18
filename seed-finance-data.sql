USE schoolwebappdb;
SET SQL_SAFE_UPDATES = 0;

-- ===============================================================
-- Finance Module Seed Data
-- Run this AFTER running the EF Core migration that creates the
-- Finance tables (Accounts, FeeCategories, FeeStructures,
-- FeeStructureItems, StudentInvoices, StudentInvoiceItems,
-- Payments, ExpenseCategories, Expenses, JournalEntries, JournalLines,
-- Budgets, BudgetLines)
--
-- This script is IDEMPOTENT: running it multiple times will not
-- create duplicates. It cleans up existing duplicates first.
-- ===============================================================

-- ----------------------------------------------------------------
-- CLEANUP: Remove duplicate accounts (keeps the record with lowest Id per Code)
-- Also clears dependent rows first so FKs don't block cleanup.
-- ----------------------------------------------------------------
SET FOREIGN_KEY_CHECKS = 0;

-- Remove budget lines pointing to duplicate accounts (they'll be re-seeded)
DELETE bl FROM `BudgetLines` bl
INNER JOIN `Accounts` a ON bl.AccountId = a.Id
INNER JOIN (
    SELECT Code, MIN(Id) AS KeepId FROM `Accounts` GROUP BY Code HAVING COUNT(*) > 1
) dup ON a.Code = dup.Code AND a.Id <> dup.KeepId;

-- Remove journal lines that reference duplicate accounts
DELETE jl FROM `JournalLines` jl
INNER JOIN `Accounts` a ON jl.AccountId = a.Id
INNER JOIN (
    SELECT Code, MIN(Id) AS KeepId FROM `Accounts` GROUP BY Code HAVING COUNT(*) > 1
) dup ON a.Code = dup.Code AND a.Id <> dup.KeepId;

-- Null out FK references on FeeCategories / ExpenseCategories / Expenses / Payments
UPDATE `FeeCategories` fc
INNER JOIN `Accounts` a ON fc.IncomeAccountId = a.Id
INNER JOIN (
    SELECT Code, MIN(Id) AS KeepId FROM `Accounts` GROUP BY Code HAVING COUNT(*) > 1
) dup ON a.Code = dup.Code AND a.Id <> dup.KeepId
SET fc.IncomeAccountId = dup.KeepId;

UPDATE `ExpenseCategories` ec
INNER JOIN `Accounts` a ON ec.ExpenseAccountId = a.Id
INNER JOIN (
    SELECT Code, MIN(Id) AS KeepId FROM `Accounts` GROUP BY Code HAVING COUNT(*) > 1
) dup ON a.Code = dup.Code AND a.Id <> dup.KeepId
SET ec.ExpenseAccountId = dup.KeepId;

UPDATE `Expenses` e
INNER JOIN `Accounts` a ON e.PaidFromAccountId = a.Id
INNER JOIN (
    SELECT Code, MIN(Id) AS KeepId FROM `Accounts` GROUP BY Code HAVING COUNT(*) > 1
) dup ON a.Code = dup.Code AND a.Id <> dup.KeepId
SET e.PaidFromAccountId = dup.KeepId;

UPDATE `Payments` p
INNER JOIN `Accounts` a ON p.BankAccountId = a.Id
INNER JOIN (
    SELECT Code, MIN(Id) AS KeepId FROM `Accounts` GROUP BY Code HAVING COUNT(*) > 1
) dup ON a.Code = dup.Code AND a.Id <> dup.KeepId
SET p.BankAccountId = dup.KeepId;

-- Parent account references: remap to the kept duplicate
UPDATE `Accounts` a
INNER JOIN `Accounts` pa ON a.ParentAccountId = pa.Id
INNER JOIN (
    SELECT Code, MIN(Id) AS KeepId FROM `Accounts` GROUP BY Code HAVING COUNT(*) > 1
) dup ON pa.Code = dup.Code AND pa.Id <> dup.KeepId
SET a.ParentAccountId = dup.KeepId;

-- Now delete the duplicate Accounts (keeping lowest Id per Code)
DELETE a1 FROM `Accounts` a1
INNER JOIN `Accounts` a2 ON a1.Code = a2.Code AND a1.Id > a2.Id;

-- Clean up duplicate FeeCategories, ExpenseCategories by Name
DELETE fc1 FROM `FeeCategories` fc1
INNER JOIN `FeeCategories` fc2 ON fc1.Name = fc2.Name AND fc1.Id > fc2.Id;

DELETE ec1 FROM `ExpenseCategories` ec1
INNER JOIN `ExpenseCategories` ec2 ON ec1.Name = ec2.Name AND ec1.Id > ec2.Id;

-- Clean up duplicate Budgets by Name (cascading lines)
DELETE bl FROM `BudgetLines` bl
INNER JOIN `Budgets` b ON bl.BudgetId = b.Id
INNER JOIN (
    SELECT Name, MIN(Id) AS KeepId FROM `Budgets` GROUP BY Name HAVING COUNT(*) > 1
) dup ON b.Name = dup.Name AND b.Id <> dup.KeepId;

DELETE b1 FROM `Budgets` b1
INNER JOIN `Budgets` b2 ON b1.Name = b2.Name AND b1.Id > b2.Id;

SET FOREIGN_KEY_CHECKS = 1;

-- Ensure Accounts.Code has a unique index so INSERT IGNORE actually skips duplicates
SET @idx_exists := (SELECT COUNT(*) FROM information_schema.statistics
    WHERE table_schema = DATABASE() AND table_name = 'Accounts' AND index_name = 'UK_Accounts_Code');
SET @sql := IF(@idx_exists = 0,
    'ALTER TABLE `Accounts` ADD UNIQUE KEY `UK_Accounts_Code` (`Code`)',
    'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Unique index on FeeCategories.Name
SET @idx_exists := (SELECT COUNT(*) FROM information_schema.statistics
    WHERE table_schema = DATABASE() AND table_name = 'FeeCategories' AND index_name = 'UK_FeeCategories_Name');
SET @sql := IF(@idx_exists = 0,
    'ALTER TABLE `FeeCategories` ADD UNIQUE KEY `UK_FeeCategories_Name` (`Name`)', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Unique index on ExpenseCategories.Name
SET @idx_exists := (SELECT COUNT(*) FROM information_schema.statistics
    WHERE table_schema = DATABASE() AND table_name = 'ExpenseCategories' AND index_name = 'UK_ExpenseCategories_Name');
SET @sql := IF(@idx_exists = 0,
    'ALTER TABLE `ExpenseCategories` ADD UNIQUE KEY `UK_ExpenseCategories_Name` (`Name`)', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Unique index on Budgets.Name
SET @idx_exists := (SELECT COUNT(*) FROM information_schema.statistics
    WHERE table_schema = DATABASE() AND table_name = 'Budgets' AND index_name = 'UK_Budgets_Name');
SET @sql := IF(@idx_exists = 0,
    'ALTER TABLE `Budgets` ADD UNIQUE KEY `UK_Budgets_Name` (`Name`)', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Unique index on JournalEntries.ReferenceNumber
SET @idx_exists := (SELECT COUNT(*) FROM information_schema.statistics
    WHERE table_schema = DATABASE() AND table_name = 'JournalEntries' AND index_name = 'UK_JournalEntries_Ref');
SET @sql := IF(@idx_exists = 0,
    'ALTER TABLE `JournalEntries` ADD UNIQUE KEY `UK_JournalEntries_Ref` (`ReferenceNumber`)', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ----------------------------------------------------------------
-- CHART OF ACCOUNTS (skipped if Code already exists)
-- AccountType: 1=Asset, 2=Liability, 3=Equity, 4=Income, 5=Expense
-- ----------------------------------------------------------------
INSERT IGNORE INTO `Accounts` (`Code`, `Name`, `AccountType`, `Description`, `IsActive`, `Created`, `Modified`) VALUES
-- Assets (1000-1999)
('1000', 'Cash on Hand', 1, 'Petty cash and cash at hand', 1, NOW(), NOW()),
('1100', 'Bank - Main Account', 1, 'Primary bank account', 1, NOW(), NOW()),
('1110', 'Bank - Mpesa', 1, 'M-Pesa paybill/till balance', 1, NOW(), NOW()),
('1200', 'Accounts Receivable (Fees)', 1, 'Outstanding student fees owed', 1, NOW(), NOW()),
('1300', 'Inventory - Uniforms & Books', 1, 'School store inventory', 1, NOW(), NOW()),
('1500', 'Furniture & Fittings', 1, 'Office and classroom furniture', 1, NOW(), NOW()),
('1510', 'IT Equipment', 1, 'Computers and peripherals', 1, NOW(), NOW()),
('1520', 'School Building', 1, 'Buildings and land', 1, NOW(), NOW()),

-- Liabilities (2000-2999)
('2000', 'Accounts Payable', 2, 'Amounts owed to suppliers', 1, NOW(), NOW()),
('2100', 'Accrued Salaries', 2, 'Salaries owed but not yet paid', 1, NOW(), NOW()),
('2200', 'PAYE Payable', 2, 'Pay As You Earn tax due to KRA', 1, NOW(), NOW()),
('2210', 'NHIF Payable', 2, 'NHIF deductions due', 1, NOW(), NOW()),
('2220', 'NSSF Payable', 2, 'NSSF deductions due', 1, NOW(), NOW()),
('2300', 'Deferred Revenue (Fees in Advance)', 2, 'Fees paid in advance of service', 1, NOW(), NOW()),

-- Equity (3000-3999)
('3000', 'Owner''s Equity', 3, 'School owner capital', 1, NOW(), NOW()),
('3100', 'Retained Earnings', 3, 'Accumulated surplus from prior years', 1, NOW(), NOW()),

-- Income (4000-4999)
('4000', 'Tuition Fee Income', 4, 'Fees for academic tuition', 1, NOW(), NOW()),
('4010', 'Transport Fee Income', 4, 'Fees for school transport', 1, NOW(), NOW()),
('4020', 'Meals Fee Income', 4, 'Fees for school meals', 1, NOW(), NOW()),
('4030', 'Boarding Fee Income', 4, 'Boarding fees', 1, NOW(), NOW()),
('4040', 'Activity Fee Income', 4, 'Fees for co-curricular activities', 1, NOW(), NOW()),
('4050', 'Uniform Sales', 4, 'Revenue from uniform sales', 1, NOW(), NOW()),
('4900', 'Donations & Grants', 4, 'Income from donors and grants', 1, NOW(), NOW()),

-- Expenses (5000-5999)
('5000', 'Salaries - Teaching Staff', 5, 'Teacher salaries', 1, NOW(), NOW()),
('5010', 'Salaries - Non-Teaching Staff', 5, 'Support staff salaries', 1, NOW(), NOW()),
('5100', 'Utilities - Electricity', 5, 'KPLC bills', 1, NOW(), NOW()),
('5110', 'Utilities - Water', 5, 'Water bills', 1, NOW(), NOW()),
('5120', 'Utilities - Internet & Telephone', 5, 'Internet, airtime and telephone', 1, NOW(), NOW()),
('5200', 'Teaching Supplies & Stationery', 5, 'Books, stationery, lab supplies', 1, NOW(), NOW()),
('5210', 'Cleaning & Sanitation', 5, 'Detergents, soap, toilet paper', 1, NOW(), NOW()),
('5220', 'Repairs & Maintenance', 5, 'Building, vehicle, equipment repairs', 1, NOW(), NOW()),
('5300', 'Transport & Fuel', 5, 'Bus fuel and vehicle costs', 1, NOW(), NOW()),
('5400', 'Food & Catering', 5, 'Food supplies for students/staff', 1, NOW(), NOW()),
('5500', 'Marketing & Advertising', 5, 'Publicity expenses', 1, NOW(), NOW()),
('5900', 'Miscellaneous Expenses', 5, 'Other general expenses', 1, NOW(), NOW());

-- ----------------------------------------------------------------
-- FEE CATEGORIES
-- Link each to the matching Income account (resolved by Code)
-- ----------------------------------------------------------------
INSERT IGNORE INTO `FeeCategories` (`Name`, `Description`, `Rank`, `IncomeAccountId`, `IsActive`, `Created`, `Modified`) VALUES
('Tuition',  'School fees for academic tuition', 1, (SELECT Id FROM Accounts WHERE Code='4000' LIMIT 1), 1, NOW(), NOW()),
('Transport','School bus transport fee',         2, (SELECT Id FROM Accounts WHERE Code='4010' LIMIT 1), 1, NOW(), NOW()),
('Meals',    'Lunch programme fee',              3, (SELECT Id FROM Accounts WHERE Code='4020' LIMIT 1), 1, NOW(), NOW()),
('Boarding', 'Boarding accommodation fee',       4, (SELECT Id FROM Accounts WHERE Code='4030' LIMIT 1), 1, NOW(), NOW()),
('Activity', 'Co-curricular activity fee',       5, (SELECT Id FROM Accounts WHERE Code='4040' LIMIT 1), 1, NOW(), NOW()),
('Uniform',  'Uniform and PE kit',               6, (SELECT Id FROM Accounts WHERE Code='4050' LIMIT 1), 1, NOW(), NOW()),
('Exam',     'Examination fee',                  7, (SELECT Id FROM Accounts WHERE Code='4000' LIMIT 1), 1, NOW(), NOW());

-- ----------------------------------------------------------------
-- EXPENSE CATEGORIES
-- ----------------------------------------------------------------
INSERT IGNORE INTO `ExpenseCategories` (`Name`, `Description`, `Rank`, `ExpenseAccountId`, `IsActive`, `Created`, `Modified`) VALUES
('Teacher Salaries',       'Teaching staff payroll',      1, (SELECT Id FROM Accounts WHERE Code='5000' LIMIT 1), 1, NOW(), NOW()),
('Non-Teaching Salaries',  'Support staff payroll',        2, (SELECT Id FROM Accounts WHERE Code='5010' LIMIT 1), 1, NOW(), NOW()),
('Electricity',            'Power bills (KPLC)',           3, (SELECT Id FROM Accounts WHERE Code='5100' LIMIT 1), 1, NOW(), NOW()),
('Water',                  'Water bills',                  4, (SELECT Id FROM Accounts WHERE Code='5110' LIMIT 1), 1, NOW(), NOW()),
('Internet & Phones',      'Internet and telephone bills', 5, (SELECT Id FROM Accounts WHERE Code='5120' LIMIT 1), 1, NOW(), NOW()),
('Stationery',             'Books and stationery',         6, (SELECT Id FROM Accounts WHERE Code='5200' LIMIT 1), 1, NOW(), NOW()),
('Cleaning Supplies',      'Sanitation supplies',          7, (SELECT Id FROM Accounts WHERE Code='5210' LIMIT 1), 1, NOW(), NOW()),
('Repairs',                'Building/equipment repairs',    8, (SELECT Id FROM Accounts WHERE Code='5220' LIMIT 1), 1, NOW(), NOW()),
('Fuel & Transport',       'Bus fuel and logistics',       9, (SELECT Id FROM Accounts WHERE Code='5300' LIMIT 1), 1, NOW(), NOW()),
('Food Supplies',          'Catering purchases',          10, (SELECT Id FROM Accounts WHERE Code='5400' LIMIT 1), 1, NOW(), NOW()),
('Miscellaneous',          'Other expenses',              99, (SELECT Id FROM Accounts WHERE Code='5900' LIMIT 1), 1, NOW(), NOW());

-- ----------------------------------------------------------------
-- SAMPLE JOURNAL ENTRIES (opening balances)
-- Entry 1: Record opening cash and bank balances against Owner's Equity
-- ----------------------------------------------------------------
INSERT IGNORE INTO `JournalEntries` (`ReferenceNumber`, `EntryDate`, `Description`, `IsPosted`, `Created`, `Modified`) VALUES
('JV-OPEN-001', '2025-01-01', 'Opening balances for financial year', 1, NOW(), NOW());

-- Look up by ReferenceNumber (works whether INSERT happened or was skipped)
SET @je1 := (SELECT Id FROM `JournalEntries` WHERE ReferenceNumber='JV-OPEN-001' LIMIT 1);

-- Clear any existing lines for this entry, then insert fresh lines (safe for re-runs)
DELETE FROM `JournalLines` WHERE JournalEntryId = @je1;
INSERT INTO `JournalLines` (`JournalEntryId`, `AccountId`, `Debit`, `Credit`, `Description`, `Created`, `Modified`) VALUES
(@je1, (SELECT Id FROM Accounts WHERE Code='1000' LIMIT 1),  50000,      0, 'Opening cash on hand',       NOW(), NOW()),
(@je1, (SELECT Id FROM Accounts WHERE Code='1100' LIMIT 1), 500000,      0, 'Opening bank balance',       NOW(), NOW()),
(@je1, (SELECT Id FROM Accounts WHERE Code='1520' LIMIT 1),2000000,      0, 'School building at cost',    NOW(), NOW()),
(@je1, (SELECT Id FROM Accounts WHERE Code='1500' LIMIT 1), 300000,      0, 'Furniture and fittings',     NOW(), NOW()),
(@je1, (SELECT Id FROM Accounts WHERE Code='3000' LIMIT 1),      0,2850000,'Owner''s equity contribution',NOW(), NOW());

-- ----------------------------------------------------------------
-- SAMPLE JOURNAL ENTRY: Record a term of tuition income + cash receipt
-- ----------------------------------------------------------------
INSERT IGNORE INTO `JournalEntries` (`ReferenceNumber`, `EntryDate`, `Description`, `IsPosted`, `Created`, `Modified`) VALUES
('JV-2025-001', '2025-01-15', 'Tuition fees collected - Term 1 sample', 1, NOW(), NOW());

SET @je2 := (SELECT Id FROM `JournalEntries` WHERE ReferenceNumber='JV-2025-001' LIMIT 1);

DELETE FROM `JournalLines` WHERE JournalEntryId = @je2;
INSERT INTO `JournalLines` (`JournalEntryId`, `AccountId`, `Debit`, `Credit`, `Description`, `Created`, `Modified`) VALUES
(@je2, (SELECT Id FROM Accounts WHERE Code='1100' LIMIT 1), 150000,      0, 'Fees deposited to bank', NOW(), NOW()),
(@je2, (SELECT Id FROM Accounts WHERE Code='4000' LIMIT 1),      0, 150000, 'Tuition fees income',   NOW(), NOW());

-- ----------------------------------------------------------------
-- SAMPLE JOURNAL ENTRY: Pay teacher salaries
-- ----------------------------------------------------------------
INSERT IGNORE INTO `JournalEntries` (`ReferenceNumber`, `EntryDate`, `Description`, `IsPosted`, `Created`, `Modified`) VALUES
('JV-2025-002', '2025-01-31', 'January 2025 teacher salaries', 1, NOW(), NOW());

SET @je3 := (SELECT Id FROM `JournalEntries` WHERE ReferenceNumber='JV-2025-002' LIMIT 1);

DELETE FROM `JournalLines` WHERE JournalEntryId = @je3;
INSERT INTO `JournalLines` (`JournalEntryId`, `AccountId`, `Debit`, `Credit`, `Description`, `Created`, `Modified`) VALUES
(@je3, (SELECT Id FROM Accounts WHERE Code='5000' LIMIT 1), 80000,      0, 'Salary expense',         NOW(), NOW()),
(@je3, (SELECT Id FROM Accounts WHERE Code='1100' LIMIT 1),     0, 80000, 'Bank payment',           NOW(), NOW());

-- ----------------------------------------------------------------
-- SAMPLE JOURNAL ENTRY: Pay utility bills
-- ----------------------------------------------------------------
INSERT IGNORE INTO `JournalEntries` (`ReferenceNumber`, `EntryDate`, `Description`, `IsPosted`, `Created`, `Modified`) VALUES
('JV-2025-003', '2025-02-05', 'Electricity and water bills for January', 1, NOW(), NOW());

SET @je4 := (SELECT Id FROM `JournalEntries` WHERE ReferenceNumber='JV-2025-003' LIMIT 1);

DELETE FROM `JournalLines` WHERE JournalEntryId = @je4;
INSERT INTO `JournalLines` (`JournalEntryId`, `AccountId`, `Debit`, `Credit`, `Description`, `Created`, `Modified`) VALUES
(@je4, (SELECT Id FROM Accounts WHERE Code='5100' LIMIT 1), 12000,     0, 'KPLC bill',              NOW(), NOW()),
(@je4, (SELECT Id FROM Accounts WHERE Code='5110' LIMIT 1),  4500,     0, 'Water bill',             NOW(), NOW()),
(@je4, (SELECT Id FROM Accounts WHERE Code='1100' LIMIT 1),     0, 16500, 'Bank payment',           NOW(), NOW());

-- ================================================================
-- SAMPLE BUDGETS
-- ================================================================
INSERT IGNORE INTO `Budgets` (`Name`, `Description`, `StartDate`, `EndDate`, `AcademicYearId`, `IsActive`, `Created`, `Modified`) VALUES
('Annual Operating Budget 2025', 'Operating budget for the 2025 academic year covering income & expenses', '2025-01-01', '2025-12-31',
    (SELECT Id FROM AcademicYears WHERE `Status` = 1 LIMIT 1), 1, NOW(), NOW());

SET @bdg1 := (SELECT Id FROM `Budgets` WHERE Name='Annual Operating Budget 2025' LIMIT 1);

DELETE FROM `BudgetLines` WHERE BudgetId = @bdg1;
INSERT INTO `BudgetLines` (`BudgetId`, `AccountId`, `BudgetedAmount`, `Notes`, `Created`, `Modified`) VALUES
-- Income lines
(@bdg1, (SELECT Id FROM Accounts WHERE Code='4000' LIMIT 1), 12000000, 'Tuition fees target',              NOW(), NOW()),
(@bdg1, (SELECT Id FROM Accounts WHERE Code='4010' LIMIT 1),   800000, 'Transport fees target',            NOW(), NOW()),
(@bdg1, (SELECT Id FROM Accounts WHERE Code='4020' LIMIT 1),   600000, 'Meals fee target',                 NOW(), NOW()),
(@bdg1, (SELECT Id FROM Accounts WHERE Code='4040' LIMIT 1),   400000, 'Activity fees',                    NOW(), NOW()),
-- Expense lines
(@bdg1, (SELECT Id FROM Accounts WHERE Code='5000' LIMIT 1),  6000000, 'Annual salaries budget',           NOW(), NOW()),
(@bdg1, (SELECT Id FROM Accounts WHERE Code='5100' LIMIT 1),   180000, 'Electricity for the year',         NOW(), NOW()),
(@bdg1, (SELECT Id FROM Accounts WHERE Code='5110' LIMIT 1),    60000, 'Water bills for the year',         NOW(), NOW());

-- ----------------------------------------------------------------
-- SAMPLE BUDGET: Term 1 2025
-- ----------------------------------------------------------------
INSERT IGNORE INTO `Budgets` (`Name`, `Description`, `StartDate`, `EndDate`, `AcademicYearId`, `IsActive`, `Created`, `Modified`) VALUES
('Term 1 2025 Budget', 'Term-specific budget for January-April 2025', '2025-01-01', '2025-04-30',
    (SELECT Id FROM AcademicYears WHERE `Status` = 1 LIMIT 1), 1, NOW(), NOW());

SET @bdg2 := (SELECT Id FROM `Budgets` WHERE Name='Term 1 2025 Budget' LIMIT 1);

DELETE FROM `BudgetLines` WHERE BudgetId = @bdg2;
INSERT INTO `BudgetLines` (`BudgetId`, `AccountId`, `BudgetedAmount`, `Notes`, `Created`, `Modified`) VALUES
(@bdg2, (SELECT Id FROM Accounts WHERE Code='4000' LIMIT 1), 4000000, 'Term 1 tuition target',           NOW(), NOW()),
(@bdg2, (SELECT Id FROM Accounts WHERE Code='4010' LIMIT 1),  280000, 'Term 1 transport',                NOW(), NOW()),
(@bdg2, (SELECT Id FROM Accounts WHERE Code='5000' LIMIT 1), 2000000, 'Q1 salaries',                     NOW(), NOW()),
(@bdg2, (SELECT Id FROM Accounts WHERE Code='5100' LIMIT 1),   45000, 'Q1 electricity',                  NOW(), NOW()),
(@bdg2, (SELECT Id FROM Accounts WHERE Code='5110' LIMIT 1),   15000, 'Q1 water',                        NOW(), NOW());

-- End of seed data
SELECT 'Finance seed data loaded successfully.' AS Status;

