using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Wrapper
{
  public class FriendPhoneNumberWrapper : ModelWrapper<FriendPhoneNumber>
  {
    public FriendPhoneNumberWrapper(FriendPhoneNumber model) : base(model)
    {
    }

    public string Number
    {
      get { return GetValue<string>(); }
      set { SetValue(value); }
    }
  }

}
