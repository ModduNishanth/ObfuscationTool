CREATE TABLE [dbo].[KeywordList] (
    [ID]         INT            IDENTITY (1, 1) NOT NULL,
    [Keyword]    NVARCHAR (255) NOT NULL,
    [ProjectId]  INT            NOT NULL,
    [FunctionId] INT            NOT NULL,
    CONSTRAINT [UQ_KeywordList] UNIQUE NONCLUSTERED ([Keyword] ASC, [ProjectId] ASC, [FunctionId] ASC)
);





