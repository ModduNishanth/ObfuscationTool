Create PROCEDURE [dbo].[GetDisplayNames]
    @functionBlob NVARCHAR(MAX)
AS
BEGIN
    -- Declare variables
    DECLARE @functionNames NVARCHAR(MAX);
    DECLARE @sqlQuery NVARCHAR(MAX);

    -- Convert the blob to a string
    SET @functionNames = CONVERT(NVARCHAR(MAX), @functionBlob);

    -- Create a dynamic SQL query to join the "functions" table
    SET @sqlQuery = '
        SELECT f.Name, f.DisplayName
        FROM functions f
        INNER JOIN (
            SELECT value AS Name
            FROM STRING_SPLIT(@functionNames, '';'')
        ) selectedFunctions ON f.Name = selectedFunctions.Name
    ';

    -- Execute the dynamic SQL query
    EXEC sp_executesql @sqlQuery, N'@functionNames NVARCHAR(MAX)', @functionNames;
END