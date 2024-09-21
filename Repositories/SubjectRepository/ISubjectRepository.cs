using uzbaseQuiz.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace uzbaseQuiz.Repositories
{
    public interface ISubjectRepository
    {
        Task<Subject> SaveSubject(Subject subject);
        Task<Subject> FindSubjectById(int id);
        Task<List<Subject>> GetAllSubjectsAsync();
        Task<int> DeleteSubject(int id);
        Task<Subject> UpdateSubject(Subject subject);
    }
}