CREATE TABLE [dbo].[TableColumnFunctionMapping] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [TableName]       VARCHAR (255)  NULL,
    [ColumnName]      VARCHAR (255)  NULL,
    [AddnlParamValue] VARCHAR (255)  NULL,
    [ConstantValue]   VARCHAR (2000) NULL,
    [FunctionId]      INT            NULL,
    [AddDate]         DATETIME       DEFAULT (getdate()) NULL,
    [IsDeleted]       BIT            DEFAULT ((0)) NULL,
    [IsObfuscated]    BIT            DEFAULT ((0)) NULL,
    [ObfuscatedDate]  VARCHAR (30)   NULL,
    [ProjectId]       INT            NULL,
    [IsSelected]      INT            NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);



