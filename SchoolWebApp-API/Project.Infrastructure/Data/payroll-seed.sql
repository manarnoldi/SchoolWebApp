USE schoolwebappdb;
SET SQL_SAFE_UPDATES = 0;

-- ============================================================
-- Payroll Seed Data — Kenyan Statutory Rates (2025/2026)
-- ============================================================

-- Earning Types
INSERT IGNORE INTO EarningTypes (Name, Code, IsTaxable, IsActive, Created) VALUES
('Basic Salary', 'BASIC', 1, 1, NOW()),
('House Allowance', 'HSEALL', 1, 1, NOW()),
('Transport Allowance', 'TRNALL', 1, 1, NOW()),
('Overtime', 'OVRTME', 1, 1, NOW()),
('Leave Allowance', 'LVALL', 1, 1, NOW()),
('Hardship Allowance', 'HRDALL', 1, 1, NOW()),
('Acting Allowance', 'ACTALL', 1, 1, NOW()),
('Responsibility Allowance', 'RSPALL', 1, 1, NOW()),
('Commuter Allowance', 'COMALL', 0, 1, NOW()),
('Airtime Allowance', 'AIRALL', 0, 1, NOW());

-- Deduction Types
INSERT IGNORE INTO DeductionTypes (Name, Code, IsStatutory, IsActive, Created) VALUES
('PAYE', 'PAYE', 1, 1, NOW()),
('NSSF Employee', 'NSSF', 1, 1, NOW()),
('SHIF', 'SHIF', 1, 1, NOW()),
('Affordable Housing Levy', 'AHL', 1, 1, NOW()),
('NSSF Employer', 'NSSFER', 1, 1, NOW()),
('Loan Deduction', 'LOAN', 0, 1, NOW()),
('Salary Advance', 'ADVANCE', 0, 1, NOW()),
('SACCO', 'SACCO', 0, 1, NOW()),
('Union Dues', 'UNION', 0, 1, NOW()),
('Welfare', 'WELFARE', 0, 1, NOW()),
('Helb Loan', 'HELB', 0, 1, NOW()),
('Pension - Voluntary', 'PENSION', 0, 1, NOW());

-- Tax Bands (Kenya 2025 Monthly PAYE Bands)
INSERT IGNORE INTO TaxBands (Description, LowerLimit, UpperLimit, Rate, EffectiveDate, IsActive, Created) VALUES
('Up to KES 24,000', 0, 24000, 10.00, '2025-01-01', 1, NOW()),
('KES 24,001 – 32,333', 24000, 32333, 25.00, '2025-01-01', 1, NOW()),
('KES 32,334 – 500,000', 32333, 500000, 30.00, '2025-01-01', 1, NOW()),
('KES 500,001 – 800,000', 500000, 800000, 32.50, '2025-01-01', 1, NOW()),
('Above KES 800,000', 800000, 99999999, 35.00, '2025-01-01', 1, NOW());

-- Payroll Settings (Kenya 2025)
INSERT IGNORE INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created) VALUES
-- NSSF (Tier I & II — New Rates effective Feb 2024)
('NssfTier1Ceiling', 'NSSF Tier I Ceiling', 7000, 'NSSF', 'Maximum pensionable earnings for Tier I', '2025-01-01', 1, NOW()),
('NssfTier1Rate', 'NSSF Tier I Rate (%)', 6, 'NSSF', 'Employee contribution rate for Tier I', '2025-01-01', 1, NOW()),
('NssfTier2Ceiling', 'NSSF Tier II Ceiling', 36000, 'NSSF', 'Maximum pensionable earnings for Tier II', '2025-01-01', 1, NOW()),
('NssfTier2Rate', 'NSSF Tier II Rate (%)', 6, 'NSSF', 'Employee contribution rate for Tier II', '2025-01-01', 1, NOW()),
-- SHIF (Social Health Insurance Fund — replaced NHIF Oct 2024)
('ShifRate', 'SHIF Rate (%)', 2.75, 'SHIF', 'Percentage of gross pay', '2025-01-01', 1, NOW()),
-- AHL (Affordable Housing Levy)
('AhlRate', 'AHL Rate (%)', 1.5, 'AHL', 'Percentage of gross pay', '2025-01-01', 1, NOW()),
-- Tax Reliefs
('PersonalRelief', 'Personal Relief (Monthly)', 2400, 'Relief', 'Monthly personal tax relief', '2025-01-01', 1, NOW()),
('InsuranceReliefRate', 'Insurance Relief Rate (%)', 15, 'Relief', 'Percentage of SHIF for insurance relief', '2025-01-01', 1, NOW()),
('InsuranceReliefCap', 'Insurance Relief Cap (Monthly)', 5000, 'Relief', 'Maximum monthly insurance relief', '2025-01-01', 1, NOW()),
-- Pension
('PensionReliefCap', 'Pension Relief Cap (Monthly)', 20000, 'Relief', 'Max monthly pension contribution deductible from taxable income', '2025-01-01', 1, NOW());

SET SQL_SAFE_UPDATES = 1;
