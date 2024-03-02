CREATE PROCEDURE [dbo].[spGetMapping]
    @ProjectId INT,
    @TableName Varchar(255)
AS
BEGIN
    WITH CTE AS (
        SELECT
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS OriginalOrder,
            s.ProjectId AS ProjectId,
            s.TableName AS TableName,
            s.ColumnName AS ColumnName,
            0 AS FunctionId,
            0 AS IsSelected,
            'New' AS DataType
        FROM
            dbo.stgMapping s WITH (NOLOCK)
        WHERE
            NOT EXISTS (
                SELECT TOP 1 1 FROM [dbo].[TableColumnFunctionMapping] t WITH (NOLOCK)
                WHERE t.ProjectId = s.ProjectId AND t.TableName = s.TableName
                AND t.ColumnName = s.ColumnName
            )
            AND s.ProjectId = @ProjectId
            AND s.TableName = @TableName

        UNION

        SELECT
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS OriginalOrder,
            s.ProjectId AS ProjectId,
            s.TableName AS TableName,
            s.ColumnName AS ColumnName,
            s.FunctionId AS FunctionId,
            s.IsSelected AS IsSelected,
            'Existing' AS DataType
        FROM
            [dbo].[TableColumnFunctionMapping] s
        WHERE
            EXISTS (
                SELECT TOP 1 1
                FROM [dbo].stgMapping t WITH (NOLOCK)
                WHERE t.ProjectId = s.ProjectId and t.TableName = s.TableName
                and t.ColumnName = s.ColumnName
            )
            AND s.ProjectId = @ProjectId
            AND s.TableName = @TableName

        UNION

        SELECT
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS OriginalOrder,
            s.ProjectId AS ProjectId,
            s.TableName AS TableName,
            s.ColumnName AS ColumnName,
            0 AS FunctionId,
            0 AS IsSelected,
            'Deleted' AS DataType
        FROM
            dbo.[TableColumnFunctionMapping] s WITH (NOLOCK)
        WHERE
            NOT EXISTS (
                SELECT TOP 1 1
                FROM [dbo].stgMapping t WITH (NOLOCK)
                WHERE t.ProjectId = s.ProjectId and t.TableName = s.TableName
                and t.ColumnName = s.ColumnName
            )
            AND s.ProjectId = @ProjectId
            AND s.TableName = @TableName
    )

    SELECT *
    FROM CTE
    ORDER BY OriginalOrder; -- Order by the added OriginalOrder column
END