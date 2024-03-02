using DataObfuscationApp.Model;
using DOA.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Amqp.Transaction;
using Microsoft.Extensions.Configuration;
using Models.Common;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;
using System.Text;



namespace DOA.Repository.Implementation
{
    public class ConnectionString : IConnectionString
    {
        private readonly IConfiguration _configuration;



        public ConnectionString(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public async Task<bool> CreateProject(CreateConnectionStringRequest request)
        {
            bool isSuccess = false;

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

                        // Step 2: Check if the project already exists

                         string checkQuery = "SELECT COUNT(*) FROM dbo.projects WHERE ServerName = @ServerName AND DatabaseName = @DatabaseName OR ProjectName = @ProjectName ";

                        using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@ServerName", request.ServerName);
                            checkCommand.Parameters.AddWithValue("@ProjectName", request.ProjectName);  

                            checkCommand.Parameters.AddWithValue("@DatabaseName", request.DatabaseName);
                            int existingCount = (int)await checkCommand.ExecuteScalarAsync();

                            if (existingCount > 0)
                            {
                                isSuccess = false; // Project, Server, or Database already exists.
                            }
                            else
                            {
                                // Step 3: Execute SQL query to insert the project
                                string insertQuery = "INSERT INTO dbo.projects (ProjectName, ProjectDescription, ServerName, DatabaseName, Username, Password, Datatype) " +
                                 "VALUES (TRIM(@ProjectName), TRIM(@ProjectDescription), TRIM(@ServerName), TRIM(@DatabaseName), TRIM(@Username), TRIM(@Password), TRIM(@Datatype))";


                                //string insertQuery = "INSERT INTO dbo.projects (ProjectName, ProjectDescription, ServerName, DatabaseName, Username, Password, Datatype) " +
                                //                      "VALUES (@ProjectName, @ProjectDescription, @ServerName, @DatabaseName, @Username, @Password, @Datatype)";
                                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                                {
                                    // Set the parameter values
                                    insertCommand.Parameters.AddWithValue("@ProjectName", request.ProjectName);
                                    insertCommand.Parameters.AddWithValue("@ProjectDescription", request.ProjectDescription);
                                    insertCommand.Parameters.AddWithValue("@ServerName", request.ServerName);
                                    insertCommand.Parameters.AddWithValue("@DatabaseName", request.DatabaseName);
                                    insertCommand.Parameters.AddWithValue("@Username", request.Username);
                                    // Encrypt the password using Base64 encoding
                                    //string encryptedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Password));
                                    //insertCommand.Parameters.AddWithValue("@Password", encryptedPassword);
                                    insertCommand.Parameters.AddWithValue("@Datatype", request.Datatype);
                                    insertCommand.Parameters.AddWithValue("@Password", request.Password);

                                      // Step 4: Execute the insert statement
                                    int rowsAffected = await insertCommand.ExecuteNonQueryAsync();
                                    isSuccess = rowsAffected > 0; // Set isSuccess based on the rows affected
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle the exception
                    Console.WriteLine("An error occurred: " + ex.Message);
                    isSuccess = false; // An error occurred while creating the project.
                }
            }



            return isSuccess;
        }



        public async Task<bool> EditProject(int projectId, CreateConnectionStringRequest request)
        {
            bool isSuccess = false;

            string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
            string connectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));

            if (connectionString.StartsWith("Server="))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                       
                        //using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                        //{
                        //    checkCommand.Parameters.AddWithValue("@ProjectName", request.ProjectName);

                        //    int existingCount = (int)await checkCommand.ExecuteScalarAsync();



                        //    if (existingCount > 0)
                        //    {
                        //        isSuccess = false; // Project, Server, or Database already exists.
                        //    }
                        //    else
                        //    {
                        //        string sqlQuery = "UPDATE dbo.projects SET ProjectName = @ProjectName, " +
                        //                  "ProjectDescription = @ProjectDescription, " +
                        //                  "ServerName = @ServerName, " +
                        //                  "DatabaseName = @DatabaseName, " +
                        //                  "Username = @Username, " +
                        //                  "Password = @Password, " +
                        //                  "Datatype = @Datatype " +
                        //                  "WHERE ProjectId = @ProjectId";

