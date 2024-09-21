using System.Collections.Generic;
using System.Threading.Tasks;
using uzbaseQuiz.Models;

namespace uzbaseQuiz.Repositories
{
    public interface ISubjectRepository
    {
        Task<long> DeleteSubject(long subject_id);
        Task<Subject> FindSubjectById(long subject_id);
        Task<List<Subject>> GetAllAsync();
        Task<Subject> SaveSubject(Subject subject);
        
        Task<Subject> UpdateSubject(Subject subject);
    }
}
