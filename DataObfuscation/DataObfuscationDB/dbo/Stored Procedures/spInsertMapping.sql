CREATE PROCEDURE [dbo].[spInsertMapping]
    @ProjectId INT,
    @TableName VARCHAR(255),
    @ColumnName VARCHAR(255),
    @OperationType VARCHAR(20),
    @IsSelected BIT,
    @FunctionId INT,
    @ConstantValue VARCHAR(255) = '' -- Declare @ConstantValue as nullable
AS
BEGIN
    -- Check if @OperationType is blank or empty, and set it to 'Existing'
    IF (LEN(@OperationType) = 0)
        SET @OperationType = 'Existing';

    IF @OperationType = 'New'
    BEGIN
    UPDATE t
    SET t.FunctionId = @FunctionId, t.IsSelected = @IsSelected, t.ConstantValue = @ConstantValue
    FROM [dbo].[TableColumnFunctionMapping] t
    WHERE t.ProjectId = @ProjectId
      AND t.TableName = @TableName
      AND t.ColumnName = @ColumnName;
        -- Insert 'New' rows into [dbo].[TableColumnFunctionMapping] for a specific column
        -- INSERT INTO [dbo].[TableColumnFunctionMapping] (ProjectId, TableName, ColumnName, FunctionId, IsSelected, ConstantValue, ObfuscatedDate)
        -- SELECT
        --     s.ProjectId,
        --     s.TableName,
        --     s.ColumnName,
        --     @FunctionId AS FunctionId,
        --     @IsSelected AS IsSelected,
        --     (@ConstantValue) AS ConstantValue, -- Use ISNULL to handle null value
        --     '' as ObfuscatedDate
            
        -- FROM
        --     dbo.stgMapping s
        -- WHERE
        --     NOT EXISTS (
        --         SELECT 1
        --         FROM [dbo].[TableColumnFunctionMapping] t
        --         WHERE t.ProjectId = s.ProjectId
        --           AND t.TableName = s.TableName
        --           AND t.ColumnName = s.ColumnName
        --           AND @OperationType = 'New'
        --     )
        --     AND s.ProjectId = @ProjectId
        --     AND s.TableName = @TableName
        --     AND s.ColumnName = @ColumnName; -- Apply only to the specified column
    END
    ELSE IF @OperationType = 'Existing'
    --BEGIN
    --    -- Update 'Existing' rows in [dbo].[TableColumnFunctionMapping] for a specific column
    --    UPDATE t
    --    SET t.FunctionId = @FunctionId, t.IsSelected = @IsSelected, t.ConstantValue = (@ConstantValue)
    --    FROM [dbo].[TableColumnFunctionMapping] t
    --    JOIN dbo.stgMapping s
    --    ON t.ProjectId = s.ProjectId
    --       AND t.TableName = s.TableName
    --       AND t.ColumnName = s.ColumnName
    --    WHERE t.ProjectId = @ProjectId
    --      AND t.TableName = @TableName
    --      AND t.ColumnName = @ColumnName; -- Apply only to the specified column
    --END
	BEGIN
    -- Update 'Existing' rows in [dbo].[TableColumnFunctionMapping] for a specific column
    UPDATE t
    SET t.FunctionId = @FunctionId, t.IsSelected = @IsSelected, t.ConstantValue = @ConstantValue
    FROM [dbo].[TableColumnFunctionMapping] t
    WHERE t.ProjectId = @ProjectId
      AND t.TableName = @TableName
      AND t.ColumnName = @ColumnName; -- Apply only to the specified column
END

    ELSE IF @OperationType = 'Deleted'
    BEGIN
        -- Delete 'Deleted' rows from [dbo].[TableColumnFunctionMapping] for a specific column
        DELETE t
        FROM [dbo].[TableColumnFunctionMapping] t
        WHERE NOT EXISTS (
            SELECT *
            FROM dbo.stgMapping s
            WHERE s.ProjectId = t.ProjectId
              AND s.TableName = t.TableName
              AND s.ColumnName = t.ColumnName
            )
            AND t.ProjectId = @ProjectId
            AND t.TableName = @TableName
            AND t.ColumnName = @ColumnName; -- Apply only to the specified column
    END
    -- Handle other cases or provide error handling if needed
END