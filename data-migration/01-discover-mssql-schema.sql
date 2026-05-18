-- =============================================================
-- MSSQL schema discovery for the restored 'swikunda' database.
-- Run this in SSMS or Azure Data Studio against the MSSQL server
-- that holds the restored .bak. It produces three result sets:
--   (1) all tables + row counts
--   (2) all columns with data types and nullability
--   (3) all foreign keys (parent -> referenced table)
-- Copy the output (or export as CSV) and share it back so we can
-- finalise the source -> target column mapping.
-- =============================================================

USE [swikunda];
GO

-- ---------- (1) Tables and row counts -----------------------------------
SELECT
    s.name                AS SchemaName,
    t.name                AS TableName,
    SUM(p.rows)           AS [RowCount]
FROM sys.tables           t
JOIN sys.schemas          s  ON s.schema_id = t.schema_id
JOIN sys.partitions       p  ON p.object_id = t.object_id
WHERE p.index_id IN (0, 1)        -- heap or clustered only (no dup counts)
  AND t.is_ms_shipped = 0
GROUP BY s.name, t.name
ORDER BY s.name, t.name;
GO

-- ---------- (2) Columns ------------------------------------------------
SELECT
    c.TABLE_SCHEMA,
    c.TABLE_NAME,
    c.ORDINAL_POSITION,
    c.COLUMN_NAME,
    c.DATA_TYPE,
    c.CHARACTER_MAXIMUM_LENGTH AS MaxLength,
    c.NUMERIC_PRECISION         AS [Precision],
    c.NUMERIC_SCALE             AS Scale,
    c.IS_NULLABLE,
    c.COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS c
JOIN INFORMATION_SCHEMA.TABLES  t
  ON  t.TABLE_SCHEMA = c.TABLE_SCHEMA
  AND t.TABLE_NAME   = c.TABLE_NAME
WHERE t.TABLE_TYPE = 'BASE TABLE'
ORDER BY c.TABLE_SCHEMA, c.TABLE_NAME, c.ORDINAL_POSITION;
GO

-- ---------- (3) Foreign keys -------------------------------------------
SELECT
    fk.name                                    AS FkName,
    OBJECT_SCHEMA_NAME(fkc.parent_object_id)   AS ParentSchema,
    OBJECT_NAME       (fkc.parent_object_id)   AS ParentTable,
    cp.name                                    AS ParentColumn,
    OBJECT_SCHEMA_NAME(fkc.referenced_object_id) AS RefSchema,
    OBJECT_NAME       (fkc.referenced_object_id) AS RefTable,
    cr.name                                    AS RefColumn
FROM sys.foreign_keys         fk
JOIN sys.foreign_key_columns  fkc ON fkc.constraint_object_id = fk.object_id
JOIN sys.columns              cp  ON cp.object_id = fkc.parent_object_id
                                  AND cp.column_id = fkc.parent_column_id
JOIN sys.columns              cr  ON cr.object_id = fkc.referenced_object_id
                                  AND cr.column_id = fkc.referenced_column_id
ORDER BY ParentTable, FkName;
GO
