using DataObfuscationApp.Model;
using DOA.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Models.Common;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Text;



namespace DOA.Repository.Implementation
{
    public class PreviewData : IPreviewData
    {
        private readonly IConfiguration _configuration;



        public PreviewData(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<string>> GetPreviewUpdate(Obfuscation request)
        {
            List<string> PreviewColumns = new List<string>();
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
                        SqlCommand command = new SqlCommand($"EXEC dbo.SpToPreviewData @TableName ='{request.tableName}',@ColumnName ='{request.ColumnName}',@FunctionName ='{request.FunctionName}',@ProjectID ='{request.ProjectId}',@DatabaseType ='{request.DataType}',@ConstantValue='{request.ConstantValue}'", connection);
                        SqlDataReader reader = await command.ExecuteReaderAsync();



                        while (await reader.ReadAsync())
                        {
                            string PreviewColumn = reader.GetString(0);
                            PreviewColumns.Add(PreviewColumn);
                        }



                        reader.Close();
                        //string updateQuery = command.Parameters["@UpdateStatement"].Value.ToString();
                        // Do whatever you want with the update query
                        //Console.WriteLine("Update : " + updateQuery);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return PreviewColumns;
        }





        public async Task<List<string>> ExecutePreviewQuery(SelectQuery request)
        {
            List<string> PreviewData = new List<string>();
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
                                            SqlCommand command1 = new SqlCommand($"{request.ExecuteSelectQuery}", connection1);
                                            SqlDataReader reader1 = await command1.ExecuteReaderAsync();



                                            while (await reader1.ReadAsync())
                                            {


                                                object originalData = reader1.GetValue(0);




                                                object value = reader1.GetValue(1); // Get the value as an object from the data reader
                                                string maskedData;

                                                if (value is int intvalue)
                                                {
                                                    maskedData = intvalue.ToString();
                                                }

                                                if (value is long intValue)
                                                {
                                                    // If the value is of type Int64, convert it to a string
                                                    maskedData = intValue.ToString();
                                                }
                                                else if (value is string stringValue)
                                                {
                                                    // If the value is already a string, use it directly
                                                    maskedData = stringValue;
                                                }
                                                else if (value == DBNull.Value)
                                                {
                                                    // Handle DBNull value if needed (optional)
                                                    maskedData = "NULL";
                                                }
                                                else
                                                {
                                                    // Handle other data types or throw an error if appropriate
                                                    throw new InvalidOperationException("Invalid data type encountered.");
                                                }



                                                // Now you can use the 'maskedData' variable as needed outside the if-else block
                                                PreviewData.Add($" {originalData},  {maskedData}");
                                            }
                                            reader1.Close();
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
                                            MySqlCommand command1 = new MySqlCommand($"{request.ExecuteSelectQuery}", connection1);
                                            MySqlDataReader reader1 = (MySqlDataReader)await command1.ExecuteReaderAsync();



                                            while (await reader1.ReadAsync())
                                            {
                                                string originalData = reader1.GetString(0);
                                                string maskedData = reader1.GetString(1);
                                                PreviewData.Add($"{originalData},  {maskedData}");
                                            }



                                            reader1.Close();
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
                                            OracleCommand command1 = new OracleCommand($"{request.ExecuteSelectQuery}", connection1);
                                            OracleDataReader reader1 = (OracleDataReader)await command1.ExecuteReaderAsync();



                                            while (await reader1.ReadAsync())
                                            {
                                                string columnName = reader1.GetString(0);
                                                PreviewData.Add(columnName);
                                            }



                                            reader1.Close();
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
                                    Console.WriteLine("Invalid column name.");
                                }
                            }
                        }
                    }
                }
            }
            return PreviewData;
        }
    }
}