                        //        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        //        {
                        //            // Set the parameter values
                        //            command.Parameters.AddWithValue("@ProjectName", request.ProjectName);
                        //            command.Parameters.AddWithValue("@ProjectDescription", request.ProjectDescription);
                        //            command.Parameters.AddWithValue("@ServerName", request.ServerName);
                        //            command.Parameters.AddWithValue("@DatabaseName", request.DatabaseName);
                        //            command.Parameters.AddWithValue("@Username", request.Username);

                        //            // Decode the existing password from Base64
                        //            string encryptedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Password));
                        //            command.Parameters.AddWithValue("@Password", Convert.ToBase64String(Encoding.UTF8.GetBytes(encryptedPassword)));

                        //            command.Parameters.AddWithValue("@Datatype", request.Datatype);
                        //            command.Parameters.AddWithValue("@ProjectId", projectId);

                        //            int rowsAffected = await command.ExecuteNonQueryAsync();

                        //            if (rowsAffected > 0)
                        //            {
                        //                isSuccess = true;
                        //            }
                        //        }
                        //    }
                        //}


                            // Check if there are any projects with the same name excluding the current project
                            string checkQuery = "SELECT COUNT(*) FROM dbo.projects WHERE (ServerName = @ServerName AND DatabaseName = @DatabaseName) OR ProjectName = @ProjectName AND ProjectId != @ProjectId";

