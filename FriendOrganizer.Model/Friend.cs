using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FriendOrganizer.Model
{
  public class Friend
  {
    public Friend()
    {
      PhoneNumbers = new Collection<FriendPhoneNumber>();
      Meetings = new Collection<Meeting>();
    }

    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }

    [StringLength(50)]
    public string LastName { get; set; }

    [StringLength(50)]
    [EmailAddress]
    public string Email { get; set; }

    public int? FavoriteLanguageId { get; set; }

    public ProgrammingLanguage FavoriteLanguage { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }

    public ICollection<FriendPhoneNumber> PhoneNumbers { get; set; }

    public ICollection<Meeting> Meetings { get; set; }
  }
}
