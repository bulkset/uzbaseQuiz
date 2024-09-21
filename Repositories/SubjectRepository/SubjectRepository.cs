using uzbaseQuiz.Models;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace uzbaseQuiz.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly NpgsqlConnection _npgsqlConnection;
        public SubjectRepository(string connectionString)
            => _npgsqlConnection = new NpgsqlConnection(connectionString);

        public async Task<long> DeleteSubject(long subject_id)
        {
            var sqlQuery = @"DELETE FROM subjects
                             WHERE id = @id;";

            using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, subject_id);
                await _npgsqlConnection.OpenAsync();
                var changedRowCount = await command.ExecuteNonQueryAsync();

                if (changedRowCount == 1)
                    return subject_id;
                else
                    throw new Exception("Delete process failed.");
            }
        }

        public async Task<Subject> FindSubjectById(long subject_id)
        {
            var sqlQuery = @"SELECT * FROM subjects WHERE id = @id";

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@id", NpgsqlDbType.Bigint, subject_id);

                await _npgsqlConnection.OpenAsync();

                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        Subject subject = null;
                        while (await reader.ReadAsync())
                        {
                            try
                            {
                                subject = new Subject
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Name = reader.GetString(reader.GetOrdinal("name")),
                                    MaxScore = reader.GetInt32(reader.GetOrdinal("max_score"))
                                };
                                System.Console.WriteLine($"Subject found: {subject.Name}");
                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine($"Error reading subject data: {ex.Message}");
                            }
                        }

                        return subject;
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

        public async Task<List<Subject>> GetAllAsync()
        {
            var sqlQuery = @"SELECT * FROM subjects;";
            var subjects = new List<Subject>();

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                await _npgsqlConnection.OpenAsync();
                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var subject = new Subject
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                MaxScore = reader.GetInt32(reader.GetOrdinal("max_score"))
                            };
                            subjects.Add(subject);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error retrieving subjects: {ex.Message}");
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }
            return subjects;
        }

        public async Task<Subject> SaveSubject(Subject subject)
        {
            var sqlQuery = @"
                INSERT INTO subjects (name, max_score) 
                VALUES (@name, @maxScore) 
                RETURNING id;";

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@name", subject.Name);
                command.Parameters.AddWithValue("@maxScore", subject.MaxScore);

                try
                {
                    await _npgsqlConnection.OpenAsync();

                    var newId = await command.ExecuteScalarAsync();
                    System.Console.WriteLine("New subject ID: " + newId.ToString());
                    subject.Id = Convert.ToInt32(newId);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error saving subject: {ex.Message}");
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }

            return subject;
        }

        public async Task<Subject> UpdateSubject(Subject subject)
        {
            var sqlQuery = @"
                UPDATE subjects
                SET name = @name, max_score = @maxScore
                WHERE id = @id
                RETURNING id;";

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@id", subject.Id);
                command.Parameters.AddWithValue("@name", subject.Name);
                command.Parameters.AddWithValue("@maxScore", subject.MaxScore);

                try
                {
                    await _npgsqlConnection.OpenAsync();

                    var updatedId = await command.ExecuteScalarAsync();
                    if (updatedId != null)
                    {
                        System.Console.WriteLine("Updated subject ID: " + updatedId.ToString());
                        subject.Id = Convert.ToInt32(updatedId);
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error updating subject: {ex.Message}");
                }
                finally
                {
                    if (_npgsqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        await _npgsqlConnection.CloseAsync();
                    }
                }
            }
            return subject;
        }
    }
}