                            using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                            {
                            checkCommand.Parameters.AddWithValue("@ServerName", request.ServerName);
                            checkCommand.Parameters.AddWithValue("@DatabaseName", request.DatabaseName);
                            checkCommand.Parameters.AddWithValue("@ProjectName", request.ProjectName);
                                checkCommand.Parameters.AddWithValue("@ProjectId", projectId); // Assuming projectId is the current project's id

                                int existingCount = (int)await checkCommand.ExecuteScalarAsync();

                                if (existingCount > 0)
                                {
                                    isSuccess = false; // Project with the same name already exists.
                                }
                                else
                                {
                                    // Update query based on ProjectId
                                    string sqlQuery = "UPDATE dbo.projects SET ProjectName = @ProjectName, " +
                                                      "ProjectDescription = @ProjectDescription, " +
                                                      "ServerName = @ServerName, " +
                                                      "DatabaseName = @DatabaseName, " +
                                                      "Username = @Username, " +
                                                      "Password = @Password, " +
                                                      "Datatype = @Datatype " +
                                                      "WHERE ProjectId = @ProjectId";

                                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                                    {
                                        // Set the parameter values
                                        command.Parameters.AddWithValue("@ProjectName", request.ProjectName);
                                        command.Parameters.AddWithValue("@ProjectDescription", request.ProjectDescription);
                                        command.Parameters.AddWithValue("@ServerName", request.ServerName);
                                        command.Parameters.AddWithValue("@DatabaseName", request.DatabaseName);
                                        command.Parameters.AddWithValue("@Username", request.Username);
                                        command.Parameters.AddWithValue("@Password", request.Password);


                                    // Decode the existing password from Base64
                                    //string encryptedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Password));

                                    //// Truncate the password if it exceeds the maximum length allowed by the column
                                    //int maxPasswordLength = 50; // replace with the actual maximum length
                                    //if (encryptedPassword.Length > maxPasswordLength)
                                    //{
                                    //    encryptedPassword = encryptedPassword.Substring(0, maxPasswordLength);
                                    //}

                                    //string existingPassword = Encoding.UTF8.GetString(Convert.FromBase64String(request.Password));

                                    ////command.Parameters.AddWithValue("@Password", Convert.ToBase64String(Encoding.UTF8.GetBytes(existingPassword)));


                                    //command.Parameters.AddWithValue("@Password", existingPassword);
                                        command.Parameters.AddWithValue("@Datatype", request.Datatype);
                                        command.Parameters.AddWithValue("@ProjectId", projectId);

                                        int rowsAffected = await command.ExecuteNonQueryAsync();

                                        if (rowsAffected > 0)
                                        {
                                            isSuccess = true;
                                        }
                                    }
                                }
                            }
                        

                    }




                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return isSuccess;
        }




        public async Task<ResponseModel<bool>> TestAndCheckConnectionString(int projectId)
        {
            ResponseModel<bool> errorMessage = new ResponseModel<bool>();



            try
            {
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
                            command.Parameters.AddWithValue("@ProjectID", projectId);



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
                                    string password = (string)reader["Password"];



                                    // Decrypt the password using Base64 decoding
                                    //byte[] passwordBytes = Convert.FromBase64String(encryptedPassword);
                                    //string password = Encoding.UTF8.GetString(passwordBytes);
                                    string dataType = (string)reader["DataType"];
                                    //string IP_address = (string)reader["IP_address"];
                                    //string Service_listener = (string)reader["Service_listener"];

                                    string connectionString = $"Server={serverName};Database={databaseName};Uid={username};Pwd={password};";
                                    string mysqlconnectionString = $"server={serverName};uid={username};pwd={password};database={databaseName};port=3306";

                                    string oracleConnectionString = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={"192.168.1.100"})(PORT=3306))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={"ORCL"})));User Id={username};Password={password};";

                                    //string oracleconnectionstring = $"server={serverName};uid={username};pwd={password};database={databaseName};port=3306";


                                    if (dataType == "SQLSERVER")
                                    {
                                        using (SqlConnection connection2 = new SqlConnection(connectionString))
                                        {
                                            try
                                            {
                                                connection2.Open();
                                                string query = "SELECT 1";
                                                using (SqlCommand command2 = new SqlCommand(query, connection2))
                                                {
                                                    command2.ExecuteNonQuery();
                                                }
                                                connection2.Close();



                                                // Update TestConnection to 1 as connection and query are successful
                                                using (SqlConnection updateConnection = new SqlConnection(appSettingsConnectionString))
                                                {
                                                    await updateConnection.OpenAsync();
                                                    string updateQuery = "UPDATE [dbo].[Projects] SET [TestConnection] = 1 WHERE ProjectID = @ProjectID";



                                                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, updateConnection))
                                                    {
                                                        updateCommand.Parameters.AddWithValue("@ProjectID", projectId);
                                                        await updateCommand.ExecuteNonQueryAsync();
                                                    }
                                                }



                                                errorMessage = new ResponseModel<bool>()
                                                {
                                                    IsError = true,
                                                    Message = "Connection successful."
                                                };
                                            }
                                            catch (Exception ex)
                                            {
                                                connection2.Close();
                                                // Update TestConnection to 2 as there was an error
                                                using (SqlConnection updateConnection = new SqlConnection(appSettingsConnectionString))
                                                {
                                                    await updateConnection.OpenAsync();
                                                    string updateQuery = "UPDATE [dbo].[Projects] SET [TestConnection] = 2 WHERE ProjectID = @ProjectID";



                                                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, updateConnection))
                                                    {
                                                        updateCommand.Parameters.AddWithValue("@ProjectID", projectId);
                                                        await updateCommand.ExecuteNonQueryAsync();
                                                    }
                                                }



                                                errorMessage = new ResponseModel<bool>()
                                                {
                                                    IsError = false,
                                                    Message = ex.Message
                                                };
                                            }
                                        }
                                    }
                                    else if (dataType == "MYSQL")
                                    {
                                        // Step 1: Establish a MySQL connection
                                        using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlconnectionString))
                                        {
                                            try
                                            {
                                                await mySqlConnection.OpenAsync();
                                                string query = "SELECT 1";
                                                using (MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection))
                                                {
                                                    await mySqlCommand.ExecuteNonQueryAsync();
                                                }
                                                mySqlConnection.Close();



                                                // Update TestConnection to 1 as connection and query are successful
                                                using (SqlConnection updateConnection = new SqlConnection(appSettingsConnectionString))
                                                {
                                                    await updateConnection.OpenAsync();
                                                    string updateQuery = "UPDATE [dbo].[Projects] SET [TestConnection] = 1 WHERE ProjectID = @ProjectID";



                                                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, updateConnection))
                                                    {
                                                        updateCommand.Parameters.AddWithValue("@ProjectID", projectId);
                                                        await updateCommand.ExecuteNonQueryAsync();
                                                    }
                                                }



                                                errorMessage = new ResponseModel<bool>()
                                                {
                                                    IsError = true,
                                                    Message = "Connection successful."
                                                };
                                            }
                                            catch (Exception ex)
                                            {
                                                // Update TestConnection to 2 as there was an error
                                                using (SqlConnection updateConnection = new SqlConnection(appSettingsConnectionString))
                                                {
                                                    await updateConnection.OpenAsync();
                                                    string updateQuery = "UPDATE [dbo].[Projects] SET [TestConnection] = 2 WHERE ProjectID = @ProjectID";



                                                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, updateConnection))
                                                    {
                                                        updateCommand.Parameters.AddWithValue("@ProjectID", projectId);
                                                        await updateCommand.ExecuteNonQueryAsync();
                                                    }
                                                }



                                                errorMessage = new ResponseModel<bool>()
                                                {
                                                    IsError = false,
                                                    Message = ex.Message
                                                };
                                            }
                                        }
                                    }
                                    else if (dataType == "ORACLE")
                                    {
                                        // Step 1: Establish an Oracle connection
                                        using (OracleConnection oracleConnection = new OracleConnection(oracleConnectionString))
                                        {
                                            try
                                            {
                                                await oracleConnection.OpenAsync();
                                                string query = "SELECT 1";
                                                using (OracleCommand oracleCommand = new OracleCommand(query, oracleConnection))
                                                {
                                                    await oracleCommand.ExecuteNonQueryAsync();
                                                }
                                                oracleConnection.Close();

                                                // Update TestConnection to 1 as connection and query are successful
                                                using (OracleConnection updateConnection = new OracleConnection(appSettingsConnectionString))
                                                {
                                                    await updateConnection.OpenAsync();
                                                    string updateQuery = "UPDATE Projects SET TestConnection = 1 WHERE ProjectID = :ProjectID";

                                                    using (OracleCommand updateCommand = new OracleCommand(updateQuery, updateConnection))
                                                    {
                                                        updateCommand.Parameters.Add(new OracleParameter(":ProjectID", projectId));
                                                        await updateCommand.ExecuteNonQueryAsync();
                                                    }
                                                }

                                                errorMessage = new ResponseModel<bool>()
                                                {
                                                    IsError = true,
                                                    Message = "Connection successful."
                                                };
                                            }
                                            catch (Exception ex)
                                            {
                                                // Update TestConnection to 2 as there was an error
                                                using (SqlConnection updateConnection = new SqlConnection(appSettingsConnectionString))
                                                {
                                                    await updateConnection.OpenAsync();
                                                    string updateQuery = "UPDATE [dbo].[Projects] SET [TestConnection] = 2 WHERE ProjectID = @ProjectID";



                                                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, updateConnection))
                                                    {
                                                        updateCommand.Parameters.AddWithValue("@ProjectID", projectId);
                                                        await updateCommand.ExecuteNonQueryAsync();
                                                    }
                                                }



                                                errorMessage = new ResponseModel<bool>()
                                                {
                                                    IsError = false,
                                                    Message = ex.Message
                                                };
                                            }
                                        }
                                    }

                                    else
                                    {
                                        errorMessage = new ResponseModel<bool>()
                                        {
                                            IsError = true,
                                            Message = "Invalid data type."
                                        };
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    errorMessage = new ResponseModel<bool>()
                    {
                        IsError = true,
                        Message = "Invalid connection string."
                    };
                }
            }
            catch (Exception ex)
            {
                errorMessage = new ResponseModel<bool>()
                {
                    IsError = true,
                    Message = ex.Message
                }; ;
            }



            return errorMessage;
        }



        public async Task<List<CreateConnectionStringRequest>> GetAllProjects()
        {
            List<CreateConnectionStringRequest> projects = new List<CreateConnectionStringRequest>();
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



                        // Step 2: Retrieve all projects
                        string query = "SELECT ProjectID, ProjectName, ProjectDescription, ServerName, DatabaseName, Username, Password, DataType,TestConnection FROM dbo.projects";



                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            // Step 3: Execute the SQL query and retrieve the project data
                            using (SqlDataReader reader = await command.ExecuteReaderAsync())
                            {
                                while (reader.Read())
                                {
                                    CreateConnectionStringRequest project = new CreateConnectionStringRequest
                                    {
                                        ProjectId = reader.GetInt32(0),
                                        ProjectName = reader.GetString(1),
                                        ProjectDescription = reader.GetString(2),
                                        ServerName = reader.GetString(3),
                                        DatabaseName = reader.GetString(4),
                                        Username = reader.GetString(5),
                                        Password = reader.GetString(6),
                                        Datatype = reader.GetString(7),
                                        TestConnection = reader.GetInt32(8)
                                    };



                                    projects.Add(project);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle the exception
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }



            return projects;
        }

        ////////////////////////////////////

        //public async Task<bool> DeleteProject(Update request)
        //{
        //    // Read the connection string from the appsettings.json file
        //    string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
        //    string connectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));

        //    if (connectionString.StartsWith("Server="))
        //    {
        //        try
        //        {
        //            // Step 1: Establish a connection
        //            using (SqlConnection connection = new SqlConnection(connectionString))
        //            {
        //                await connection.OpenAsync();

        //                // Step 2: Check if the ProjectId exists in TableColumnFunctionMapping
        //                string selectQuery = "SELECT COUNT(*) FROM dbo.TableColumnFunctionMapping WHERE ProjectId = @ProjectId";
        //                using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
        //                {
        //                    selectCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
        //                    int projectUsageCount = (int)await selectCommand.ExecuteScalarAsync();

        //                    // Step 3: Check if TestConnection is 1
        //                    bool isTestConnection = false;
        //                    string testConnectionQuery = "SELECT TestConnection FROM dbo.Projects WHERE ProjectId = @ProjectId";
        //                    using (SqlCommand testConnectionCommand = new SqlCommand(testConnectionQuery, connection))
        //                    {
        //                        testConnectionCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
        //                        object testConnectionResult = await testConnectionCommand.ExecuteScalarAsync();
        //                        if (testConnectionResult != null && testConnectionResult != DBNull.Value)
        //                        {
        //                            isTestConnection = Convert.ToInt32(testConnectionResult) == 1;
        //                        }
        //                    }

        //                    // Step 4: Perform deletion based on stored procedure logic
        //                    if (projectUsageCount > 0 || isTestConnection)
        //                    {
        //                        // ProjectId exists in TableColumnFunctionMapping or TestConnection is 1, do not delete the project
        //                        return false;
        //                    }
        //                    else
        //                    {
        //                        // Proceed with deletion
        //                        SqlCommand deleteCommand = new SqlCommand("DELETE FROM dbo.Projects WHERE ProjectId = @ProjectId;", connection);
        //                        deleteCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
        //                        int rowsAffected = await deleteCommand.ExecuteNonQueryAsync();

        //                        if (rowsAffected > 0)
        //                        {
        //                            return true;  // Project deleted successfully.
        //                        }
        //                        else
        //                        {
        //                            return false; // Project not found.
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //            return false; // Project deletion failed.
        //        }
        //    }
        //    return false; // Invalid connection string.
        //}


        ///////////////////////////////////

        public async Task<bool> DeleteProject(Update request)
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
                        string selectQuery = "SELECT COUNT(*) FROM dbo.TableColumnFunctionMapping WHERE ProjectId = @ProjectId";
                        using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
                            int result = (int)await selectCommand.ExecuteScalarAsync();

                            if (result > 0)
                            {
                                // ProjectId exists in TableColumnFunctionMapping, so do not delete the project
                                return false;
                            }
                        }

                        // Step 3: Delete the project if it's not used in TableColumnFunctionMapping
                        SqlCommand deleteCommand = new SqlCommand("DELETE FROM dbo.Projects WHERE ProjectId = @ProjectId;", connection);
                        deleteCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
                        int rowsAffected = await deleteCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return true;  // Project deleted successfully.
                        }
                        else
                        {
                            return false; // Project not found or not eligible for deletion.
                        }
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


        public async Task<List<GetServerTypeName>> GetServerType()
        {
            List<GetServerTypeName> GetServerTypes = new List<GetServerTypeName>();
            try
            {
                // Read the connection string from the appsettings.json file
                string base64EncodedConnectionString = _configuration.GetConnectionString("KalpitaObfuscation_Dev");
                string connectionString = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedConnectionString));

                if (connectionString.StartsWith("Server="))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        string query = "SELECT ServerTypeId,ServerTypeName FROM [dbo].[ServerType]";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            using (SqlDataReader reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    GetServerTypeName serverType = new GetServerTypeName
                                    {
                                        // Map the columns from the database to the properties of the GetServerTypeName object.
                                        // For example:
                                        ServerTypeId = reader.GetInt32(0),
                                        ServerTypeName = reader.GetString(1),
                                    };

                                    GetServerTypes.Add(serverType);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions or log them as needed.
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return GetServerTypes;
        }


}
}