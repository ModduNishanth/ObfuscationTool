CREATE PROCEDURE [dbo].[spGetObfuscateDataQuery] (
    @ProjectID AS INT,
    @TablesName AS VARCHAR(MAX)
)
AS
BEGIN
    DECLARE @FunctionName AS VARCHAR(255)
    DECLARE @AddnParamVal AS VARCHAR(255)
    DECLARE @ConstantValue AS VARCHAR(255) 
    DECLARE @TableName AS VARCHAR(MAX) 
    DECLARE @ColumnName VARCHAR(MAX)
    DECLARE @FunctionId INT  -- Added FunctionId variable
    DECLARE @sql VARCHAR(MAX)
    DECLARE @sql1 VARCHAR(MAX)
    DECLARE @ObfuscatedDate DATE = GETDATE();
    
    -- Create a table variable to store the update statements
    DECLARE @FinalUpdateStatements TABLE (TableName VARCHAR(255), UpdateStatement VARCHAR(MAX));


    -- Convert TableNames comma-separated into rows in a temporary table (#TempTable)
	-- The @FinalUpdateStatements table variable is created to store the final update statements, which will include the table name and the corresponding update statement.
    -- A temporary table #TempTable is created to store the table names provided in the @TablesName parameter. The table names are split using the STRING_SPLIT function and inserted into #TempTable.
    CREATE TABLE #TempTable (TableName VARCHAR(255));
    INSERT INTO #TempTable (TableName)
    SELECT value
    FROM STRING_SPLIT(@TablesName, ',');


    -- Cursor to iterate through the table names
	-- A cursor tablecursor is created to iterate through the table names stored in #TempTable.
    -- The OPEN statement opens the cursor, and the FETCH NEXT statement retrieves the first table name from the cursor into the @TableName variable.
    -- The WHILE loop executes as long as there are more rows to fetch from the cursor.
    DECLARE tablecursor CURSOR FOR
    SELECT TableName
    FROM #TempTable;

    OPEN tablecursor;
    FETCH NEXT FROM tablecursor INTO @TableName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Create the #Temp2 table to store the update statements for the current table
        CREATE TABLE #Temp2 (UpdateStatement VARCHAR(MAX));

        -- Cursor to iterate through the table/column mappings
        DECLARE datacursor CURSOR FOR
        SELECT a.TableName, a.ColumnName, a.AddnlParamValue, a.FunctionId, a.ConstantValue  -- Include FunctionId in the select statement
        FROM TableColumnFunctionMapping a
        INNER JOIN Functions b ON a.FunctionId = b.ID
        WHERE a.IsDeleted = 0 AND b.IsDeleted = 0 AND (a.TableName = @TableName OR @TableName = 'ALL') AND a.IsObfuscated = 0 And a.IsSelected=1
        AND a.ProjectID = @ProjectID
        ORDER BY a.TableName, a.ColumnName;


		--The OPEN statement opens the datacursor to start fetching rows from the cursor.
        -- The FETCH NEXT statement retrieves the first row from the cursor and assigns the column values to their corresponding variables.




        OPEN datacursor;
        FETCH NEXT FROM datacursor INTO @TableName, @ColumnName, @AddnParamVal, @FunctionId, @ConstantValue; -- Update the variables

      
		--These variables (@CurrentTable and @CurrentUpdateStatement) are used to keep track of the current table being processed and the update statement being constructed.
        DECLARE @CurrentTable VARCHAR(255) = '';
        DECLARE @CurrentUpdateStatement NVARCHAR(MAX) = '';
		        DECLARE @encryptstatement NVARCHAR(MAX) = '';



	--This block checks if a new table is encountered during the iteration. If it is a new table, it performs the following actions:
       --If it's not the first table, it checks if the previous update statement already exists in #Temp2. If it doesn't exist, it inserts the previous update statement into #Temp2.
       --It sets the current table name (@CurrentTable) to the new table name.
       --It initializes the current update statement (@CurrentUpdateStatement) to start building a new update statement for the current table.

        WHILE @@FETCH_STATUS = 0
        BEGIN
            IF @CurrentTable <> @TableName -- New table encountered
            BEGIN
                IF @CurrentTable <> '' -- Not the first table, save the previous update statement
                BEGIN
                    -- Check if the same update statement already exists in #Temp2, if not, insert it
                    IF NOT EXISTS (
                        SELECT 1
                        FROM #Temp2
                        WHERE UpdateStatement = @CurrentUpdateStatement
                    )
                    BEGIN
                        INSERT INTO #Temp2 (UpdateStatement)
                        VALUES (@CurrentUpdateStatement);
                    END
                END
                




                -- Start building a new update statement for the current table
                SET @CurrentTable = @TableName;
                SET @CurrentUpdateStatement = '  UPDATE ' + @TableName + ' SET ';
            END

            SET @sql = '';
            SET @sql1 = '';

			-- This query retrieves the function name (@FunctionName) from the Functions table based on the current FunctionId.
            SELECT @FunctionName = Name
            FROM Functions
            WHERE ID = @FunctionId;


			--This block handles the case when the FunctionId is not 7 (i.e., for other functions).
          --If the FunctionName is "ConstantValue", it sets the @sql variable to assign a constant value to the column.
          --If the FunctionName is not "ConstantValue", it constructs the @sql variable based on the function name and the column name. It also appends the additional parameter value, if present.
          --The @sql1 variable is updated accordingly to represent the column name or the function result as a new column name.
            IF @FunctionId = 7  -- Check if FunctionId is 7 (SelfRandom)
            BEGIN
                SET @FunctionName = 'SelfRandom';

                SET @sql = @ColumnName + ' = (SELECT TOP 1 ' + @ColumnName + ' FROM ' + @TableName + ' a WHERE a.' + @ColumnName + ' <> ' + @TableName + '.' + @ColumnName + ' ORDER BY NEWID()), ';
                SET @sql1 = @ColumnName + ',' + @ColumnName + 'New = (SELECT TOP 1 ' + @ColumnName + ' FROM ' + @TableName + ' a WHERE a.' + @ColumnName + ' <> ' + @TableName + '.' + @ColumnName + ' ORDER BY NEWID())';

                -- Print the SelfRandom statement
                PRINT @sql;
            END
            ELSE 
            BEGIN
                  IF @FunctionName = 'ConstantValue'
                BEGIN
				
		    	DECLARE @JConstantValue NVARCHAR(255);
			    SELECT @JConstantValue = JSON_VALUE(@ConstantValue, '$.ConstantValue');
                    SET @sql = @ColumnName + '=''' + CAST(@JConstantValue AS VARCHAR(255)) + '''                                                        , ';
                    SET @sql1 = @ColumnName + ',''' + CAST(@JConstantValue AS VARCHAR(255)) + '''';
					print @JConstantValue;
                END
			else  IF @FunctionName = 'dbo.EncryptCertificate'
                BEGIN
                 -- Get the certificateName from the ConstantValue column in JSON format
                  DECLARE @CertificateName NVARCHAR(255);
                    SELECT @CertificateName = JSON_VALUE(@ConstantValue, '$.CertificateName');
	 
	              print @CertificateName
					 -- Generate the update statement for Certificate function
					--SET @sql = @ColumnName + ' = dbo.EncryptCertificate(' + @ColumnName + ',''' + @CertificateName + '''), ';
					--SET @sql1 = @ColumnName + ',''' + @CertificateName + '''';

	             SET @encryptstatement = ' OPEN SYMMETRIC KEY SymmetricKey1 DECRYPTION BY CERTIFICATE ' + @CertificateName +
               '; UPDATE ' + @TableName +
               ' SET  Encrypted_Value = ENCRYPTBYKEY(KEY_GUID(''SymmetricKey1''),' + @ColumnName + ')' +
               '; CLOSE SYMMETRIC KEY SymmetricKey1 ; '

			    PRINT @encryptstatement;
			 
               END

                ELSE 
                BEGIN
                    SET @sql = @ColumnName + ' = ' + @FunctionName + '(';

                    IF @FunctionName <> 'fnGenerateRandomNumberBetweenRange'
                    BEGIN
                        SET @sql = @sql + @ColumnName;
                        SET @sql1 = @sql1 + @ColumnName;
                    END

                    IF LEN(ISNULL(@AddnParamVal, '')) > 0
                    BEGIN
                        IF @FunctionName = 'fnGenerateRandomNumberBetweenRange'
                        BEGIN
                            SET @sql = @sql + @AddnParamVal;
                            SET @sql1 = @sql1 + @AddnParamVal;
                        END
                        ELSE
                        BEGIN
                            SET @sql = @sql + ',' + @AddnParamVal;
                            SET @sql1 = @sql1 + ',' + @AddnParamVal;
                        END
                    END

                    SET @sql = @sql + ')                  , ';
                    SET @sql1 = @sql1 + ' AS ' + @ColumnName;
                END
            END

            
--This block executes within the nested datacursor loop.
--It appends the column update statement to the current update statement for the table.
--It updates the TableColumnFunctionMapping table to set the IsObfuscated flag as 1 and the ObfuscatedDate as the current date for the processed column.
--It fetches the next row from the datacursor into the respective variables.
--After iterating through all the column mappings for a table, it checks if the same update statement already exists in #Temp2. If it doesn't, it inserts the update statement into #Temp2.
--It inserts the update statements for the current table into the @FinalUpdateStatements table variable.
--The temporary table #Temp2 is dropped to clear the data for the current table.
        
           SET @CurrentUpdateStatement =@encryptstatement+ @CurrentUpdateStatement+'                   '+ @sql;
           set @encryptstatement='';

            -- Update the mapping table to set IsObfuscated as 1
            UPDATE TableColumnFunctionMapping
            SET IsObfuscated = 1, ObfuscatedDate = @ObfuscatedDate
            WHERE TableName = @TableName AND ColumnName = @ColumnName AND ProjectId = @ProjectID;

            FETCH NEXT FROM datacursor INTO @TableName, @ColumnName, @AddnParamVal, @FunctionId, @ConstantValue;
        END;

        CLOSE datacursor;
        DEALLOCATE datacursor;

        -- Check if the same update statement already exists in #Temp2, if not, insert it
        IF NOT EXISTS (
            SELECT 1
            FROM #Temp2
            WHERE UpdateStatement = @CurrentUpdateStatement
        )
        BEGIN
            -- Insert the last update statement into #Temp2
            IF @CurrentUpdateStatement <> ''
            BEGIN
			 set @CurrentUpdateStatement = @CurrentUpdateStatement +'                   '
                SET @CurrentUpdateStatement = LEFT(@CurrentUpdateStatement, LEN(@CurrentUpdateStatement) - 19); -- Remove trailing comma
                INSERT INTO #Temp2 (UpdateStatement)
                VALUES (@CurrentUpdateStatement);
            END;
        END;

        -- Select the update statements for the current table into the final table variable
        INSERT INTO @FinalUpdateStatements (TableName, UpdateStatement)
        SELECT @TableName, UpdateStatement
        FROM #Temp2;

        -- Drop the temporary table for the current table
        DROP TABLE #Temp2;

       FETCH NEXT FROM tablecursor INTO @TableName;
    END;

    CLOSE tablecursor;
    DEALLOCATE tablecursor;

    -- Select the update statements for each table from the final table variable
	--This query selects the update statements from the @FinalUpdateStatements table variable, which contains the table name and corresponding update statement for each table.
	--The result set is returned as the output of the stored procedure.

    SELECT TableName, UpdateStatement
    FROM @FinalUpdateStatements;

   
--This block is responsible for printing the remaining update queries for tables without the SelfRandom function.
--A new temporary table @RemainingUpdateStatements is created to store the update statements that do not contain the SelfRandom function.
--TheINSERT INTO statement inserts the update statements that do not contain the SelfRandom function into the @RemainingUpdateStatements table.
--The subquery filters the update statements from @FinalUpdateStatements where the table name is not present in the table names associated with the SelfRandom function.
--A new cursor remainingcursor is created to iterate through the update statements in @RemainingUpdateStatements.
--The OPEN statement opens the cursor, and the FETCH NEXT statement retrieves the first update statement from the cursor into the @RemainingUpdateStatement variable.
--The WHILE loop executes as long as there are more update statements to fetch from the cursor.
--Within the loop, each remaining update statement is printed using the PRINT statement.
--The next update statement is fetched from the cursor using FETCH NEXT.
--Once all the update statements are printed, the cursor is closed and deallocated.


    DECLARE @RemainingUpdateStatements TABLE (UpdateStatement VARCHAR(MAX));
    INSERT INTO @RemainingUpdateStatements (UpdateStatement)
    SELECT UpdateStatement
    FROM @FinalUpdateStatements
    WHERE TableName NOT IN (
        SELECT DISTINCT TableName
        FROM @FinalUpdateStatements
        WHERE UpdateStatement LIKE '%SelfRandom%'
    );

    DECLARE @RemainingUpdateStatement VARCHAR(MAX);
    DECLARE remainingcursor CURSOR FOR
    SELECT UpdateStatement
    FROM @RemainingUpdateStatements;

    OPEN remainingcursor;
    FETCH NEXT FROM remainingcursor INTO @RemainingUpdateStatement;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        PRINT @RemainingUpdateStatement;
        FETCH NEXT FROM remainingcursor INTO @RemainingUpdateStatement;
    END;

    CLOSE remainingcursor;
    DEALLOCATE remainingcursor;

   
	--This statement drops the temporary table #TempTable, which was used to store the table names provided as input to the stored procedure.
    DROP TABLE #TempTable;
END