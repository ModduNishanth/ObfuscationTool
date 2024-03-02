CREATE PROCEDURE [dbo].[InsertDataIfNeeded]
AS
BEGIN
    -- Check if data from stgmapping exists in TableColumnFunctionMapping
    IF  EXISTS (
        SELECT 1
        FROM stgmapping sm
        WHERE NOT EXISTS (
            SELECT 1
            FROM TableColumnFunctionMapping tfm
            WHERE tfm.TableName = sm.TableName AND tfm.ColumnName = sm.ColumnName AND tfm.ProjectId=sm.ProjectId
        )
    )
    BEGIN
        -- Insert data from stgmapping into TableColumnFunctionMapping
        INSERT INTO TableColumnFunctionMapping (TableName, ColumnName, FunctionId, IsSelected,ProjectId, ObfuscatedDate,ConstantValue)
        SELECT TableName, ColumnName, 0, 0,ProjectId,'',''
        FROM stgmapping sm
        WHERE NOT EXISTS (
            SELECT 1
            FROM TableColumnFunctionMapping tfm
            WHERE tfm.TableName = sm.TableName AND tfm.ColumnName = sm.ColumnName AND tfm.ProjectId=sm.ProjectId
        )
    END
END;