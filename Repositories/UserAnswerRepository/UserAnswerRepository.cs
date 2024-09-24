using uzbaseQuiz.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace uzbaseQuiz.Repositories
{
    public class UserAnswerRepository
    {
        private readonly NpgsqlConnection _npgsqlConnection;

        public UserAnswerRepository(string connectionString)
        {
            _npgsqlConnection = new NpgsqlConnection(connectionString);
        }

        // Save UserAnswer
        public async Task<UserAnswer> SaveUserAnswerAsync(UserAnswer userAnswer)
        {
            var sqlQuery = @"
                INSERT INTO user_answers (user_test_id, question_id, answer_id, is_correct) 
                VALUES (@userTestId, @questionId, @answerId, @isCorrect) 
                RETURNING id;";

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@userTestId", userAnswer.UserTestId);
                command.Parameters.AddWithValue("@questionId", userAnswer.QuestionId);
                command.Parameters.AddWithValue("@answerId", userAnswer.AnswerId);
                command.Parameters.AddWithValue("@isCorrect", userAnswer.IsCorrect);

                try
                {
                    await _npgsqlConnection.OpenAsync();
                    var newId = await command.ExecuteScalarAsync();
                    userAnswer.Id = Convert.ToInt32(newId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving user answer: {ex.Message}");
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }

            return userAnswer;
        }

        // Find UserAnswer by Id
        public async Task<UserAnswer> FindUserAnswerByIdAsync(int id)
        {
            var sqlQuery = "SELECT * FROM user_answers WHERE id = @id";

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
                            return new UserAnswer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                UserTestId = reader.GetInt32(reader.GetOrdinal("user_test_id")),
                                QuestionId = reader.GetInt32(reader.GetOrdinal("question_id")),
                                AnswerId = reader.GetInt32(reader.GetOrdinal("answer_id")),
                                IsCorrect = reader.GetBoolean(reader.GetOrdinal("is_correct"))
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error finding user answer: {ex.Message}");
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

        // Get all UserAnswers
        public async Task<List<UserAnswer>> GetAllUserAnswersAsync()
        {
            var sqlQuery = "SELECT * FROM user_answers";
            var userAnswers = new List<UserAnswer>();

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                try
                {
                    await _npgsqlConnection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            userAnswers.Add(new UserAnswer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                UserTestId = reader.GetInt32(reader.GetOrdinal("user_test_id")),
                                QuestionId = reader.GetInt32(reader.GetOrdinal("question_id")),
                                AnswerId = reader.GetInt32(reader.GetOrdinal("answer_id")),
                                IsCorrect = reader.GetBoolean(reader.GetOrdinal("is_correct"))
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting all user answers: {ex.Message}");
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }

            return userAnswers;
        }

        // Update UserAnswer
        public async Task<UserAnswer> UpdateUserAnswerAsync(UserAnswer userAnswer)
        {
            var sqlQuery = @"
                UPDATE user_answers
                SET user_test_id = @userTestId, question_id = @questionId, answer_id = @answerId, is_correct = @isCorrect
                WHERE id = @id
                RETURNING id;";

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@id", userAnswer.Id);
                command.Parameters.AddWithValue("@userTestId", userAnswer.UserTestId);
                command.Parameters.AddWithValue("@questionId", userAnswer.QuestionId);
                command.Parameters.AddWithValue("@answerId", userAnswer.AnswerId);
                command.Parameters.AddWithValue("@isCorrect", userAnswer.IsCorrect);

                try
                {
                    await _npgsqlConnection.OpenAsync();
                    var updatedId = await command.ExecuteScalarAsync();
                    if (updatedId != null)
                    {
                        return userAnswer;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating user answer: {ex.Message}");
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

        // Delete UserAnswer
        public async Task<int> DeleteUserAnswerAsync(int id)
        {
            var sqlQuery = "DELETE FROM user_answers WHERE id = @id";

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
                    Console.WriteLine($"Error deleting user answer: {ex.Message}");
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }

            throw new Exception("User answer deletion failed.");
        }
    }
}
