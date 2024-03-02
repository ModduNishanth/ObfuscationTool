using DataObfuscationApp.Model;
using DOA.Repository.Interface;
using Models.Common;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;
using OfficeOpenXml;
using Microsoft.Extensions.Configuration;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace DOA.Repository.Implementation
{
    public class MappingTable : IMappingTable
    {
        private readonly IConfiguration _configuration;

        public MappingTable(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<string>> MapTableColumn(GetTableColumn column)
        {
            List<string> columnNames = new List<string>();

            try
            {
                // Retrieve the connection string from configuration
                string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
                string connectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));

                // Check if the connection string is valid
                if (!string.IsNullOrWhiteSpace(connectionString) && connectionString.StartsWith("Server="))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        // Construct the SQL query with parameterized query to prevent SQL injection
                        string query = $"SELECT ColumnName FROM dbo.TableColumnFunctionMapping WHERE ProjectId = @ProjectId AND TableName= @TableName";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ProjectId", column.ProjectId);
                        command.Parameters.AddWithValue("@TableName", column.tableName);
                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            // Retrieve and process column name from the result
                            string columnName = reader.GetString(0);
                            columnNames.Add(columnName);
                        }

                        reader.Close();

                        return columnNames; // Return the list of column names
                    }
                }
                else
                {
                    return null; // Invalid connection string
                }
            }
            catch (Exception ex)
            {
                // TODO: Log the exception for debugging
                return null; // Mapping failed
            }
        }
        public async Task<List<IsObfuscated>> MappedTables(Update update)
        {
            List<IsObfuscated> TableNames = new List<IsObfuscated>();
            string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
            string connectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));
            if (connectionString.StartsWith("Server="))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        //SqlCommand command = new SqlCommand($"declare @pProjectId int = {update.ProjectId} " +
                        //    $"SELECT DISTINCT TableName, IsObfuscated,ObfuscatedDate FROM dbo.TableColumnFunctionMapping Where ProjectID = @pProjectId AND IsSelected=1;", connection);

                        SqlCommand command = new SqlCommand($"DECLARE @pProjectId INT = {update.ProjectId};" +
                                                           "SELECT TableName, MAX(CAST(IsObfuscated AS INT)) AS IsObfuscated, MAX(ObfuscatedDate) AS ObfuscatedDate " +
                                                           "FROM dbo.TableColumnFunctionMapping " +
                                                           "WHERE ProjectID = @pProjectId AND IsSelected = 1 " +
                                                           "GROUP BY TableName;",
                                                           connection);

                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            string TableName = reader.GetString(0);
                            bool IsObfuscated = reader.GetInt32(1) == 1;
                            string ObfuscatedDate = reader.GetString(2);

                            TableNames.Add(new IsObfuscated
                            {
                                TableNames = TableName,
                                Obfuscated = IsObfuscated,
                                ObfuscatedDate = ObfuscatedDate
                            });
                        }

                        reader.Close();
                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return (TableNames);
        }
        public async Task<byte[]> GetTableDataInExcelFormat(Update update)
        {
            byte[] excelData = null;
            string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
            string connectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
                if (connectionString.StartsWith("Server="))
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            await connection.OpenAsync();
                            SqlCommand command = new SqlCommand($"select ProjectName, DatabaseName, TableName, ColumnName, IsSelected, IsObfuscated from ExcelDataView Where ProjectId = {update.ProjectId}", connection);
                            using (SqlDataReader reader = await command.ExecuteReaderAsync())
                            {
                                // Create a worksheet
                                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ExcelDataView");

                                // Set the column headers
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    worksheet.Cells[1, i + 1].Value = reader.GetName(i);
                                    worksheet.Cells[1, i + 1].Style.Font.Bold = true; // Set the font to bold
                                }

                                // Populate the data rows
                                int rowIndex = 2;
                                while (await reader.ReadAsync())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        string columnName = reader.GetName(i);
                                        object columnValue = reader.GetValue(i);

                                        // Convert IsObfuscated and IsSelected columns
                                        if (columnName == "IsSelected")
                                        {
                                            worksheet.Cells[rowIndex, i + 1].Value = (int)columnValue == 1 ? true : false;
                                        }
                                        else
                                        {
                                            worksheet.Cells[rowIndex, i + 1].Value = columnValue;
                                        }
                                    }
                                    rowIndex++;
                                }

                                // Convert the Excel package to byte array
                                excelData = excelPackage.GetAsByteArray();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            return excelData;
        }
        public async Task<List<vwGetTableColumnData>> GetAllTableColumnData(GetTableColumn getTableColumn)
        {
            List<vwGetTableColumnData> tableColumnDataList = new List<vwGetTableColumnData>();

            // Read the connection string from the appsettings.json file
            string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
            string connectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));

            if (connectionString.StartsWith("Server="))
            {
                try
                {
                    // Step 1: Establish a connection
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        // Step 2: Retrieve data from the view
                        string query = $"SELECT * FROM dbo.vwGetTableColumnData where ProjectId={getTableColumn.ProjectId} AND TableName ='{getTableColumn.tableName}'";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            // Step 3: Execute the SQL query and retrieve the data
                            using (SqlDataReader reader = await command.ExecuteReaderAsync())
                            {
                                while (reader.Read())
                                {
                                    vwGetTableColumnData columnData = new vwGetTableColumnData
                                    {
                                        // Assuming TableColumnData class properties match the columns in the view
                                        // You need to adjust property names according to your TableColumnData class
                                        ProjectID = reader.IsDBNull(reader.GetOrdinal("ProjectID")) ? default : reader.GetInt32(reader.GetOrdinal("ProjectID")),
                                        MappingID = reader.IsDBNull(reader.GetOrdinal("MappingID")) ? default : reader.GetInt32(reader.GetOrdinal("MappingID")),
                                        ColumnName = reader.IsDBNull(reader.GetOrdinal("ColumnName")) ? null : reader.GetString(reader.GetOrdinal("ColumnName")),
                                        FunctionID = reader.IsDBNull(reader.GetOrdinal("FunctionID")) ? default : reader.GetInt32(reader.GetOrdinal("FunctionID")),
                                        MappingStatus = reader.IsDBNull(reader.GetOrdinal("MappingStatus")) ? null : reader.GetString(reader.GetOrdinal("MappingStatus")),
                                        DisplayName = reader.IsDBNull(reader.GetOrdinal("DisplayName")) ? null : reader.GetString(reader.GetOrdinal("DisplayName")),
                                        // Add more properties as needed
                                    };

                                    tableColumnDataList.Add(columnData);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle the exception
                    throw;
                }
            }

            return tableColumnDataList;
        }
        public async Task<bool> AddFunctionNo(FunctionAdd functionAdd)
        {
            string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
            string connectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));

            if (connectionString.StartsWith("Server="))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        await connection.OpenAsync();

                        for (int i = 0; i < functionAdd.ColumnName.Count; i++)
                        {

                            // Create a JSON object to store ConstantValue and certificateName
                            //var jsonParams = new
                            //{
                            //    ConstantValue = functionAdd.ConstantValue[i],
                            //    CertificateName = functionAdd.CertificateName[i]
                            //};

                            //// Convert the object to a JSON string
                            //string jsonParamsString = JsonConvert.SerializeObject(jsonParams);

                            // Use the JSON string in the query
                            //string query = $"EXEC [dbo].[spInsertMapping] @ProjectId={functionAdd.ProjectID}, @TableName='{functionAdd.TableName}', @ColumnName='{functionAdd.ColumnName[i]}'," +
                            //    $" @OperationType='{functionAdd.ColumnStatus[i]}',@IsSelected=1, @FunctionId={functionAdd.FunctionNo[i]}, @JsonParams='{jsonParamsString}'";

                            string jsonParams;

                            if (functionAdd.FunctionNo[i] == 9)
                            {
                                jsonParams = $"{{ \"CertificateName\": \"{functionAdd.CertificateName[i]}\" }}";
                            }
                            else if (functionAdd.FunctionNo[i] == 8)
                            {
                                // Additional condition for FunctionNo 8
                                jsonParams = $"{{ \"ConstantValue\": \"{functionAdd.ConstantValue[i]}\" }}";
                                // Set jsonParams to an empty string for FunctionNo 8
                            }
                            else
                            {
                                jsonParams = "";
                            }

                            if (functionAdd.FunctionNo[i] == 0)
                            {
                                string query = $"EXEC [dbo].[spInsertMapping] @ProjectId={functionAdd.ProjectID}, @TableName='{functionAdd.TableName}', @ColumnName='{functionAdd.ColumnName[i]}'," +
                               $" @OperationType='{functionAdd.ColumnStatus[i]}',@IsSelected=0, @FunctionId={functionAdd.FunctionNo[i]} , @ConstantValue='{jsonParams}'";
                                using (SqlCommand command = new SqlCommand(query, connection))
                                {
                                    int rowsAffected = await command.ExecuteNonQueryAsync();
                                    if (rowsAffected <= 0)
                                    {
                                        // Update failed or no rows were affected
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                string query = $"EXEC [dbo].[spInsertMapping] @ProjectId={functionAdd.ProjectID}, @TableName='{functionAdd.TableName}', @ColumnName='{functionAdd.ColumnName[i]}'," +
                               $" @OperationType='{functionAdd.ColumnStatus[i]}',@IsSelected={functionAdd.IsSelected[i]}, @FunctionId={functionAdd.FunctionNo[i]} , @ConstantValue='{jsonParams}'";
                                using (SqlCommand command = new SqlCommand(query, connection))
                                {
                                    int rowsAffected = await command.ExecuteNonQueryAsync();
                                    if (rowsAffected <= 0)
                                    {
                                        // Update failed or no rows were affected
                                        return false;
                                    }
                                }
                            }
                        }

                        string insertDataIfNeededQuery = "EXEC [dbo].[InsertDataIfNeeded]";
                        using (SqlCommand insertDataIfNeededCommand = new SqlCommand(insertDataIfNeededQuery, connection))
                        {
                            await insertDataIfNeededCommand.ExecuteNonQueryAsync();
                        }




                        // All updates were successful
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Handle other exceptions
                        // Log or handle the exception as needed
                        throw;
                    }
                    finally
                    {
                        // Close the connection
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                        }
                    }

                    //try
                    //{
                    //    await connection.OpenAsync();

                    //    for (int i = 0; i < functionAdd.TableName.Count; i++)
                    //    {
                    //        // Fetch column names for the current table
                    //        for (int j = 0; j < functionAdd.ColumnName.Count; j++)  
                    //        {
                    //            string query = $"EXEC [dbo].[spInsertMapping] @ProjectId={functionAdd.ProjectID}, @TableName='{functionAdd.TableName[i]}', @ColumnName='{functionAdd.ColumnName[j]}'," +
                    //                $" @OperationType='{functionAdd.ColumnStatus[j]}',@IsSelected=1, @FunctionId={functionAdd.FunctionNo[j]} , @ConstantValue='{functionAdd.ConstantValue[j]}'";

                    //            using (SqlCommand command = new SqlCommand(query, connection))
                    //            {
                    //                int rowsAffected = await command.ExecuteNonQueryAsync();
                    //                if (rowsAffected <= 0)
                    //                {
                    //                    // Update failed or no rows were affected
                    //                    return false;
                    //                }
                    //            }
                    //        }
                    //    }

                    //    string insertDataIfNeededQuery = "EXEC [dbo].[InsertDataIfNeeded]";
                    //    using (SqlCommand insertDataIfNeededCommand = new SqlCommand(insertDataIfNeededQuery, connection))
                    //    {
                    //        await insertDataIfNeededCommand.ExecuteNonQueryAsync();
                    //    }

                    //    // All updates were successful
                    //    return true;
                    //}
                    //catch (Exception ex)
                    //{
                    //    // Handle other exceptions
                    //    // Log or handle the exception as needed
                    //    throw;
                    //}
                    //finally
                    //{
                    //    // Close the connection
                    //    if (connection.State == ConnectionState.Open)
                    //    {
                    //        connection.Close();
                    //    }
                    //}


                }
            }

            return false; // Connection string doesn't start with "Server="
        }
        public async Task<bool> InsertStgMapping(ExecUpdateQueryObfuscationModel request)
        {
            try
            {
                // Retrieve the base64 encoded connection string from configuration
                string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
                string connectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));

                // Check if the connection string is valid
                if (connectionString.StartsWith("Server="))
                {
                    // Create and open a database connection asynchronously
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        // Use parameters to prevent SQL injection
                        string sqlCommandText = "EXEC [dbo].[spInsertStgMapping] @ProjectID, @TableName";
                        SqlCommand command = new SqlCommand(sqlCommandText, connection);
                        command.Parameters.AddWithValue("@ProjectID", request.ProjectId);
                        command.Parameters.AddWithValue("@TableName", request.TableName);

                        // Execute the SQL command asynchronously
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        // Check if the operation was successful based on the rows affected
                        bool isSuccessful = rowsAffected > 0;

                        return isSuccessful;
                    }
                }
                else
                {
                    // Invalid connection string format
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, log, and possibly throw or return false
                // Example: Log.Error(ex, "An error occurred while inserting data");
                return false;
            }
        }

        //public async Task<List<ColumnInfo>> ExecuteSPAndGetColumnInfo(ExecUpdateQueryObfuscationModel request)
        //{
        //    try
        //    {
        //        string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
        //        string connectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));



        //        if (!connectionString.StartsWith("Server="))
        //        {
        //            // Invalid connection string format
        //            return null;
        //        }



        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            await connection.OpenAsync();



        //            // Execute the first stored procedure without retrieving data
        //            string execSqlCommandText1 = "EXEC [dbo].[spInsertStgMapping] @ProjectID, @TableName";
        //            using (SqlCommand execCommand1 = new SqlCommand(execSqlCommandText1, connection))
        //            {
        //                execCommand1.Parameters.AddWithValue("@ProjectID", request.ProjectId);
        //                execCommand1.Parameters.AddWithValue("@TableName", request.TableName);



        //                await execCommand1.ExecuteNonQueryAsync();
        //            }



        //            // Execute the second stored procedure to retrieve data
        //            string selectSqlCommandText2 = "EXEC [dbo].[spGetMapping] @ProjectID, @TableName";
        //            using (SqlCommand selectCommand2 = new SqlCommand(selectSqlCommandText2, connection))
        //            {
        //                selectCommand2.Parameters.AddWithValue("@ProjectID", request.ProjectId); // provide values for parameters
        //                selectCommand2.Parameters.AddWithValue("@TableName", request.TableName);



        //                using (SqlDataReader reader2 = await selectCommand2.ExecuteReaderAsync())
        //                {
        //                    List<ColumnInfo> columnInfoList = new List<ColumnInfo>();



        //                    while (await reader2.ReadAsync())
        //                    {
        //                        ColumnInfo columnInfo = new ColumnInfo
        //                        {
        //                            ColumnName = reader2.GetString(reader2.GetOrdinal("ColumnName")),
        //                            DataType = reader2.GetString(reader2.GetOrdinal("DataType"))
        //                        };
        //                        columnInfoList.Add(columnInfo);
        //                    }



        //                    return columnInfoList;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions here, log, and possibly throw or return null
        //        // Example: Log.Error(ex, "An error occurred while retrieving column info");
        //        return null;
        //    }
        //}

        public async Task<List<Models.Common.TableColumnInfo>> ExecuteSPAndGetColumnInfo(ExecUpdateQueryObfuscationModel request)
        {
            List<string> tableNames = new List<string>();
            List<string> columnNames = new List<string>();
            List<ColumnInfo> columnInfoList = new List<ColumnInfo>();
            List<Models.Common.TableColumnInfo> resultData = new List<Models.Common.TableColumnInfo>();


            // Read the connection string from the appsettings.json file
            string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
            string appSettingsConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));

            if (appSettingsConnectionString.StartsWith("Server="))
            {
                // Step 1: Establish a connection
                using (SqlConnection connection = new SqlConnection(appSettingsConnectionString))
                {
                    await connection.OpenAsync();

                    // Step 2: Execute SQL query
                    string sqlQuery = "SELECT * FROM dbo.projects WHERE ProjectID = @ProjectID";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        // Set the parameter value
                        command.Parameters.AddWithValue("@ProjectID", request.ProjectId);

                        // Step 3: Execute the select statement
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Create the connection string
                                string serverName = (string)reader["ServerName"];
                                string databaseName = (string)reader["DatabaseName"];
                                string username = (string)reader["Username"];
                                // Retrieve the encrypted password
                                //string encryptedPassword = (string)reader["Password"];

                                //// Decrypt the password using Base64 decoding
                                //byte[] passwordBytes = Convert.FromBase64String(encryptedPassword);
                                //string password = Encoding.UTF8.GetString(passwordBytes);
                                string password = (string)reader["password"];

                                string dataType = (string)reader["DataType"];

                                string connectionString = $"Server={serverName};Database={databaseName};Uid={username};Pwd={password};";
                                string mysqlconnectionString = $"server={serverName};uid={username};pwd={password};database={databaseName};port=3306";
                                string oracleconnectionString = $"server={serverName};uid={username};pwd={password};database={databaseName};port=3306";



                                if (dataType == "SQLSERVER")
                                {
                                    using (SqlConnection connection1 = new SqlConnection(connectionString))
                                    {
                                        try
                                        {
                                            await connection1.OpenAsync();
                                            SqlCommand command1 = new SqlCommand("SELECT \r\n    t.TABLE_NAME,\r\n    STUFF((\r\n        SELECT ', ' + c.COLUMN_NAME\r\n        FROM INFORMATION_SCHEMA.COLUMNS c\r\n        WHERE c.TABLE_NAME = t.TABLE_NAME\r\n        FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '') AS COLUMN_NAMES\r\nFROM \r\n    INFORMATION_SCHEMA.TABLES t\r\nWHERE \r\n    t.TABLE_TYPE = 'BASE TABLE'\r\nORDER BY \r\n    t.TABLE_NAME;", connection1);
                                            SqlDataReader reader1 = await command1.ExecuteReaderAsync();

                                            Dictionary<string, string> tableColumnDictionary = new Dictionary<string, string>();

                                            while (await reader1.ReadAsync())
                                            {
                                                string TableName = reader1.GetString(0);
                                                string columnNamesResult = reader1.GetString(1);

                                                tableNames.Add(TableName);
                                                tableColumnDictionary[TableName] = columnNamesResult;
                                            }

                                            reader.Close();
                                            string truncateSqlCommandText = "TRUNCATE TABLE dbo.stgMapping";
                                            using (SqlCommand truncateCommand = new SqlCommand(truncateSqlCommandText, connection))
                                            {
                                                await truncateCommand.ExecuteNonQueryAsync();

                                                foreach (string tableName in tableNames)
                                                {
                                                    if (tableColumnDictionary.TryGetValue(tableName, out string columnNamesResult))
                                                    {
                                                        // Split the comma-separated column names into an array
                                                        string[] columnNamesArray = columnNamesResult.Split(',');

                                                        // Check if the current table matches the desired table (e.g., based on ProjectId)
                                                        if (tableName.Trim() == request.TableName.Trim())
                                                        {
                                                            // Iterate through the column names and perform the desired action
                                                            foreach (string columnName in columnNamesArray)
                                                            {
                                                                // Insert the columnName into the stgMapping table
                                                                string insertSqlCommandText = "INSERT INTO dbo.stgMapping (TableName, ColumnName, ProjectId) VALUES (@TableName, @ColumnName, @ProjectId)";
                                                                using (SqlCommand insertCommand = new SqlCommand(insertSqlCommandText, connection))
                                                                {
                                                                    insertCommand.Parameters.AddWithValue("@TableName", tableName);
                                                                    insertCommand.Parameters.AddWithValue("@ColumnName", columnName.Trim());
                                                                    insertCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
                                                                    await insertCommand.ExecuteNonQueryAsync();

                                                                }
                                                            }
                                                        }




                                                    }
                                                }

                                                // After inserting data into stgMapping, execute the stored procedure for a specific table
                                                string spName = $"dbo.spGetMapping"; // Replace with your actual stored procedure name
                                                using (SqlCommand spCommand = new SqlCommand(spName, connection))
                                                {
                                                    spCommand.CommandType = CommandType.StoredProcedure;

                                                    // Add parameters for the stored procedure
                                                    spCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
                                                    spCommand.Parameters.AddWithValue("@TableName", request.TableName); // Pass the specific table name as a parameter



                                                    // Execute the stored procedure
                                                    using (SqlDataReader spReader = await spCommand.ExecuteReaderAsync())
                                                    {
                                                        while (await spReader.ReadAsync())
                                                        {
                                                            int projectId = spReader.GetInt32(1);
                                                            int roworder = (int)spReader.GetInt64(0);
                                                            string tableName = spReader.GetString(2);
                                                            string columnName = spReader.GetString(3);
                                                            string dataStatus = spReader.GetString(6);

                                                            // Add the result to the list
                                                            resultData.Add(new Models.Common.TableColumnInfo
                                                            {
                                                                ProjectId = projectId,
                                                                TableName = tableName,
                                                                ColumnName = columnName,
                                                                DataType = dataStatus
                                                            });

                                                        }
                                                    }
                                                    foreach (string tableName in tableNames)
                                                    {
                                                        if (tableColumnDictionary.TryGetValue(tableName, out string columnNamesResult))
                                                        {
                                                            // Split the comma-separated column names into an array
                                                            string[] columnNamesArray = columnNamesResult.Split(',');

                                                            // Check if the current table matches the desired table (e.g., based on ProjectId)
                                                            if (tableName.Trim() == request.TableName.Trim())
                                                            {
                                                                foreach (string columnName in columnNamesArray)
                                                                {
                                                                    // Check if the row already exists in the TableColumnFunctionMapping table
                                                                    string checkExistenceQuery = "SELECT COUNT(*) FROM dbo.TableColumnFunctionMapping WHERE TableName = @TableName AND ColumnName = @ColumnName AND ProjectId = @ProjectId";

                                                                    using (SqlCommand checkExistenceCommand = new SqlCommand(checkExistenceQuery, connection))
                                                                    {
                                                                        checkExistenceCommand.Parameters.AddWithValue("@TableName", tableName);
                                                                        checkExistenceCommand.Parameters.AddWithValue("@ColumnName", columnName.Trim());
                                                                        checkExistenceCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);

                                                                        int rowCount = (int)await checkExistenceCommand.ExecuteScalarAsync();

                                                                        if (rowCount == 0)
                                                                        {
                                                                            // Insert the columnName into the TableColumnFunctionMapping table
                                                                            string insertSqlCommandText1 = "INSERT INTO dbo.TableColumnFunctionMapping (TableName, ColumnName, ProjectId, IsSelected, FunctionId, ConstantValue, IsObfuscated, ObfuscatedDate, IsDeleted, AddDate) " +
                                                                                                            "VALUES (@TableName, @ColumnName, @ProjectId, 0, 0, ' ', 0, ' ', 0, ' ')";

                                                                            using (SqlCommand insertCommand = new SqlCommand(insertSqlCommandText1, connection))
                                                                            {
                                                                                insertCommand.Parameters.AddWithValue("@TableName", tableName);
                                                                                insertCommand.Parameters.AddWithValue("@ColumnName", columnName.Trim());
                                                                                insertCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);

                                                                                await insertCommand.ExecuteNonQueryAsync();
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }


                                                }



                                            }


                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                    }

                                }

                                //else if (dataType == "MYSQL")
                                //{
                                //    using (MySqlConnection connection1 = new MySqlConnection(connectionString))
                                //    {
                                //        try
                                //        {
                                //            await connection1.OpenAsync();
                                //            MySqlCommand command1 = new MySqlCommand("SHOW TABLES", connection1);
                                //            MySqlDataReader reader1 = (MySqlDataReader)await command1.ExecuteReaderAsync();

                                //            while (await reader1.ReadAsync())
                                //            {
                                //                string tableName = reader1.GetString(0);
                                //                tableNames.Add(tableName);
                                //            }

                                //            reader.Close();
                                //        }
                                //        catch (Exception ex)
                                //        {
                                //            Console.WriteLine(ex.Message);
                                //        }
                                //    }
                                //}


                                else if (dataType == "MYSQL")
                                {
                                    using (MySqlConnection connection1 = new MySqlConnection(mysqlconnectionString))
                                    {
                                        try
                                        {
                                            await connection1.OpenAsync();
                                            MySqlCommand command1 = new MySqlCommand($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{request.TableName}' ", connection1);
                                            MySqlDataReader reader1 = (MySqlDataReader)await command1.ExecuteReaderAsync();

                                            Dictionary<string, string> tableColumnDictionary = new Dictionary<string, string>();

                                            while (await reader1.ReadAsync())
                                            {


                                                string columnName = reader1.GetString(0);  // Access the first (and only) column
                                                columnNames.Add(columnName);


                                            }
                                            reader.Close();
                                            reader1.Close();
                                            string truncateSqlCommandText = "TRUNCATE TABLE stgMapping"; // Assumes stgMapping table is in the current database
                                            using (SqlCommand truncateCommand = new SqlCommand(truncateSqlCommandText, connection))
                                            {
                                                await truncateCommand.ExecuteNonQueryAsync();
                                            }

                                            using (SqlCommand truncateCommand = new SqlCommand(truncateSqlCommandText, connection))
                                            {
                                                await truncateCommand.ExecuteNonQueryAsync();

                                                foreach (string columnName in columnNames)
                                                {


                                                    // Insert the columnName into the stgMapping table
                                                    string insertSqlCommandText = "INSERT INTO stgMapping (TableName, ColumnName, ProjectId) VALUES (@TableName, @ColumnName, @ProjectId)";
                                                    using (SqlCommand insertCommand = new SqlCommand(insertSqlCommandText, connection))
                                                    {
                                                        insertCommand.Parameters.AddWithValue("@TableName", request.TableName);
                                                        insertCommand.Parameters.AddWithValue("@ColumnName", columnName);
                                                        insertCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
                                                        await insertCommand.ExecuteNonQueryAsync();
                                                    }
                                                }

                                                // After inserting data into stgMapping, execute the stored procedure for a specific table
                                                string spName = $"dbo.spGetMapping"; // Replace with your actual stored procedure name
                                                using (SqlCommand spCommand = new SqlCommand(spName, connection))
                                                {
                                                    spCommand.CommandType = CommandType.StoredProcedure;

                                                    // Add parameters for the stored procedure
                                                    spCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
                                                    spCommand.Parameters.AddWithValue("@TableName", request.TableName); // Pass the specific table name as a parameter

                                                    // Execute the stored procedure
                                                    using (SqlDataReader spReader = await spCommand.ExecuteReaderAsync())
                                                    {
                                                        while (await spReader.ReadAsync())
                                                        {
                                                            int projectId = spReader.GetInt32(1);
                                                            int roworder = (int)spReader.GetInt64(0);
                                                            string tableName = spReader.GetString(2);
                                                            string columnName = spReader.GetString(3);
                                                            string dataStatus = spReader.GetString(6);
                                                            // Add the result to the list
                                                            resultData.Add(new Models.Common.TableColumnInfo
                                                            {
                                                                ProjectId = projectId,
                                                                TableName = tableName,
                                                                ColumnName = columnName,
                                                                DataType = dataStatus
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                    }
                                }

                                //else if (dataType == "ORACLE")
                                //{
                                //    using (OracleConnection connection1 = new OracleConnection(connectionString))
                                //    {
                                //        try
                                //        {
                                //            await connection1.OpenAsync();
                                //            OracleCommand command1 = new OracleCommand($"", connection1);
                                //            OracleDataReader reader1 = (OracleDataReader)await command1.ExecuteReaderAsync();

                                //            while (await reader.ReadAsync())
                                //            {
                                //                string tableName = reader.GetString(0);
                                //                tableNames.Add(tableName);
                                //            }
                                //            reader.Close();
                                //        }
                                //        catch (Exception ex)
                                //        {
                                //            Console.WriteLine(ex.Message);
                                //        }
                                //    }
                                //}
                                else if (dataType == "ORACLE")
                                {
                                    using (OracleConnection connection1 = new OracleConnection(oracleconnectionString))
                                    {
                                        try
                                        {
                                            await connection1.OpenAsync();

                                            // Define the SQL query for Oracle (may need adjustment based on Oracle database version)
                                            OracleCommand command1 = new OracleCommand("SELECT \r\n    TABLE_NAME,\r\n    LISTAGG(COLUMN_NAME, ', ') WITHIN GROUP (ORDER BY COLUMN_NAME) AS COLUMN_NAMES\r\nFROM \r\n    ALL_TAB_COLUMNS\r\nWHERE \r\n    OWNER = :owner AND TABLE_TYPE = 'TABLE'\r\nGROUP BY \r\n    TABLE_NAME", connection1);
                                            command1.Parameters.Add(new OracleParameter("owner", OracleDbType.Varchar2)).Value = connection1;

                                            OracleDataReader reader1 = (OracleDataReader)await command1.ExecuteReaderAsync();

                                            Dictionary<string, string> tableColumnDictionary = new Dictionary<string, string>();

                                            while (await reader1.ReadAsync())
                                            {
                                                string TableName = reader1.GetString(0);
                                                string columnNamesResult = reader1.GetString(1);

                                                tableNames.Add(TableName);
                                                tableColumnDictionary[TableName] = columnNamesResult;
                                            }

                                            reader.Close();
                                            reader1.Close();

                                            // Truncate table in Oracle
                                            string truncateSqlCommandText = "TRUNCATE TABLE stgMapping"; // Replace with your actual table name
                                            using (SqlCommand truncateCommand = new SqlCommand(truncateSqlCommandText, connection))
                                            {
                                                await truncateCommand.ExecuteNonQueryAsync();
                                            }

                                            using (SqlCommand truncateCommand = new SqlCommand(truncateSqlCommandText, connection))
                                            {
                                                await truncateCommand.ExecuteNonQueryAsync();

                                                foreach (string tableName in tableNames)
                                                {
                                                    if (tableColumnDictionary.TryGetValue(tableName, out string columnNamesResult))
                                                    {
                                                        // Split the comma-separated column names into an array
                                                        string[] columnNamesArray = columnNamesResult.Split(',');

                                                        // Check if the current table matches the desired table (e.g., based on ProjectId)
                                                        if (tableName.Trim() == request.TableName.Trim())
                                                        {
                                                            // Iterate through the column names and perform the desired action
                                                            foreach (string columnName in columnNamesArray)
                                                            {
                                                                // Insert the columnName into the stgMapping table

                                                                string insertSqlCommandText = "INSERT INTO stgMapping (TableName, ColumnName, ProjectId) VALUES (:TableName, :ColumnName, :ProjectId)";
                                                                using (OracleCommand insertCommand = new OracleCommand(insertSqlCommandText, connection1))
                                                                {
                                                                    insertCommand.Parameters.Add(new OracleParameter("TableName", OracleDbType.Varchar2)).Value = tableName;
                                                                    insertCommand.Parameters.Add(new OracleParameter("ColumnName", OracleDbType.Varchar2)).Value = columnName;
                                                                    insertCommand.Parameters.Add(new OracleParameter("ProjectId", OracleDbType.Int32)).Value = request.ProjectId;
                                                                    await insertCommand.ExecuteNonQueryAsync();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                // After inserting data into stgMapping, execute the stored procedure for a specific table
                                                string spName = "spGetMapping"; // Replace with your actual stored procedure name
                                                using (OracleCommand spCommand = new OracleCommand(spName, connection1))
                                                {
                                                    spCommand.CommandType = CommandType.StoredProcedure;

                                                    // Add parameters for the stored procedure
                                                    spCommand.Parameters.Add(new OracleParameter("ProjectId", OracleDbType.Int32)).Value = request.ProjectId;
                                                    spCommand.Parameters.Add(new OracleParameter("TableName", OracleDbType.Varchar2)).Value = request.TableName;

                                                    // Execute the stored procedure
                                                    using (OracleDataReader spReader = (OracleDataReader)await spCommand.ExecuteReaderAsync())
                                                    {
                                                        while (await spReader.ReadAsync())
                                                        {
                                                            int projectId = spReader.GetInt32(0);
                                                            string tableName = spReader.GetString(1);
                                                            string columnName = spReader.GetString(2);
                                                            string dataStatus = spReader.GetString(5);

                                                            // Add the result to the list
                                                            resultData.Add(new Models.Common.TableColumnInfo
                                                            {
                                                                ProjectId = projectId,
                                                                TableName = tableName,
                                                                ColumnName = columnName,
                                                                DataType = dataStatus
                                                            });
                                                        }
                                                    }
                                                }


                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                    }
                                }

                                else
                                {
                                    // Handle the case where the provider name is not recognized.
                                    Console.WriteLine("Invalid database provider name.");
                                }
                            }
                        }
                    }
                }
            }
            return resultData;

        }
    }
}


