CREATE VIEW [dbo].[ExcelDataView]
AS
SELECT P.ProjectName, P.DatabaseName, PCM.TableName, PCM.ColumnName, PCM.IsObfuscated, PCM.IsSelected , PCM.ProjectId
FROM dbo.Projects AS P
INNER JOIN dbo.TableColumnFunctionMapping AS PCM ON P.ProjectID = PCM.ProjectId