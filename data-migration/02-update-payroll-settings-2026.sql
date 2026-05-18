-- =============================================================================
-- Payroll setting updates for 2026 (Kenya)
-- Run AGAINST the MySQL `swikunda` database AFTER the data migration completes.
--
-- Two issues this script fixes:
--   1. PayrollSettings has duplicate rows (EF migration's INSERT IGNORE doesn't
--      enforce uniqueness on `Key` since there is no unique index on it).
--   2. NSSF tier ceilings are the Year 1 (Feb 2024) values - outdated.
--      Current values per NSSF Act 2013 phased schedule (Year 2/3, 2025-2026):
--        Tier I  Ceiling: 8,000  (Lower Earnings Limit)
--        Tier II Ceiling: 72,000 (Upper Earnings Limit)
--      Verify against the latest NSSF Gazette notice before applying if you
--      are on or past Feb 2027 (Year 4+).
--
-- Other statutory items (SHIF 2.75%, AHL 1.5%, Personal Relief 2,400, NSSF
-- rate 6%, PAYE brackets) are unchanged from current legislation.
-- =============================================================================

USE swikunda;

-- ---------------------------------------------------------------------------
-- 1) Dedupe PayrollSettings: keep MIN(Id) per Key, delete the rest.
-- ---------------------------------------------------------------------------
DELETE ps
FROM   PayrollSettings ps
JOIN   (
    SELECT `Key`, MIN(Id) AS KeepId
    FROM   PayrollSettings
    GROUP BY `Key`
) keep ON keep.`Key` = ps.`Key`
WHERE  ps.Id <> keep.KeepId;

-- ---------------------------------------------------------------------------
-- 2) Update NSSF tier ceilings to current 2026 values.
-- ---------------------------------------------------------------------------
UPDATE PayrollSettings
SET    Value         = 8000,
       EffectiveDate = '2026-01-01',
       Modified      = NOW(),
       ModifiedBy    = 'manual-update'
WHERE  `Key` = 'NssfTier1Ceiling';

UPDATE PayrollSettings
SET    Value         = 72000,
       EffectiveDate = '2026-01-01',
       Modified      = NOW(),
       ModifiedBy    = 'manual-update'
WHERE  `Key` = 'NssfTier2Ceiling';

-- ---------------------------------------------------------------------------
-- 3) Bump EffectiveDate on the remaining (already-current) settings to 2026
--    so the active set is clearly tagged with the current year.
-- ---------------------------------------------------------------------------
UPDATE PayrollSettings
SET    EffectiveDate = '2026-01-01',
       Modified      = NOW(),
       ModifiedBy    = 'manual-update'
WHERE  `Key` IN (
    'NssfTier1Rate',
    'NssfTier2Rate',
    'ShifRate',
    'AhlRate',
    'PersonalRelief',
    'InsuranceReliefRate',
    'InsuranceReliefCap',
    'PensionReliefCap'
);

-- ---------------------------------------------------------------------------
-- 4) Dedupe TaxBands too (same migration-replay risk).
-- ---------------------------------------------------------------------------
DELETE tb
FROM   TaxBands tb
JOIN   (
    SELECT LowerLimit, UpperLimit, Rate, EffectiveDate, MIN(Id) AS KeepId
    FROM   TaxBands
    GROUP BY LowerLimit, UpperLimit, Rate, EffectiveDate
) keep
  ON  keep.LowerLimit    = tb.LowerLimit
  AND keep.UpperLimit    = tb.UpperLimit
  AND keep.Rate          = tb.Rate
  AND keep.EffectiveDate = tb.EffectiveDate
WHERE  tb.Id <> keep.KeepId;

-- Bump current PAYE band EffectiveDate to 2026-01-01 (brackets unchanged from Finance Act 2023).
UPDATE TaxBands
SET    EffectiveDate = '2026-01-01',
       Modified      = NOW(),
       ModifiedBy    = 'manual-update'
WHERE  IsActive = 1;

-- ---------------------------------------------------------------------------
-- 5) Verification
-- ---------------------------------------------------------------------------
SELECT 'PayrollSettings count', COUNT(*) AS rows_ FROM PayrollSettings;
SELECT `Key`, Name, Value, Category, EffectiveDate, IsActive
FROM   PayrollSettings
ORDER BY Category, `Key`;

SELECT 'TaxBands count', COUNT(*) AS rows_ FROM TaxBands;
SELECT Description, LowerLimit, UpperLimit, Rate, EffectiveDate, IsActive
FROM   TaxBands
ORDER BY EffectiveDate, LowerLimit;
