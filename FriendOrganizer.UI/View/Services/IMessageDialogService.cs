using System.Threading.Tasks;

namespace FriendOrganizer.UI.View.Services
{
  public interface IMessageDialogService
  {
    Task<MessageDialogResult> ShowOkCancelDialogAsync(string text, string title);
    Task ShowInfoDialogAsync(string text);
  }
}