using uzbaseQuiz.Models;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace uzbaseQuiz.Repositories
{
    public class UserRepository : IUserRepository
    {
         private readonly NpgsqlConnection _npgsqlConnection;
        public UserRepository(string connectionString)
            => _npgsqlConnection = new NpgsqlConnection(connectionString);


        public async Task<long> DeleteUser(long user_id)
        {
              var sqlQuery = @"DELETE FROM users
                             WHERE user_id = @user_id;";

            using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@user_id", NpgsqlDbType.Integer, user_id);
                await _npgsqlConnection.OpenAsync();
                var changedRowCount = await command.ExecuteNonQueryAsync();

                if (changedRowCount == 1)
                    return user_id;
                else
                    throw new Exception("Delete qilish jarayonida kutilmagan xatolik.");
            }
        }

        // public async Task<BotUser> FindUserById(long user_id)
        // {
        //      var sqlQuery = @"SELECT * FROM Users WHERE user_id = @user_id";

        //     using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
        //     {
        //         command.Parameters.AddWithValue("@user_id", NpgsqlDbType.Integer, user_id);
        //         await _npgsqlConnection.OpenAsync();
        //         var reader = await command.ExecuteReaderAsync();

        //         if (!reader.HasRows)
        //             throw new Exception($"Job get qilishda xatolik, user_id = {user_id}");

        //         var jobs = new List<BotUser>();
        //         while (reader.Read())
        //         {
        //             jobs.Add(new BotUser()
        //             {
        //                 Id = int.Parse(reader["Id"].ToString()),
        //                 Name = reader["Name"].ToString(),
        //                 PhoneNumber = reader["PhoneNumber"].ToString(),
        //                 Role = reader["Role"].ToString(),
        //                 CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
        //                 user_id = long.Parse(reader["user_id"].ToString())
        //             });
        //         }
        //         return jobs.FirstOrDefault();
        //     }
        // }
        public async Task<BotUser> FindUserById(long user_id)
        {
            var sqlQuery = @"SELECT * FROM users WHERE user_id = @user_id"; // Use lowercase if your table is defined that way

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@user_id", NpgsqlDbType.Bigint, user_id); // Use Bigint for long

                await _npgsqlConnection.OpenAsync();
                
                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        BotUser user = null;
                        while (await reader.ReadAsync())
                        {
                            try
                            {
                                user = new BotUser
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("phone")),
                                    Role = reader.GetString(reader.GetOrdinal("Role")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                    user_id = reader.GetInt64(reader.GetOrdinal("user_id"))
                                };
                                System.Console.WriteLine($"User found: {user.Name}");
                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine($"Error reading user data: {ex.Message}");
                            }
                        }

                        return user;
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error executing query: {ex.Message}");
                    return null; 
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }
        }

        public Task<List<BotUser>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        
        public async Task<BotUser> SaveUser(BotUser user)
        {
            var sqlQuery = @"
                INSERT INTO users (name, phone, role, created_at, user_id) 
                VALUES (@name, @phone, @role, @createdAt, @user_id) 
                RETURNING id;"; 

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@phone", user.PhoneNumber);
                command.Parameters.AddWithValue("@role", user.Role);
                command.Parameters.AddWithValue("@createdAt", user.CreatedAt);
                command.Parameters.AddWithValue("@user_id", user.user_id);

                try
                {
                    await _npgsqlConnection.OpenAsync();

                    var newId = await command.ExecuteScalarAsync();
                    System.Console.WriteLine("New user ID: " + newId.ToString());
                    user.Id = Convert.ToInt32(newId);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error saving user: {ex.Message}");
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }

            return user;
        }

    }
}