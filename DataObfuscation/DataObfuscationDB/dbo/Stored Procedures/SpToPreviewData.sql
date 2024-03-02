
CREATE PROCEDURE [dbo].[SpToPreviewData]
    @TableName VARCHAR(max),
    @ColumnName VARCHAR(max),
    @FunctionName VARCHAR(max),
    @ProjectID INT,
    @DatabaseType VARCHAR(max),
	@ConstantValue varchar(255) =''
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SqlQuery VARCHAR(MAX);

    IF (@DatabaseType = 'SQLSERVER')
    BEGIN
        IF (@FunctionName = 'dbo.selfrandom')
        BEGIN
            SET @SqlQuery = N' WITH UpdatedData AS ( SELECT ' + QUOTENAME(@ColumnName) + ' AS Name, (SELECT TOP 1 a.' + QUOTENAME(@ColumnName) + ' FROM ' + QUOTENAME(@TableName) + ' a  WHERE a.' + QUOTENAME(@ColumnName) + ' <> c.' + QUOTENAME(@ColumnName) + '   ORDER BY NEWID()) AS NewTableName  FROM ' + QUOTENAME(@TableName) + ' c ) SELECT * FROM UpdatedData;';
        END
        ELSE
        IF (@FunctionName = 'ConstantValue')
        BEGIN
		                    

            SET @SqlQuery = N'SELECT TOP 10 ' + QUOTENAME(@ColumnName) + ' AS Original_Data, ''' + @ConstantValue + ''' AS Masked_Data FROM ' + QUOTENAME(@TableName) + ';'
        END
        ELSE
        BEGIN
            SET @SqlQuery = N'SELECT TOP 10 ' + QUOTENAME(@ColumnName) + ' AS Original_Data, ' + @FunctionName + '(' + QUOTENAME(@ColumnName) + ') AS Masked_Data FROM ' + QUOTENAME(@TableName) + ';';
        END
    END





    ELSE IF (@DatabaseType = 'MYSQL')
    BEGIN
        IF (@FunctionName = 'dbo.selfrandom')
        BEGIN
            SET @SqlQuery = CONCAT('WITH UpdatedData AS ( SELECT ', (@ColumnName), ' ,(SELECT a.', (@ColumnName), ' FROM ', (@TableName), ' a  WHERE a.', (@ColumnName), ' <> c.', (@ColumnName), '  ORDER BY RAND() LIMIT 1   ) AS NewTableName  FROM ', (@TableName), ' c ) SELECT * FROM UpdatedData;');
        END
        ELSE
        IF (@FunctionName = 'ConstantValue')
        BEGIN
           SET @SqlQuery = CONCAT('SELECT ',  (@ColumnName), ' AS Original_Data,''',+ @ConstantValue+''' AS Masked_Data FROM ',  @TableName, ' LIMIT 5;' );

        END
        ELSE
        BEGIN
            SET @SqlQuery = CONCAT('SELECT ', (@ColumnName), ' AS Original_Data, ', CONCAT(@FunctionName, '(', (@ColumnName), ')'), ' AS Masked_Data FROM ', (@TableName), ' LIMIT 10;' );
        END
    END







    ELSE IF (@DatabaseType = 'ORACLE')
    BEGIN
        IF (@FunctionName = 'dbo.selfrandom')
        BEGIN
            SET @SqlQuery = CONCAT('WITH UpdatedData AS ( SELECT ', (@ColumnName), ' AS "Name",  (SELECT a.', (@ColumnName), '  FROM ', (@TableName), ' a   WHERE a.', (@ColumnName), ' <> c.', (@ColumnName), '  ORDER BY DBMS_RANDOM.VALUE ) AS "NewTableName",   ROW_NUMBER() OVER(ORDER BY 1) AS "rn"  FROM ', (@TableName), ' c )  SELECT * FROM UpdatedData WHERE "rn" = 1;');
        END
        ELSE
        IF (@FunctionName = 'ConstantValue')
        BEGIN
            SET @SqlQuery = CONCAT('SELECT',(@ColumnName),' AS "Original_Data", '''+@ConstantValue+''' As "Masked Data" From"', @TableName, '" FETCH FIRST 5 ROWS ONLY');
        END
        ELSE
        BEGIN
            SET @SqlQuery = CONCAT('SELECT ', (@ColumnName), ' AS "Original_Data", ',  CONCAT(@FunctionName, '(', (@ColumnName), ')'), ' AS "Masked_Data" FROM "', @TableName, '" FETCH FIRST 10 ROWS ONLY');
        END
    END




	select @SqlQuery;
END;