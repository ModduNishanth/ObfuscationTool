CREATE TABLE [dbo].[Projects] (
    [ProjectID]          INT           IDENTITY (1, 1) NOT NULL,
    [ProjectName]        VARCHAR (255) NULL,
    [ProjectDescription] VARCHAR (255) NULL,
    [ServerName]         VARCHAR (255) NULL,
    [DatabaseName]       VARCHAR (255) NULL,
    [UserName]           VARCHAR (255) NULL,
    [Password]           VARCHAR (255) NULL,
    [DataType]           VARCHAR (255) NULL,
    [TestConnection]     INT           DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([ProjectID] ASC)
);



