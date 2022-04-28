using FriendOrganizer.Model;
using System;

namespace FriendOrganizer.UI.Wrapper
{
  public class MeetingWrapper : ModelWrapper<Meeting>
  {
    public MeetingWrapper(Meeting model) : base(model)
    {
    }

    public int Id { get { return Model.Id; } }

    public string Title
    {
      get { return GetValue<string>(); }
      set { SetValue(value); }
    }

    public DateTime DateFrom
    {
      get { return GetValue<DateTime>(); }
      set
      {
        SetValue(value);
        if (DateTo < DateFrom)
        {
          DateTo = DateFrom;
        }
      }
    }

    public DateTime DateTo
    {
      get { return GetValue<DateTime>(); }
      set
      {
        SetValue(value);
        if (DateTo < DateFrom)
        {
          DateFrom = DateTo;
        }
      }
    }
  }
}
