using DataObfuscationApp.Model;
using DOA.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Models.Common;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOA.Repository.Implementation
{
    public class ExecuteObfuscationSp : IExecuteObfuscationSp
    {
        private readonly IConfiguration _configuration;

        public ExecuteObfuscationSp(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<ExecutionResult> ExecUpdateQueryObfuscation(ExecUpdateQueryObfuscationModel request)
        {
            ExecutionResult result = new ExecutionResult();
            List<string> ExecuteSpQuery = new List<string>();
            string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
            string connectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));
            if (connectionString.StartsWith("Server="))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();





                        string[] tableNames = request.TableName.Split(',');

                        foreach (string tableName in tableNames)
                        {
                            SqlCommand command = new SqlCommand($"EXEC [dbo].[spGetObfuscateDataQuery] @ProjectID = {request.ProjectId}, @TablesName = '{tableName}'", connection);
                            SqlDataReader reader = await command.ExecuteReaderAsync();
                            while (await reader.ReadAsync())
                            {
                                string executeQuery = reader.GetString(1);
                                ExecuteSpQuery.Add(executeQuery);
                            }
                            reader.Close();
                        }
                        foreach (string tableName in tableNames)
                        {
                            try
                            {
                                
                                    string sqlQuery = "SELECT * FROM dbo.projects WHERE ProjectID = @ProjectID" ;
                                    using (SqlCommand command1 = new SqlCommand(sqlQuery, connection))
                                    {
                                        command1.Parameters.AddWithValue("@ProjectID", request.ProjectId);
                                        using (SqlDataReader reader1 = await command1.ExecuteReaderAsync())
                                        {
                                            if (await reader1.ReadAsync())
                                            {
                                                string serverName = (string)reader1["ServerName"];
                                                string databaseName = (string)reader1["DatabaseName"];
                                                string username = (string)reader1["Username"];
                                                string encryptedPassword = (string)reader1["Password"];
                                            //byte[] passwordBytes = Convert.FromBase64String(encryptedPassword);
                                            //string password = Encoding.UTF8.GetString(passwordBytes);
                                            string password = (string)reader1["password"];
                                            string dataType = (string)reader1["DataType"];
                                                string connectionString1 = $"Server={serverName};Database={databaseName};Uid={username};Pwd={password};";
                                                string mysqlconnectionString = $"server={serverName};uid={username};pwd={password};database={databaseName};port=3306";
                                                string oracleconnectionstring = $"server={serverName};uid={username};pwd={password};database={databaseName};port=3306";

                                                List<string> ObfuscatedColumn = new List<string>();
                                                foreach (var query in ExecuteSpQuery)
                                                {
                                                    if (dataType == "SQLSERVER")
                                                    {
                                                        using (SqlConnection connection1 = new SqlConnection(connectionString1))
                                                        {
                                                            try
                                                            {
                                                                await connection1.OpenAsync();


                                                                using ( SqlCommand command2 = new SqlCommand(query, connection1))
                                                                {
                                                                    command2.CommandTimeout = 1200; // Set timeout to 20 for no timeout

                                                                    using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                                                                    {
                                                                        while (await reader2.ReadAsync())
                                                                        {
                                                                            string executeQuery1 = reader2.GetString(1);
                                                                            ObfuscatedColumn.Add(executeQuery1);
                                                                            result.IsSuccessful = true;
                                                                            result.TableName = tableName;

                                                                        break;
                                                                        }
                                                                    }
                                                                }

                                                            }

                                                            catch (Exception ex)
                                                            {
                                                                reader1.Close();
                                                                Console.WriteLine(ex.Message);
                                                                result.IsSuccessful = false; // Set IsSuccessful to false for this table
                                                                result.ErrorMessage = ex.Message;
                                                                result.TableName = tableName;
                                                            SqlCommand command = new SqlCommand($"Update TableColumnFunctionMapping Set IsObfuscated =0 where TableName ='{result.TableName} ' And  ProjectID =  {request.ProjectId} ", connection);
                                                            SqlDataReader reader3 = await command.ExecuteReaderAsync();     
                                                            reader3.Close();
                                                            break;
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
                                                                MySqlCommand command2 = new MySqlCommand(query, connection1);

                                                                // Set the command timeout before executing the command
                                                                command2.CommandTimeout = 1200;

                                                                MySqlDataReader reader2 = (MySqlDataReader)await command2.ExecuteReaderAsync();
                                                                while (await reader2.ReadAsync())
                                                                {
                                                                    string executeQuery1 = reader2.GetString(0);
                                                                    ObfuscatedColumn.Add(executeQuery1);
                                                                    result.IsSuccessful = true;
                                                                    result.TableName = tableName;
                                                                    break;
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                Console.WriteLine(ex.Message);
                                                                result.ErrorMessage = ex.Message;
                                                                result.IsSuccessful = false; // Set IsSuccessful to false for this table
                                                                result.TableName = tableName;
                                                                // Handle the exception appropriately
                                                            }
                                                        }
                                                    }

                                                    else if (dataType == "ORACLE ")
                                                    {
                                                        using (OracleConnection connection1 = new OracleConnection(oracleconnectionstring))
                                                        {
                                                            try
                                                            {
                                                                await connection1.OpenAsync();
                                                                OracleCommand command2 = new OracleCommand(query, connection1);
                                                                command2.CommandTimeout = 1200;
                                                                OracleDataReader reader2 = (OracleDataReader)await command2.ExecuteReaderAsync();
                                                                while (await reader2.ReadAsync())
                                                                {
                                                                    string executeQuery1 = reader2.GetString(0);
                                                                    ObfuscatedColumn.Add(executeQuery1);
                                                                    result.IsSuccessful = true;
                                                                    result.TableName = tableName;
                                                                    break;
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                Console.WriteLine(ex.Message);
                                                                result.ErrorMessage = ex.Message;
                                                                result.IsSuccessful = false; // Set IsSuccessful to false for this table
                                                                result.TableName = tableName;
                                                                break;



                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Invalid column name.");
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                        
                                    }
                                
                               
                                if (!result.IsSuccessful)
                                {
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                // Handle the exception here
                                Console.WriteLine($"An error occurred while executing query for table {tableName}: {ex.Message}");
                                // You can also log the error or take other appropriate actions
                            }
                            break;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    result.ErrorMessage = ex.Message;
                    return result;
                }
            }
            return result;
        }

        public async Task<bool> CheckFunctionInfo(FunctionCheck request)
        {
            List<string> MissingFunctionNames = new List<string>();
            //List<string> ExistingFunctionNames = new List<string>();
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

                                if (dataType == "SQLSERVER")
                                {
                                    using (SqlConnection connection1 = new SqlConnection(connectionString))
                                    {
                                        try
                                        {
                                            await connection1.OpenAsync();
                                            SqlCommand command1 = new SqlCommand("SELECT o.name AS FunctionName FROM sys.objects o JOIN sys.sql_modules m ON o.object_id = m.object_id WHERE o.type = 'FN'", connection1);
                                            SqlDataReader reader1 = await command1.ExecuteReaderAsync();

                                            while (await reader1.ReadAsync())
                                            {
                                                string functionName = reader1.GetString(0);
                                                MissingFunctionNames.Add(functionName);
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
                                    using (MySqlConnection connection1 = new MySqlConnection(connectionString))
                                    {
                                        try
                                        {
                                            await connection.OpenAsync();
                                            MySqlCommand command1 = new MySqlCommand("SELECT o.name AS FunctionName FROM sys.objects o JOIN sys.sql_modules m ON o.object_id = m.object_id WHERE o.type = 'FN'", connection1);
                                            MySqlDataReader reader1 = (MySqlDataReader)await command1.ExecuteReaderAsync();

                                            while (await reader1.ReadAsync())
                                            {
                                                string functionName = reader.GetString(0);
                                                MissingFunctionNames.Add(functionName);
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
                                    using (OracleConnection connection1 = new OracleConnection(connectionString))
                                    {
                                        try
                                        {
                                            await connection1.OpenAsync();
                                            OracleCommand command2 = new OracleCommand("SELECT object_name AS FunctionName FROM all_objects WHERE object_type = 'FUNCTION'", connection1);
                                            OracleDataReader reader2 = (OracleDataReader)await command2.ExecuteReaderAsync();

                                            while (await reader2.ReadAsync())
                                            {
                                                string functionName = reader2.GetString(0);
                                                MissingFunctionNames.Add(functionName);
                                            }

                                            reader2.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                    }
                                }
                                // Read the folder path from the appsettings.json file
                                string connectionString1 = _configuration["AzureSettings:StorageConfig:StorageConnectionString"];
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
                                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString1);
                                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                                    CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                                    // List blobs in the specified folder
                                    var blobDirectory = container.GetDirectoryReference(blobFolderPath);
                                    var blobList = await blobDirectory.ListBlobsSegmentedAsync(null);

                                    // Create a list to store existing function names from Azure Blob Storage
                                    List<string> ExistingFunctionNames = new List<string>();

                                    int count = 0;
                                    foreach (var blob in blobList.Results)
                                    {
                                        if (blob is CloudBlockBlob blockBlob)
                                        {
                                            // Get the file name (function name) without the folder path
                                            string fileName = Path.GetFileNameWithoutExtension(blockBlob.Name);

                                            // Add the file name (function name) to the list of existing function names
                                            ExistingFunctionNames.Add(fileName);

                                            // Increment the count to limit the number of file names retrieved
                                            count++;

                                            // If you have retrieved the first six file names, break out of the loop
                                            if (count >= 6)
                                                break;
                                        }
                                    }

                                    // Now, you have the ExistingFunctionNames list containing function names from Azure Blob Storage
                                    // You can continue with the rest of your code:
                                    var finalMissingFunctionNames = MissingFunctionNames.OrderBy(n => n);
                                    var finalExistingFunctionNames = ExistingFunctionNames.OrderBy(n => n);

                                    bool haveSameFunctions = finalMissingFunctionNames.All(finalMissingFunctionNames.Contains) &&
                                                             finalExistingFunctionNames.All(finalExistingFunctionNames.Contains);

                                    return haveSameFunctions;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    // Handle any exception that might occur while reading from Azure Blob Storage
                                    // You can choose to throw or return false here based on your requirement.
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public async Task<bool> UpdateObfuscateColumn(Update update)
        {
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

                        SqlCommand command = new SqlCommand($"UPDATE dbo.TableColumnFunctionMapping SET IsObfuscated = 0 WHERE ProjectID ={update.ProjectId}  AND EXISTS ( SELECT 1 FROM dbo.TableColumnFunctionMapping where ProjectID ={update.ProjectId} AND IsObfuscated = 1) ;", connection);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0) {
                            return true;
                        }
                        Console.WriteLine($"Updated {rowsAffected} rows.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return false;
        }


        public async Task<bool> DeleteTable(Deletetable request)
        {

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



                        // Step 2: Check if the ProjectId exists in TableColumnFunctionMapping
                        string selectQuery = "SELECT ProjectId FROM dbo.TableColumnFunctionMapping WHERE ProjectId = @ProjectId";
                        using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
                            int result = await selectCommand.ExecuteNonQueryAsync();



                            if (result == null)
                            {
                                // ProjectId exists in TableColumnFunctionMapping, so do not delete the project
                                return false;
                            }
                        }
                        string[] tableNames = request.TableName.Split(',');
                        foreach (string TableName in tableNames)
                        {

                            // Step 3: Delete the tables
                            

                            SqlCommand deleteCommand = new SqlCommand($"DELETE FROM dbo.TableColumnFunctionMapping WHERE ProjectId = @ProjectId AND TableName = @TableName", connection);
                            deleteCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
                            deleteCommand.Parameters.AddWithValue("@TableName", TableName);

                            int rowsAffected = await deleteCommand.ExecuteNonQueryAsync();



                            if (rowsAffected <= 0)
                            {
                                return false;  
                            }
                            
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false; // Project deletion failed.
                }
            }
            return false; // Invalid connection string.
        }
    }
}


