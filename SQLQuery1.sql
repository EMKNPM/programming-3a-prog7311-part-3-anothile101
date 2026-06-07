-- Add CreatedAt column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Contracts ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
    PRINT 'CreatedAt column added successfully.';
END
ELSE
BEGIN
    PRINT 'CreatedAt column already exists.';
END

-- Add UpdatedAt column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE Contracts ADD UpdatedAt DATETIME2 NULL;
    PRINT 'UpdatedAt column added successfully.';
END
ELSE
BEGIN
    PRINT 'UpdatedAt column already exists.';
END

-- Verify columns were added
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Contracts' 
AND COLUMN_NAME IN ('CreatedAt', 'UpdatedAt');