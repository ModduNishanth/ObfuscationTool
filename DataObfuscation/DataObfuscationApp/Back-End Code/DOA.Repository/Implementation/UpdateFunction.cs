using DOA.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Models.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;



namespace DOA.Repository.Implementation
{
    public class UpdateFunction : IUpdateFunction
    {
        private readonly IConfiguration _configuration;



        public UpdateFunction(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> CreateFunctions(UpdateFunctions request)
        {
            bool success = false; // Initialize the success flag



            // Read the connection string from the appsettings.json file
            string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
            string connectionString1 = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));



            if (connectionString1.StartsWith("Server="))
            {
                // Step 1: Establish a connection
                using (SqlConnection connection = new SqlConnection(connectionString1))
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





                                // The path to the folder where you want to save the .json file for SQL Server
                                string blobConnectionString = _configuration["AzureSettings:StorageConfig:StorageConnectionString"];
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
                                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobConnectionString);
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
                                            string[] scriptStatements = fileContent.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                                            string[] mysqlscriptStatements = fileContent.Split(new[] { "DELIMETER" }, StringSplitOptions.RemoveEmptyEntries);





                                            if (dataType == "SQLSERVER")
                                            {
                                                using (SqlConnection connection1 = new SqlConnection(connectionString))
                                                {
                                                    await connection1.OpenAsync();



                                                    foreach (string statement in scriptStatements)
                                                    {
                                                        using (SqlCommand command1 = new SqlCommand(statement, connection1))
                                                        {
                                                            try
                                                            {
                                                                // Execute each statement separately
                                                                await command1.ExecuteNonQueryAsync();
                                                                success = true; // Set the success flag to true
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                // Handle the exception
                                                                success = false; // Set the success flag to false
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (dataType == "MYSQL")
                                            {
                                                using (MySqlConnection connection1 = new MySqlConnection(mysqlconnectionString))
                                                {
                                                    await connection1.OpenAsync();
                                                    foreach (string statement in mysqlscriptStatements)
                                                    {
                                                        using (MySqlCommand command1 = new MySqlCommand(statement, connection1))
                                                        {
                                                            try
                                                            {
                                                                // Execute each statement separately
                                                                await command1.ExecuteNonQueryAsync();
                                                                success = true; // Set the success flag to true
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                // Handle the exception
                                                                success = false; // Set the success flag to false
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (dataType == "ORACLE")
                                            {
                                                using (OracleConnection connection1 = new OracleConnection(oracleconnectionString))
                                                {
                                                    await connection1.OpenAsync();
                                                    foreach (string statement in scriptStatements)
                                                    {
                                                        using (OracleCommand command1 = new OracleCommand(statement, connection1))
                                                        {
                                                            try
                                                            {
                                                                // Execute each statement separately
                                                                await command1.ExecuteNonQueryAsync();
                                                                success = true; // Set the success flag to true
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                // Handle the exception
                                                                success = false; // Set the success flag to false
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                success = false;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    success = false;
                                }
                            }
                        }
                    }
                }
            }
            return success;
        }



        public async Task<bool> CopyProject(CopyProject copyProject)
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
                        SqlCommand command = new SqlCommand($"EXEC dbo.spCopyProjectMapping  @ProjectId = '{copyProject.ProjectId}', @ProjectIdFrom = {copyProject.ProjectIdfrom};", connection);
                        await command.ExecuteNonQueryAsync();
                    }
                    return true; // Query executed successfully
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return false; // Query execution failed
        }
    }
}