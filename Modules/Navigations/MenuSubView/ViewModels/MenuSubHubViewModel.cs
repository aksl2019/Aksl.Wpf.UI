using System;
using System.Linq;
using System.Threading.Tasks;

using Prism;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unity;

using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;
using Aksl.Toolkit.Services;

namespace Aksl.Modules.MenuSub.ViewModels
{
    public class MenuSubHubViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator; 
        private readonly IDialogViewService _dialogViewService;
        private readonly IMenuService _menuService;
        private object _currentView;
        #endregion

        #region Constructors
        public MenuSubHubViewModel()
        {
            _container = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IUnityContainer>();
            _regionManager = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IRegionManager>();
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();
            _menuService = _container.Resolve<IMenuService>();
        }
        #endregion

        #region Properties
        public HierarchicalMenusViewModel HierarchicalMenus { get; private set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        #endregion

        #region Register BuildWorkspaceView Event
        private void RegisterBuildWorkspaceViewEvents()
        {
            _eventAggregator.GetEvent<OnMenuSubContentChangedViewEvent>().Subscribe(async (msccve) =>
            {
                var currentMenuItem = msccve.CurrentMenuItem;

                try
                {
                    string viewTypeAssemblyQualifiedName = currentMenuItem.ViewName;
                    Type viewType = Type.GetType(viewTypeAssemblyQualifiedName);
                    // var view = _container.Resolve(viewType);
                    if (viewType is not null)
                    {
                        IRegion region = _regionManager.Regions[RegionNames.MenuSubWorkspaceRegion];
                        var viewName = viewType.Name;

                        _currentView = region.Views.FirstOrDefault(v => v.GetType() == viewType);
                        if (_currentView is null)
                        {
                            _currentView = region.GetView(viewType.FullName);
                        }

                        if (_currentView is not null)
                        {
                            if (currentMenuItem.IsCacheable)
                            {
                                region.Activate(_currentView);
                            }
                            else
                            {
                                region.Remove(_currentView);

                                AddView();
                            }
                        }
                        else
                        {
                            AddView();
                        }

                        void AddView()
                        {
                            if (IsSelectedOnInitialize())
                            {
                                _regionManager.RequestNavigate(RegionNames.MenuSubWorkspaceRegion, viewName);
                            }
                            else if (CanAddView())
                            {
                                NavigationParameters navigationParameters = new()
                                {
                                    { "CurrentMenuItem", currentMenuItem }
                                };

                                _regionManager.RequestNavigate(RegionNames.MenuSubWorkspaceRegion, viewName, navigationParameters);
                            }
                        }

                        bool IsSelectedOnInitialize() => !string.IsNullOrEmpty(currentMenuItem.ModuleName) && currentMenuItem.IsSelectedOnInitialize;

                        bool CanAddView() => !string.IsNullOrEmpty(currentMenuItem.ModuleName) && currentMenuItem.SubMenus.Count == 0;
                    }
                    else
                    {
                        await _dialogViewService.AlertAsync(message: $"Unable to find \"{viewTypeAssemblyQualifiedName}\".", title: $"Error:Missing Type");
                    }
                }
                catch (Exception ex)
                {
                    await _dialogViewService.AlertAsync(message: $"Unable to loading \"{currentMenuItem.ModuleName}\" module.: \"{ex.Message}\"", title: "Error: Load Module");
                }
            }, ThreadOption.UIThread, true);
        }
        #endregion

        #region Create HierarchicalMenus ViewModel Method
        private async Task CreateHierarchicalMenusViewModel(MenuItem currentMenuItem)
        {
            IsLoading = true;

            var rootMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);

            HierarchicalMenus = new(_eventAggregator,_menuService);
            AddPropertyChanged();

            void AddPropertyChanged()
            {
                HierarchicalMenus.PropertyChanged += (sender, e) =>
                {
                    if (sender is HierarchicalMenusViewModel hmvm)
                    {
                        if (e.PropertyName == nameof(HierarchicalMenusViewModel.IsLoading) && !hmvm.IsLoading)
                        {
                            IsLoading = false;
                        }
                    }
                };
            }

            HierarchicalMenus.CreateHierarchicalMenuItemViewModels(rootMenuItem);
            RaisePropertyChanged(nameof(HierarchicalMenus));
        }
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var parameters = navigationContext.Parameters;
            if (parameters.TryGetValue("CurrentMenuItem", out MenuItem currentMenuItem))
            {
                RegisterBuildWorkspaceViewEvents();

                CreateHierarchicalMenusViewModel(currentMenuItem).GetAwaiter().GetResult();
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
        #endregion
    }
}
