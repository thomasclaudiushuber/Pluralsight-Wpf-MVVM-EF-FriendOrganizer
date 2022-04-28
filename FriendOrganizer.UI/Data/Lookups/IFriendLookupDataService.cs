using System.Collections.Generic;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Lookups
{
  public interface IFriendLookupDataService
  {
    Task<IEnumerable<LookupItem>> GetFriendLookupAsync();
  }
}