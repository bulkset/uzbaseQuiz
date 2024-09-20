using uzbaseQuiz.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace uzbaseQuiz.Repositories
{
    public interface IUserRepository
    {
        Task<BotUser> SaveUser(BotUser user);      
        Task<BotUser> FindUserById(long id);        
        Task<List<BotUser>> GetAllAsync();        
        Task<long> DeleteUser(long id);        
    }
}
