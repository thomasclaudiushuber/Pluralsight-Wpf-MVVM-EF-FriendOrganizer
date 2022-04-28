using FriendOrganizer.Model;
using FriendOrganizer.UI.Event;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using FriendOrganizer.UI.Wrapper;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Data.Lookups;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data.Entity.Infrastructure;

namespace FriendOrganizer.UI.ViewModel
{
  public class FriendDetailViewModel : DetailViewModelBase, IFriendDetailViewModel
  {
    private IFriendRepository _friendRepository;
    private FriendWrapper _friend;
    private FriendPhoneNumberWrapper _selectedPhoneNumber;
    private IProgrammingLanguageLookupDataService _programmingLanguageLookupDataService;

    public FriendDetailViewModel(IFriendRepository friendRepository,
      IEventAggregator eventAggregator,
      IMessageDialogService messageDialogService,
      IProgrammingLanguageLookupDataService programmingLanguageLookupDataService)
      : base(eventAggregator, messageDialogService)
    {
      _friendRepository = friendRepository;
      _programmingLanguageLookupDataService = programmingLanguageLookupDataService;

      eventAggregator.GetEvent<AfterCollectionSavedEvent>()
       .Subscribe(AfterCollectionSaved);

      AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
      RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);

      ProgrammingLanguages = new ObservableCollection<LookupItem>();
      PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
    }

    public override async Task LoadAsync(int friendId)
    {
      var friend = friendId > 0
        ? await _friendRepository.GetByIdAsync(friendId)
        : CreateNewFriend();

      Id = friendId;

      InitializeFriend(friend);

      InitializeFriendPhoneNumbers(friend.PhoneNumbers);

      await LoadProgrammingLanguagesLookupAsync();
    }

    private void InitializeFriend(Friend friend)
    {
      Friend = new FriendWrapper(friend);
      Friend.PropertyChanged += (s, e) =>
      {
        if (!HasChanges)
        {
          HasChanges = _friendRepository.HasChanges();
        }
        if (e.PropertyName == nameof(Friend.HasErrors))
        {
          ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
        if (e.PropertyName == nameof(Friend.FirstName)
         || e.PropertyName == nameof(Friend.LastName))
        {
          SetTitle();
        }

      };
      ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
      if (Friend.Id == 0)
      {
        // Little trick to trigger the validation
        Friend.FirstName = "";
      }
      SetTitle();
    }

    private void SetTitle()
    {
      Title = $"{Friend.FirstName} {Friend.LastName}";
    }

    private void InitializeFriendPhoneNumbers(ICollection<FriendPhoneNumber> phoneNumbers)
    {
      foreach (var wrapper in PhoneNumbers)
      {
        wrapper.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
      }
      PhoneNumbers.Clear();
      foreach (var friendPhoneNumber in phoneNumbers)
      {
        var wrapper = new FriendPhoneNumberWrapper(friendPhoneNumber);
        PhoneNumbers.Add(wrapper);
        wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
      }
    }

    private void FriendPhoneNumberWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!HasChanges)
      {
        HasChanges = _friendRepository.HasChanges();
      }
      if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
      {
        ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
      }
    }

    private async Task LoadProgrammingLanguagesLookupAsync()
    {
      ProgrammingLanguages.Clear();
      ProgrammingLanguages.Add(new NullLookupItem { DisplayMember = " - " });
      var lookup = await _programmingLanguageLookupDataService.GetProgrammingLanguageLookupAsync();
      foreach (var lookupItem in lookup)
      {
        ProgrammingLanguages.Add(lookupItem);
      }
    }

    public FriendWrapper Friend
    {
      get { return _friend; }
      private set
      {
        _friend = value;
        OnPropertyChanged();
      }
    }

    public FriendPhoneNumberWrapper SelectedPhoneNumber
    {
      get { return _selectedPhoneNumber; }
      set
      {
        _selectedPhoneNumber = value;
        OnPropertyChanged();
        ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
      }
    }

    public ICommand AddPhoneNumberCommand { get; }

    public ICommand RemovePhoneNumberCommand { get; }

    public ObservableCollection<LookupItem> ProgrammingLanguages { get; }

    public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }

    protected override async void OnSaveExecute()
    {
      await SaveWithOptimisticConcurrencyAsync(_friendRepository.SaveAsync,
        () =>
        {
          HasChanges = _friendRepository.HasChanges();
          Id = Friend.Id;
          RaiseDetailSavedEvent(Friend.Id, $"{Friend.FirstName} {Friend.LastName}");
        });
    }

    protected override bool OnSaveCanExecute()
    {
      return Friend != null
        && !Friend.HasErrors
        && PhoneNumbers.All(pn => !pn.HasErrors)
        && HasChanges;
    }

    protected override async void OnDeleteExecute()
    {
      if (await _friendRepository.HasMeetingsAsync(Friend.Id))
      {
        await MessageDialogService.ShowInfoDialogAsync($"{Friend.FirstName} {Friend.LastName} can't be deleted, as this friend is part of at least one meeting");
        return;
      }

      var result = await MessageDialogService.ShowOkCancelDialogAsync($"Do you really want to delete the friend {Friend.FirstName} {Friend.LastName}?",
        "Question");
      if (result == MessageDialogResult.OK)
      {
        _friendRepository.Remove(Friend.Model);
        await _friendRepository.SaveAsync();
        RaiseDetailDeletedEvent(Friend.Id);
      }
    }

    private void OnAddPhoneNumberExecute()
    {
      var newNumber = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
      newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
      PhoneNumbers.Add(newNumber);
      Friend.Model.PhoneNumbers.Add(newNumber.Model);
      newNumber.Number = ""; // Trigger validation :-)
    }

    private void OnRemovePhoneNumberExecute()
    {
      SelectedPhoneNumber.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
      _friendRepository.RemovePhoneNumber(SelectedPhoneNumber.Model);
      PhoneNumbers.Remove(SelectedPhoneNumber);
      SelectedPhoneNumber = null;
      HasChanges = _friendRepository.HasChanges();
      ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
    }

    private bool OnRemovePhoneNumberCanExecute()
    {
      return SelectedPhoneNumber != null;
    }

    private Friend CreateNewFriend()
    {
      var friend = new Friend();
      _friendRepository.Add(friend);
      return friend;
    }

    private async void AfterCollectionSaved(AfterCollectionSavedEventArgs args)
    {
      if (args.ViewModelName == nameof(ProgrammingLanguageDetailViewModel))
      {
        await LoadProgrammingLanguagesLookupAsync();
      }
    }
  }
}
