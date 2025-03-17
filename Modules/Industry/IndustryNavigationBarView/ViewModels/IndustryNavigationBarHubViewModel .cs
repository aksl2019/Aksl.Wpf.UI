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

using Aksl.Toolkit.Services;

using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;

namespace Aksl.Modules.IndustryNavigationBar.ViewModels
{
    public class IndustryNavigationBarHubViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IDialogViewService _dialogViewService;
        private readonly IMenuService _menuService;
        private object _currentView;
        #endregion

        #region Constructors
       // public IndustryNavigationBarHubViewModel(IUnityContainer container, IEventAggregator eventAggregator, IRegionManager regionManager, IDialogViewService dialogViewService)
        public IndustryNavigationBarHubViewModel()
        {
            //_container = container;
            //_eventAggregator = eventAggregator;
            //_regionManager = regionManager;
            //_dialogViewService = dialogViewService;

            _regionManager = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IRegionManager>();
            _container = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IUnityContainer>();
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();

            _menuService = _container.Resolve<IMenuService>();
        }
        #endregion

        #region Properties
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }

        public GroupedMenusViewModel NavigationBars { get; private set; }
        #endregion

        #region RegisterEvent Method
        private void RegisterBuildWorkspaceViewEvents()
        {
            _eventAggregator.GetEvent<OnBuildIndustryWorkspaceViewEvent>().Subscribe(async (biwve) =>
            {
                try
                {
                    #region Method
                    //_moduleManager.LoadModule(bwve.CurrentMenuItem.ModuleName);

                    //string viewTypeAssemblyQualifiedName = bwve.CurrentMenuItem.ViewName;

                    //Type viewType = Type.GetType(viewTypeAssemblyQualifiedName);
                    //var view = _container.Resolve(viewType);
                    //if (view != null)
                    //{
                    //    IRegion region = _regionManager.Regions[RegionNames.NavigationBarWorkspaceRegion];
                    //    region.RemoveAll();

                    //    //_currentView = region.GetView(viewTypeAssemblyQualifiedName);

                    //    //if (_currentView != null)
                    //    //{
                    //    //    region.Remove(_currentView);
                    //    //}

                    //    _currentView = view;
                    //    region.Add(_currentView, viewTypeAssemblyQualifiedName);
                    //}
                    #endregion

                    #region Method
                    //string viewTypeAssemblyQualifiedName = bwve.CurrentMenuItem.ViewName;
                    //Type viewType = Type.GetType(viewTypeAssemblyQualifiedName);
                    //if (viewType is not null)
                    //{
                    //    IRegion region = _regionManager.Regions[RegionNames.NavigationBarWorkspaceRegion];
                    //    var viewName = viewType.Name;

                    //    //_currentView = region.GetView(viewTypeAssemblyQualifiedName);
                    //    _currentView = region.Views.FirstOrDefault(v => v.GetType() == viewType);
                    //    if (_currentView is not null)
                    //    {
                    //        if (bwve.CurrentMenuItem.IsCacheable)
                    //        {
                    //            region.Activate(_currentView);
                    //        }
                    //        else
                    //        {
                    //            region.Remove(_currentView);

                    //            var view = _container.Resolve(viewType);
                    //            region.Add(view, viewTypeAssemblyQualifiedName);

                    //            region.Activate(view);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        var view = _container.Resolve(viewType);
                    //        region.Add(view, viewTypeAssemblyQualifiedName);

                    //        region.Activate(view);
                    //    }
                    //}
                    #endregion

                    #region Method
                    string viewTypeAssemblyQualifiedName = biwve.CurrentMenuItem.ViewName;
                    Type viewType = Type.GetType(viewTypeAssemblyQualifiedName);
                    if (viewType is not null)
                    {
                        IRegion region = _regionManager.Regions[RegionNames.IndustryNavigationBarWorkspaceRegion];
                        var viewName = viewType.Name;

                        //_currentView = region.GetView(viewTypeAssemblyQualifiedName);
                        _currentView = region.Views.FirstOrDefault(v => v.GetType() == viewType);
                        if (_currentView is null)
                        {
                            _currentView = region.GetView(viewType.FullName);
                        }

                        if (_currentView is not null)
                        {
                            if (biwve.CurrentMenuItem.IsCacheable)
                            {
                                region.Activate(_currentView);
                            }
                            else
                            {
                                region.Remove(_currentView);

                                // ResolveView();
                                AddView();

                                //var view = _container.Resolve(viewType);
                                //region.Add(view, viewTypeAssemblyQualifiedName);

                                //region.Activate(view);
                            }
                        }
                        else
                        {
                            // ResolveView();
                            AddView();

                            //var view = _container.Resolve(viewType);
                            //region.Add(view, viewTypeAssemblyQualifiedName);

                            //region.Activate(view);
                        }

                        void ResolveView()
                        {
                            var view = _container.Resolve(viewType);
                            region.Add(view, viewTypeAssemblyQualifiedName);

                            region.Activate(view);
                        }

                        void AddView()
                        {
                            if (CanAddView())
                            {
                                NavigationParameters navigationParameters = new()
                                {
                                    { "CurrentMenuItem", biwve.CurrentMenuItem }
                                };

                                _regionManager.RequestNavigate(RegionNames.IndustryNavigationBarWorkspaceRegion, viewName, navigationParameters);
                            }
                        }

                        bool CanAddView() => !string.IsNullOrEmpty(biwve.CurrentMenuItem.ModuleName) && biwve.CurrentMenuItem.SubMenus.Count == 0;
                    }
                    else
                    {
                        await _dialogViewService.AlertAsync(message: $"Unable to find \"{viewTypeAssemblyQualifiedName}\".", title: $"Error:Missing Type");
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    await _dialogViewService.AlertAsync(message: $"Unable to loading \"{biwve.CurrentMenuItem.ModuleName}\" module.: \"{ex.Message}\"", title: "Error: Load Module");
                }
            }, ThreadOption.UIThread, true);
        }
        #endregion

        #region Create GroupedMenus ViewModel Method
        private async Task CreateGroupedMenusViewModelAsync(MenuItem parentMenuItem)
        {
            IsLoading = true;

            var rootMenuItem = await _menuService.GetMenuAsync(parentMenuItem.NavigationName);

            NavigationBars = new(_eventAggregator, rootMenuItem);
            AddPropertyChanged();

            void AddPropertyChanged()
            {
                NavigationBars.PropertyChanged += (sender, propertyName) =>
                {
                    if (sender is GroupedMenusViewModel gmvm)
                    {
                        if (!gmvm.IsLoading)
                        {
                            IsLoading = false;
                        }
                    }
                };
            }

            NavigationBars.CreateGroupedMenuViewModels();
            RaisePropertyChanged(nameof(NavigationBars));
        }
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegisterBuildWorkspaceViewEvents();

            var parameters = navigationContext.Parameters;
            if (parameters.TryGetValue("CurrentMenuItem", out MenuItem parentMenuItem))
            {
                CreateGroupedMenusViewModelAsync(parentMenuItem).GetAwaiter().GetResult();
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
