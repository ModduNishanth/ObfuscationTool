using DataObfuscationApp.Model;
using DOA.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Models.Common;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace DOA.Repository.Implementation
{
    public class GetKeywordList : IGetKeywordList
    {
        private readonly IConfiguration _configuration;



        public GetKeywordList(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<KeywordList>> GetKeywordListAsync(Update request)
        {
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



                        string sqlQuery = $"SELECT kl.keyword,kl.ID,kl.FunctionId,f.DisplayName FROM dbo.KeywordList kl INNER JOIN dbo.Functions f ON kl.FunctionId = f.ID WHERE ProjectId = {request.ProjectId};";



                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            List<KeywordList> keywordList = new List<KeywordList>();



                            while (await reader.ReadAsync())
                            {
                                KeywordList keywordItem = new KeywordList
                                {
                                    KeyWords = reader.GetString(reader.GetOrdinal("Keyword")),
                                    KeywordId = reader.GetInt32(reader.GetOrdinal("ID")),
                                    FunctionId = reader.GetInt32(reader.GetOrdinal("FunctionId")),
                                    DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                };



                                keywordList.Add(keywordItem);
                            }



                            return keywordList;
                        }
                    }
                }
                else
                {
                    // Handle invalid connection string
                    return null; // or throw an exception
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                throw;
            }
        }


        public async Task<bool> UpdateKeywordList(KeywordList request)
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

                        string checkQuery = "SELECT COUNT(*) FROM dbo.KeywordList WHERE Keyword = @Keyword ";

                        using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                        {

                            checkCommand.Parameters.AddWithValue("@Keyword", request.KeyWords);

                            int existingCount = (int)await checkCommand.ExecuteScalarAsync();




                            if (existingCount > 0)
                            {
                                isSuccess = false; // Project, Server, or Database already exists.
                            }
                            else
                            {
                                // Step 2: Create an insert command and set parameters
                                SqlCommand insertCommand = new SqlCommand("INSERT INTO dbo.keywordList (Keyword, ProjectId, FunctionId) VALUES (@Keyword, @ProjectId, @FunctionId)", connection);
                                insertCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
                                insertCommand.Parameters.AddWithValue("@Keyword", request.KeyWords);  // Use the current keyword from the list
                                insertCommand.Parameters.AddWithValue("@FunctionId", request.FunctionId);



                                // Step 3: Execute the command
                                int rowsAffected = await insertCommand.ExecuteNonQueryAsync();



                                return rowsAffected > 0; // Return true if rows were affected (update successful), false otherwise
                            }
                           
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    isSuccess = false;  // Keyword update failed
                }
            }

            return isSuccess; // Invalid connection string
        }



        public async Task<bool> EditKeywordsAsync(KeywordList request)
        {
            bool isSuccess = false;
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

                        string checkQuery = "SELECT COUNT(*) FROM dbo.KeywordList WHERE Keyword = @Keyword AND ID = @KeywordId AND FunctionId=@FunctionId AND ProjectId=@ProjectId";


                        using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@ProjectId", request.ProjectId);
                            checkCommand.Parameters.AddWithValue("@Keyword", request.EditKeyword);
                            checkCommand.Parameters.AddWithValue("@FunctionId", request.FunctionId);

                            checkCommand.Parameters.AddWithValue("@KeywordId", request.KeywordId);


                            int existingCount = (int)await checkCommand.ExecuteScalarAsync();




                            if (existingCount > 0)
                            {
                                isSuccess = false; // Project, Server, or Database already exists.
                            }
                            else
                            {
                                string updateQuery = "UPDATE dbo.keywordList SET Keyword = @EditKeyword, FunctionId = @FunctionId WHERE ProjectId = @ProjectId AND ID = @KeywordId;";

                                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                                {
                                    // Use parameters to prevent SQL injection
                                    command.Parameters.AddWithValue("@EditKeyword", request.EditKeyword);
                                    command.Parameters.AddWithValue("@FunctionId", request.FunctionId);
                                    command.Parameters.AddWithValue("@ProjectId", request.ProjectId);
                                    command.Parameters.AddWithValue("@KeywordId", request.KeywordId);

                                    int rowsAffected = await command.ExecuteNonQueryAsync();

                                    // Return true if any rows were affected, otherwise false
                                    if (rowsAffected > 0)
                                    {
                                        isSuccess = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Handle invalid connection string format
                    // For example: throw new InvalidOperationException("Invalid connection string format");
                    // You can also log the error or take other appropriate actions.
                    return isSuccess; // or throw an exception
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                // Log the exception
                Console.WriteLine(ex.Message);

                // You can choose to rethrow the exception, return a default value, or throw a custom exception here
                // throw; // rethrow the exception if you want the caller to handle it
                // return false; // or return a default value
            }

            return isSuccess;
        }


        public async Task<int> DeleteKeywordsAsync(KeywordList request)
        {
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



                        string deleteQuery = $"DELETE FROM dbo.keywordList WHERE ProjectId = {request.ProjectId} AND Keyword ='{request.DeleteKeyword}';";



                        using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                        {
                            int rowsAffected = await command.ExecuteNonQueryAsync();
                            return rowsAffected;
                        }
                    }
                }
                else
                {
                    // Handle invalid connection string format
                    // For example: throw new InvalidOperationException("Invalid connection string format");
                    // You can also log the error or take other appropriate actions.
                    return 0; // or throw an exception
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                // For example: log the exception, return a default value, or throw a custom exception
                throw; // rethrow the exception
            }
        }



    }
}