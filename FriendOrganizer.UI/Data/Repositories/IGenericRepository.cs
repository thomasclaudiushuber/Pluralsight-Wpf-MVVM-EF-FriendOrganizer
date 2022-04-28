using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
  public interface IGenericRepository<T>
  {
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task SaveAsync();
    bool HasChanges();
    void Add(T model);
    void Remove(T model);
  }
}