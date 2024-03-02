CREATE PROCEDURE [dbo].[spCopyProjectMapping]
  @ProjectId VARCHAR(MAX),
  @ProjectIdFrom INT
AS
BEGIN
  INSERT INTO TableColumnFunctionMapping
  SELECT
    TableName,
    ColumnName,
    AddnlParamValue,
    ConstantValue,
    FunctionId,
    GETDATE(),
    IsDeleted,
    0 AS IsObfuscated,
    ' ' AS ObfuscatedDate,
    @ProjectIdFrom as ProjectId, -- Set the destination ProjectId
    IsSelected
  FROM
    TableColumnFunctionMapping
  WHERE
    ProjectId IN (SELECT value FROM STRING_SPLIT(@ProjectId, ','))
    AND IsDeleted = 0;
END