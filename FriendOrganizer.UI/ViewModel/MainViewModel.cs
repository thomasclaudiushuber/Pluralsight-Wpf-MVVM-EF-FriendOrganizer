using Autofac.Features.Indexed;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
  public class MainViewModel : ViewModelBase
  {
    private IEventAggregator _eventAggregator;
    private IDetailViewModel _selectedDetailViewModel;
    private IMessageDialogService _messageDialogService;
    private IIndex<string, IDetailViewModel> _detailViewModelCreator;

    public MainViewModel(INavigationViewModel navigationViewModel,
      IIndex<string, IDetailViewModel> detailViewModelCreator,
      IEventAggregator eventAggregator,
      IMessageDialogService messageDialogService)
    {
      _eventAggregator = eventAggregator;
      _detailViewModelCreator = detailViewModelCreator;
      _messageDialogService = messageDialogService;

      DetailViewModels = new ObservableCollection<IDetailViewModel>();

      _eventAggregator.GetEvent<OpenDetailViewEvent>()
       .Subscribe(OnOpenDetailView);
      _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
        .Subscribe(AfterDetailDeleted);
      _eventAggregator.GetEvent<AfterDetailClosedEvent>()
        .Subscribe(AfterDetailClosed);

      CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
      OpenSingleDetailViewCommand = new DelegateCommand<Type>(OnOpenSingleDetailViewExecute);

      NavigationViewModel = navigationViewModel;
    }

    public async Task LoadAsync()
    {
      await NavigationViewModel.LoadAsync();
    }

    public ICommand CreateNewDetailCommand { get; }

    public ICommand OpenSingleDetailViewCommand { get; }

    public INavigationViewModel NavigationViewModel { get; }

    public ObservableCollection<IDetailViewModel> DetailViewModels { get; }

    public IDetailViewModel SelectedDetailViewModel
    {
      get { return _selectedDetailViewModel; }
      set
      {
        _selectedDetailViewModel = value;
        OnPropertyChanged();
      }
    }

    private async void OnOpenDetailView(OpenDetailViewEventArgs args)
    {
      var detailViewModel = DetailViewModels
        .SingleOrDefault(vm => vm.Id == args.Id
        && vm.GetType().Name == args.ViewModelName);

      if (detailViewModel == null)
      {
        detailViewModel = _detailViewModelCreator[args.ViewModelName];
        try
        {
          await detailViewModel.LoadAsync(args.Id);
        }
        catch
        {
          await _messageDialogService.ShowInfoDialogAsync("Could not load the entity, " +
              "maybe it was deleted in the meantime by another user. " +
              "The navigation is refreshed for you.");
          await NavigationViewModel.LoadAsync();
          return;
        }

        DetailViewModels.Add(detailViewModel);
      }

      SelectedDetailViewModel = detailViewModel;
    }

    private int nextNewItemId = 0;
    private void OnCreateNewDetailExecute(Type viewModelType)
    {
      OnOpenDetailView(
        new OpenDetailViewEventArgs
        {
          Id = nextNewItemId--,
          ViewModelName = viewModelType.Name
        });
    }

    private void OnOpenSingleDetailViewExecute(Type viewModelType)
    {
      OnOpenDetailView(
     new OpenDetailViewEventArgs
     {
       Id = -1,
       ViewModelName = viewModelType.Name
     });
    }

    private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
    {
      RemoveDetailViewModel(args.Id, args.ViewModelName);
    }

    private void AfterDetailClosed(AfterDetailClosedEventArgs args)
    {
      RemoveDetailViewModel(args.Id, args.ViewModelName);
    }

    private void RemoveDetailViewModel(int id, string viewModelName)
    {
      var detailViewModel = DetailViewModels
             .SingleOrDefault(vm => vm.Id == id
             && vm.GetType().Name == viewModelName);
      if (detailViewModel != null)
      {
        DetailViewModels.Remove(detailViewModel);
      }
    }
  }
}
