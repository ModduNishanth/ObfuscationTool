CREATE PROCEDURE [dbo].[spInsertStgMapping]
   @ProjectId nvarchar(200),
    @tablename NVARCHAR(128)
	--@columnName varchar (max)
	
AS
BEGIN
    DECLARE @sql NVARCHAR(MAX);
    
    -- Truncate stgMapping table
    TRUNCATE TABLE stgMapping;
    
    -- Build dynamic SQL to insert column names
    SET @sql = N'';
    SELECT @sql = @sql + N'INSERT INTO stgMapping (TableName, ColumnName, ProjectId)
                          SELECT ''' + @tablename + N''', COLUMN_NAME,'+@ProjectId +' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = ''' + @tablename + N''';'
    
    -- Execute dynamic SQL
    EXEC sp_executesql @sql;
END;