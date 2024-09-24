using uzbaseQuiz.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace uzbaseQuiz.Repositories
{
    public class BotUserRepository
    {
        private readonly NpgsqlConnection _npgsqlConnection;

        public BotUserRepository(string connectionString)
        {
            _npgsqlConnection = new NpgsqlConnection(connectionString);
        }

        // Save BotUser
        public async Task<BotUser> SaveBotUserAsync(BotUser botUser)
        {
            var sqlQuery = @"
                INSERT INTO bot_users (name, phone_number, role, created_at, user_id) 
                VALUES (@name, @phoneNumber, @role, @createdAt, @userId) 
                RETURNING id;";

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@name", botUser.Name);
                command.Parameters.AddWithValue("@phoneNumber", botUser.PhoneNumber);
                command.Parameters.AddWithValue("@role", botUser.Role);
                command.Parameters.AddWithValue("@createdAt", botUser.CreatedAt);
                command.Parameters.AddWithValue("@userId", botUser.user_id);

                try
                {
                    await _npgsqlConnection.OpenAsync();
                    var newId = await command.ExecuteScalarAsync();
                    botUser.Id = Convert.ToInt32(newId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving bot user: {ex.Message}");
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }

            return botUser;
        }

        // Find BotUser by Id
        public async Task<BotUser> FindBotUserByIdAsync(int id)
        {
            var sqlQuery = "SELECT * FROM bot_users WHERE id = @id";

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    await _npgsqlConnection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new BotUser
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
                                Role = reader.GetString(reader.GetOrdinal("role")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                user_id = reader.GetInt64(reader.GetOrdinal("user_id"))
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error finding bot user: {ex.Message}");
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }

            return null;
        }

        // Get all BotUsers
        public async Task<List<BotUser>> GetAllBotUsersAsync()
        {
            var sqlQuery = "SELECT * FROM bot_users";
            var botUsers = new List<BotUser>();

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                try
                {
                    await _npgsqlConnection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            botUsers.Add(new BotUser
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
                                Role = reader.GetString(reader.GetOrdinal("role")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                user_id = reader.GetInt64(reader.GetOrdinal("user_id"))
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting all bot users: {ex.Message}");
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }

            return botUsers;
        }

        // Update BotUser
        public async Task<BotUser> UpdateBotUserAsync(BotUser botUser)
        {
            var sqlQuery = @"
                UPDATE bot_users
                SET name = @name, phone_number = @phoneNumber, role = @role, created_at = @createdAt, user_id = @userId
                WHERE id = @id
                RETURNING id;";

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@id", botUser.Id);
                command.Parameters.AddWithValue("@name", botUser.Name);
                command.Parameters.AddWithValue("@phoneNumber", botUser.PhoneNumber);
                command.Parameters.AddWithValue("@role", botUser.Role);
                command.Parameters.AddWithValue("@createdAt", botUser.CreatedAt);
                command.Parameters.AddWithValue("@userId", botUser.user_id);

                try
                {
                    await _npgsqlConnection.OpenAsync();
                    var updatedId = await command.ExecuteScalarAsync();
                    if (updatedId != null)
                    {
                        return botUser;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating bot user: {ex.Message}");
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }

            return null;
        }

        // Delete BotUser
        public async Task<int> DeleteBotUserAsync(int id)
        {
            var sqlQuery = "DELETE FROM bot_users WHERE id = @id";

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    await _npgsqlConnection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 1)
                    {
                        return id;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting bot user: {ex.Message}");
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }

            throw new Exception("Bot user deletion failed.");
        }
    }
}
