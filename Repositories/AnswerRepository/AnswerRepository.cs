using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using uzbaseQuiz.Models;

namespace uzbaseQuiz.Repositories
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly string _connectionString;

        public AnswerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Answer>> GetAllAnswersAsync()
        {
            var answers = new List<Answer>();

            using (var db = new NpgsqlConnection(_connectionString))
            {
                await db.OpenAsync();

                using (var cmd = new NpgsqlCommand("SELECT id, question_id, answer_text, is_correct FROM answers", db))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            answers.Add(new Answer
                            {
                                Id = reader.GetInt32(0),
                                QuestionId = reader.GetInt32(1),
                                AnswerText = reader.GetString(2),
                                IsCorrect = reader.GetBoolean(3)
                            });
                        }
                    }
                }
            }

            return answers;
        }

        public async Task<Answer> FindAnswerById(int id)
        {
            Answer answer = null;

            using (var db = new NpgsqlConnection(_connectionString))
            {
                await db.OpenAsync();

                using (var cmd = new NpgsqlCommand("SELECT id, question_id, answer_text, is_correct FROM answers WHERE id = @Id", db))
                {
                    cmd.Parameters.AddWithValue("Id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            answer = new Answer
                            {
                                Id = reader.GetInt32(0),
                                QuestionId = reader.GetInt32(1),
                                AnswerText = reader.GetString(2),
                                IsCorrect = reader.GetBoolean(3)
                            };
                        }
                    }
                }
            }

            return answer;
        }

        public async Task SaveAnswer(Answer answer)
        {
            using (var db = new NpgsqlConnection(_connectionString))
            {
                await db.OpenAsync();

                using (var cmd = new NpgsqlCommand("INSERT INTO answers (question_id, answer_text, is_correct) VALUES (@QuestionId, @AnswerText, @IsCorrect)", db))
                {
                    cmd.Parameters.AddWithValue("QuestionId", answer.QuestionId);
                    cmd.Parameters.AddWithValue("AnswerText", answer.AnswerText);
                    cmd.Parameters.AddWithValue("IsCorrect", answer.IsCorrect);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateAnswer(Answer answer)
        {
            using (var db = new NpgsqlConnection(_connectionString))
            {
                await db.OpenAsync();

                using (var cmd = new NpgsqlCommand("UPDATE answers SET question_id = @QuestionId, answer_text = @AnswerText, is_correct = @IsCorrect WHERE id = @Id", db))
                {
                    cmd.Parameters.AddWithValue("QuestionId", answer.QuestionId);
                    cmd.Parameters.AddWithValue("AnswerText", answer.AnswerText);
                    cmd.Parameters.AddWithValue("IsCorrect", answer.IsCorrect);
                    cmd.Parameters.AddWithValue("Id", answer.Id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAnswer(int id)
        {
            using (var db = new NpgsqlConnection(_connectionString))
            {
                await db.OpenAsync();

                using (var cmd = new NpgsqlCommand("DELETE FROM answers WHERE id = @Id", db))
                {
                    cmd.Parameters.AddWithValue("Id", id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
