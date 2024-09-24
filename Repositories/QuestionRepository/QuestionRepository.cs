using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using uzbaseQuiz.Models;

namespace uzbaseQuiz.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly string _connectionString;

        public QuestionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Question> FindQuestionById(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM questions WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Question
                            {
                                Id = reader.GetInt32(0),
                                SubjectId = reader.GetInt32(1),
                                QuestionText = reader.GetString(2),
                                CorrectAnswerId = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            var questions = new List<Question>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM questions", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            questions.Add(new Question
                            {
                                Id = reader.GetInt32(0),
                                SubjectId = reader.GetInt32(1),
                                QuestionText = reader.GetString(2),
                                CorrectAnswerId = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
            return questions;
        }

        public async Task<List<Question>> GetQuestionsBySubjectIdAsync(int subjectId)
        {
            var questions = new List<Question>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM questions WHERE subject_id = @subjectId", connection))
                {
                    command.Parameters.AddWithValue("subjectId", subjectId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            questions.Add(new Question
                            {
                                Id = reader.GetInt32(0),
                                SubjectId = reader.GetInt32(1),
                                QuestionText = reader.GetString(2),
                                CorrectAnswerId = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
            return questions;
        }

        public async Task<Question> SaveQuestion(Question question)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("INSERT INTO questions (subject_id, question_text, correct_answer_id) VALUES (@subjectId, @questionText, @correctAnswerId) RETURNING id", connection))
                {
                    command.Parameters.AddWithValue("subjectId", question.SubjectId);
                    command.Parameters.AddWithValue("questionText", question.QuestionText);
                    command.Parameters.AddWithValue("correctAnswerId", (object)question.CorrectAnswerId ?? DBNull.Value);
                    question.Id = (int)await command.ExecuteScalarAsync();
                }
            }
            return question;
        }

        public async Task UpdateQuestion(Question question)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("UPDATE questions SET subject_id = @subjectId, question_text = @questionText, correct_answer_id = @correctAnswerId WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("subjectId", question.SubjectId);
                    command.Parameters.AddWithValue("questionText", question.QuestionText);
                    command.Parameters.AddWithValue("correctAnswerId", (object)question.CorrectAnswerId ?? DBNull.Value);
                    command.Parameters.AddWithValue("id", question.Id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteQuestion(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("DELETE FROM questions WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
