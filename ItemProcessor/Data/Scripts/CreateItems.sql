CREATE TABLE dbo.Items
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(120) NOT NULL,
    Weight DECIMAL(18,2) NOT NULL,
    IsProcessed BIT NOT NULL CONSTRAINT DF_Items_IsProcessed DEFAULT(0),
    ParentItemId INT NULL,
    ProcessedAt DATETIME2 NULL,
    CONSTRAINT FK_Items_ParentItem
        FOREIGN KEY (ParentItemId) REFERENCES dbo.Items(Id)
);
GO

INSERT INTO dbo.Items (Name, Weight, IsProcessed, ParentItemId, ProcessedAt)
VALUES
('Parent-Item', 12.50, 1, NULL, SYSUTCDATETIME()),
('Child-Item-A', 6.25, 0, 1, NULL),
('Child-Item-B', 4.10, 0, 1, NULL);
GO
