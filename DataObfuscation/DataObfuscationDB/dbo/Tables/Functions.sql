CREATE TABLE [dbo].[Functions] (
    [ID]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (255) NULL,
    [Names]       VARCHAR (255) NULL,
    [DisplayName] VARCHAR (255) NULL,
    [Description] VARCHAR (255) NULL,
    [AddDate]     DATETIME      DEFAULT (getdate()) NULL,
    [IsDisplay]   BIT           DEFAULT ((0)) NULL,
    [IsDeleted]   BIT           DEFAULT ((0)) NULL
);



