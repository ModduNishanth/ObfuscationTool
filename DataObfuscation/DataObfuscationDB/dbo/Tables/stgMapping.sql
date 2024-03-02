CREATE TABLE [dbo].[stgMapping] (
    [TableName]  VARCHAR (255) NULL,
    [ColumnName] VARCHAR (255) NULL,
    [ProjectId]  INT           NULL,
    [IsSelected] INT           DEFAULT ((0)) NULL
);

