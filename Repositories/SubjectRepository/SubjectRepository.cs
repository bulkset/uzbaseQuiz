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

        public async Task<Subject> FindSubjectById(int id)
        {
            var sqlQuery = @"SELECT * FROM subjects WHERE id = @id"; 

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

                await _npgsqlConnection.OpenAsync();

                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        Subject subject = null;
                        while (await reader.ReadAsync())
                        {
                            subject = new Subject
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                MaxScore = reader.GetInt32(reader.GetOrdinal("max_score"))
                            };
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

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            var sqlQuery = @"SELECT * FROM subjects"; 

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                await _npgsqlConnection.OpenAsync();

                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var subjects = new List<Subject>();
                        while (await reader.ReadAsync())
                        {
                            subjects.Add(new Subject
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                MaxScore = reader.GetInt32(reader.GetOrdinal("max_score"))
                            });
                        }

                        return subjects;
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

        public async Task<int> DeleteSubject(int id)
        {
            var sqlQuery = @"DELETE FROM subjects WHERE id = @id";

            await using (var command = new NpgsqlCommand(sqlQuery, _npgsqlConnection))
            {
                command.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);
                await _npgsqlConnection.OpenAsync();
                var changedRowCount = await command.ExecuteNonQueryAsync();

                if (changedRowCount == 1)
                    return id;
                else
                    throw new Exception("Delete qilish jarayonida kutilmagan xatolik.");
            }
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

                await _npgsqlConnection.OpenAsync();

                try
                {
                    var newId = await command.ExecuteScalarAsync();
                    if (newId != null)
                    {
                        return subject;
                    }
                    else
                    {
                        throw new Exception("Updating subject failed.");
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error updating subject: {ex.Message}");
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
    }
}