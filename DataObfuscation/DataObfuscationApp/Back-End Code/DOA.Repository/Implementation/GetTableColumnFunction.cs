using DataObfuscationApp.Model;
using DOA.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Models.Common;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.Json.Nodes;


namespace DOA.Repository.Implementation
{
    public class GetTableColumnFunction : IGetTableColumnFunction
    {
        private readonly IConfiguration _configuration;

        public GetTableColumnFunction(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<string>> GetTableNames(Update update)
        {
            List<string> tableNames = new List<string>();
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
                        command.Parameters.AddWithValue("@ProjectID", update.ProjectId);

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

                                // Decrypt the password using Base64 decoding
                                //byte[] passwordBytes = Convert.FromBase64String(encryptedPassword);
                                //string password = Encoding.UTF8.GetString(passwordBytes);
                                string password = (string)reader["password"];
                                string dataType = (string)reader["DataType"];

                                string connectionString = $"server={serverName};uid={username};pwd={password};database={databaseName};";
                                string mysqlconnectionString = $"server={serverName};uid={username};pwd={password};database={databaseName};port=3306";
                                string oracleconnectionString = $"server={serverName};uid={username};pwd={password};database={databaseName};port=3306";

                                if (dataType == "SQLSERVER")
                                {
                                    using (SqlConnection connection1 = new SqlConnection(connectionString))
                                    {
                                        try
                                        {
                                            await connection1.OpenAsync();
                                            //SqlCommand command = new SqlCommand("SELECT '[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']' FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", connection);
                                            SqlCommand command1 = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'", connection1);
                                            SqlDataReader reader1 = await command1.ExecuteReaderAsync();

                                            while (await reader1.ReadAsync())
                                            {
                                                string tableName = reader1.GetString(0);
                                                tableNames.Add(tableName);
                                            }

                                            reader.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                    }
                                }
                                else if (dataType == "MYSQL")
                                {
                                    using (MySqlConnection connection1 = new MySqlConnection(mysqlconnectionString))
                                    {
                                        try
                                        {
                                            await connection1.OpenAsync();
                                            MySqlCommand command1 = new MySqlCommand("SHOW TABLES", connection1);
                                            MySqlDataReader reader1 = (MySqlDataReader)await command1.ExecuteReaderAsync();

                                            while (await reader1.ReadAsync())
                                            {
                                                string tableName = reader1.GetString(0);
                                                tableNames.Add(tableName);
                                            }

                                            reader.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                    }
                                }
                                else if (dataType == "ORACLE")
                                {
                                    using (OracleConnection connection1 = new OracleConnection(oracleconnectionString))
                                    {
                                        try
                                        {
                                            await connection1.OpenAsync();
                                            OracleCommand command1 = new OracleCommand($"SELECT table_name FROM user_tables;\r\n", connection1);
                                            OracleDataReader reader1 = (OracleDataReader)await command1.ExecuteReaderAsync();

                                            while (await reader.ReadAsync())
                                            {
                                                string tableName = reader.GetString(0);
                                                tableNames.Add(tableName);
                                            }

                                            reader.Close();
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
            return (tableNames);
        }

        public async Task<bool> GetColumnNames(GetTableColumn request)
        {
            bool success = false; // Initialize the success flag

            try
            {
                string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
                string appSettingsConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));

                if (appSettingsConnectionString.StartsWith("Server="))
                {
                    using (SqlConnection mainConnection = new SqlConnection(appSettingsConnectionString))
                    {
                        await mainConnection.OpenAsync();

                        string sqlQuery = "SELECT * FROM dbo.projects WHERE ProjectID = @ProjectID";

                        using (SqlCommand mainCommand = new SqlCommand(sqlQuery, mainConnection))
                        {
                            mainCommand.Parameters.AddWithValue("@ProjectID", request.ProjectId);

                            using (SqlDataReader mainReader = await mainCommand.ExecuteReaderAsync())
                            {
                                if (await mainReader.ReadAsync())
                                {
                                    string serverName = (string)mainReader["ServerName"];
                                    string databaseName = (string)mainReader["DatabaseName"];
                                    string username = (string)mainReader["Username"];
                                    //string encryptedPassword = (string)mainReader["Password"];
                                    //byte[] passwordBytes = Convert.FromBase64String(encryptedPassword);
                                    //string password = Encoding.UTF8.GetString(passwordBytes);
                                    string password = (string)mainReader["password"];
                                    string dataType = (string)mainReader["DataType"];
                                    string connectionString = $"Server={serverName};Database={databaseName};Uid={username};Pwd={password};";

                                    if (dataType == "SQLSERVER")
                                    {
                                        using (SqlConnection schemaConnection = new SqlConnection(connectionString))
                                        {
                                            await schemaConnection.OpenAsync();
                                            string schemaQuery = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@TableName";
                                            using (SqlCommand schemaCommand = new SqlCommand(schemaQuery, schemaConnection))
                                            {
                                                schemaCommand.Parameters.AddWithValue("@TableName", request.tableName);

                                                using (SqlDataReader schemaReader = await schemaCommand.ExecuteReaderAsync())
                                                {
                                                    while (await schemaReader.ReadAsync())
                                                    {
                                                        string columnName = schemaReader.GetString(0);

                                                        // Check if the record already exists in the mapping table
                                                        using (SqlConnection checkConnection = new SqlConnection(appSettingsConnectionString))
                                                        {
                                                            await checkConnection.OpenAsync();
                                                            string checkQuery = $"SELECT COUNT(*) FROM dbo.TableColumnFunctionMapping WHERE TableName=@TableName AND ColumnName=@ColumnName";
                                                            using (SqlCommand checkCommand = new SqlCommand(checkQuery, checkConnection))
                                                            {
                                                                checkCommand.Parameters.AddWithValue("@TableName", request.tableName);
                                                                checkCommand.Parameters.AddWithValue("@ColumnName", columnName);

                                                                int count = (int)await checkCommand.ExecuteScalarAsync();

                                                                if (count == 0) // If record doesn't exist, insert it
                                                                {
                                                                    using (SqlConnection insertConnection = new SqlConnection(appSettingsConnectionString))
                                                                    {
                                                                        await insertConnection.OpenAsync();
                                                                        string insertQuery = $"INSERT INTO dbo.TableColumnFunctionMapping VALUES (@TableName, @ColumnName, null, null, 0, getdate(), 0, 0, '', @ProjectId, 0)";
                                                                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, insertConnection))
                                                                        {
                                                                            insertCommand.Parameters.AddWithValue("@TableName", request.tableName);
                                                                            insertCommand.Parameters.AddWithValue("@ColumnName", columnName);
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

                                        success = true; // Set the success flag if all operations are successful
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
                // Handle the exception appropriately, such as logging or throwing a custom exception.
            }

            return success; // Return the success flag
        }

        public async Task<bool> CheckIsColumnDeleted(GetTableColumn request)
        {
            bool success = false; // Initialize the success flag

            try
            {
                string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
                string appSettingsConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));

                using (SqlConnection mainConnection = new SqlConnection(appSettingsConnectionString))
                {
                    await mainConnection.OpenAsync();
                    string savedcolumn = $"SELECT ColumnName FROM dbo.TableColumnFunctionMapping WHERE ProjectID = {request.ProjectId} and TableName = '{request.tableName}'";
                    string sqlQuery = "SELECT * FROM dbo.projects WHERE ProjectID = @ProjectID";

                    using (SqlCommand mainCommand = new SqlCommand(sqlQuery, mainConnection))
                    {
                        mainCommand.Parameters.AddWithValue("@ProjectID", request.ProjectId);

                        using (SqlDataReader mainReader = await mainCommand.ExecuteReaderAsync())
                        {
                            if (await mainReader.ReadAsync())
                            {
                                string serverName = (string)mainReader["ServerName"];
                                string databaseName = (string)mainReader["DatabaseName"];
                                string username = (string)mainReader["Username"];
                                string encryptedPassword = (string)mainReader["Password"];
                                //byte[] passwordBytes = Convert.FromBase64String(encryptedPassword);
                                //string password = Encoding.UTF8.GetString(passwordBytes);
                                string password = (string)mainReader["password"];
                                string dataType = (string)mainReader["DataType"];
                                string connectionString = $"Server={serverName};Database={databaseName};Uid={username};Pwd={password};";

                                if (dataType == "SQLSERVER")
                                {
                                    using (SqlConnection schemaConnection = new SqlConnection(connectionString))
                                    {
                                        await schemaConnection.OpenAsync();

                                        string schemaQuery = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@TableName";
                                        using (SqlCommand schemaCommand = new SqlCommand(schemaQuery, schemaConnection))
                                        {
                                            schemaCommand.Parameters.AddWithValue("@TableName", request.tableName);

                                            using (SqlDataReader schemaReader = await schemaCommand.ExecuteReaderAsync())
                                            {
                                                while (await schemaReader.ReadAsync())
                                                {
                                                    string columnName = schemaReader.GetString(0);

                                                    using (SqlConnection deleteConnection = new SqlConnection(appSettingsConnectionString))
                                                    {
                                                        await deleteConnection.OpenAsync();
                                                        string deleteQuery = $"DELETE FROM dbo.TableColumnFunctionMapping WHERE ProjectID = {request.ProjectId} AND TableName = '{request.tableName}' AND ColumnName = @ColumnName";
                                                        using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, deleteConnection))
                                                        {
                                                            deleteCommand.Parameters.AddWithValue("@ColumnName", columnName);
                                                            await deleteCommand.ExecuteNonQueryAsync();
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
                }

                success = true; // Set success flag to true at the end of the process
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                success = false;
            }

            return success;
        }

        public async Task<List<ColumnMapping>> GetMappedColumnNames(GetTableColumn request)
        {
            List<ColumnMapping> columnMappings = new List<ColumnMapping>();

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
                        SqlCommand command = new SqlCommand($"SELECT COLUMNNAME, IsSelected,FunctionId,ConstantValue FROM TableColumnFunctionMapping WHERE TableName = '{request.tableName}' AND ProjectId = {request.ProjectId}", connection);
                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            string columnName = reader.GetString(0);
                            int isSelected = reader.GetInt32(1);
                            int functionId = reader.GetInt32(2);
                            string constantValueJson = reader.GetString(3);

                            // Deserialize the entire JSON object
                            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(constantValueJson);

                            // Check if jsonObject is not null
                            if (jsonObject != null)
                            {
                                // Access the "ConstantValue" property
                                string constantValue = jsonObject.ContainsKey("ConstantValue") ? jsonObject["ConstantValue"]?.ToString() : null;

                                // Access the "certificateName" property
                                string certificateName = jsonObject.ContainsKey("CertificateName") ? jsonObject["CertificateName"]?.ToString() : null;

                                // Combine the values if they are not null
                                //string combinedConstantValue = !string.IsNullOrEmpty(constantValue) && !string.IsNullOrEmpty(certificateName)
                                //    ? $"{constantValue} - {certificateName}"
                                //    : constantValue ?? certificateName;

                                string combinedConstantValue = !string.IsNullOrEmpty(constantValue) && !string.IsNullOrEmpty(certificateName)
                                 ? $"{constantValue} + {certificateName}"
                                 : constantValue ?? certificateName;

                                ColumnMapping mapping = new ColumnMapping
                                {
                                    ColumnName = columnName,
                                    IsSelected = isSelected,
                                    FunctionId = functionId,
                                    ConstantValue = combinedConstantValue
                                };

                                columnMappings.Add(mapping);
                            }
                            else
                            {
                                ColumnMapping mapping = new ColumnMapping
                                {
                                    ColumnName = columnName,
                                    IsSelected = isSelected,
                                    FunctionId = functionId,
                                    ConstantValue = "",
                                };
                                columnMappings.Add(mapping);
                            }
                           
                        }
                        reader.Close();

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return columnMappings;
        }

        public async Task<List<FunctionInfo>> GetFunctionInfo(Update request)
        {
            // Read the connection string from the appsettings.json file
            string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
            string appSettingsConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));
            List<FunctionInfo> functionInfos = new List<FunctionInfo>();
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
                                string dataType = (string)reader["DataType"];
                                string connectionString = _configuration["AzureSettings:StorageConfig:StorageConnectionString"];
                                string containerName = _configuration["AzureSettings:StorageConfig:ContainerName"];
                                string blobFolderPath;
                                switch (dataType)
                                {
                                    case "SQLSERVER":
                                        blobFolderPath = "ObfuscationAsset/SQLDefinition/";
                                        break;
                                    case "MYSQL":
                                        blobFolderPath = "ObfuscationAsset/MYSQLDefinition/";
                                        break;
                                    case "ORACLE":
                                        blobFolderPath = "ObfuscationAsset/ORACLEDefinition/";
                                        break;
                                    default:
                                        // Handle the case when an unsupported database type is provided
                                        throw new ArgumentException("Invalid database type.");
                                }

                                try
                                {
                                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                                    CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                                    // List blobs in the specified folder
                                    var blobDirectory = container.GetDirectoryReference(blobFolderPath);
                                    var blobList = await blobDirectory.ListBlobsSegmentedAsync(null);

                                    foreach (var blob in blobList.Results)
                                    {
                                        if (blob is CloudBlockBlob blockBlob)
                                        {
                                            // Read the contents of each blob
                                            string fileContent = await blockBlob.DownloadTextAsync();

                                            if (!string.IsNullOrEmpty(fileContent))
                                            {
                                                // Add the blob content as a query to the functionInfos list
                                                string functionName = Path.GetFileNameWithoutExtension(blockBlob.Name);
                                                functionInfos.Add(new FunctionInfo(functionName, fileContent));
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
                    }
                }
            }
            return functionInfos;
        }

        public async Task<List<FunctionInfo>> GetViewInfo()
        {
            List<FunctionInfo> functionInfos = new List<FunctionInfo>();
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
                        // Retrieve view information
                        SqlCommand viewCommand = new SqlCommand("SELECT o.name AS ViewName, m.definition AS ViewDefinition FROM sys.objects o JOIN sys.sql_modules m ON o.object_id = m.object_id WHERE o.type = 'V' AND o.name = 'vwGetRANDValue'", connection);
                        SqlDataReader viewReader = await viewCommand.ExecuteReaderAsync();

                        while (await viewReader.ReadAsync())
                        {
                            string viewName = viewReader.GetString(0);
                            string viewDefinition = viewReader.GetString(1);
                            functionInfos.Add(new FunctionInfo(viewName, viewDefinition));
                        }

                        viewReader.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return functionInfos;
        }

        public async Task<List<Function>> GetFunctionNames()
        {
            List<Function> functionList = new List<Function>();
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
                        SqlCommand command = new SqlCommand("SELECT ID, Name,Names,DisplayName FROM dbo.Functions;", connection);
                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32(0);
                            string Name = reader.GetString(1);
                            string Names = reader.GetString(2);
                            string functionName = reader.GetString(3);
                            functionList.Add(new Function { ID = id, Name = Name, FunctionName = functionName, Names = Names });
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return functionList;
        }

    }
}


