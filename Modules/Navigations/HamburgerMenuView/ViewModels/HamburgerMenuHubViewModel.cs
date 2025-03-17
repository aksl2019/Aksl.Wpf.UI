﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

namespace Aksl.Modules.HamburgerMenu.ViewModels
{
    public class HamburgerMenuHubViewModel : BindableBase, INavigationAware
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
        public HamburgerMenuHubViewModel()
        {
            _regionManager = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IRegionManager>();
            _container = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IUnityContainer>();
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();

            _menuService = _container.Resolve<IMenuService>();

            SelectedDisplayMode = SplitViewDisplayMode.CompactInline;
            IsPaneOpen = true;
            SelectedPlacement = SplitViewPanePlacement.Left;
        }
        #endregion

        #region Properties
        public HamburgerMenuViewModel HamburgerMenu { get; private set; }

        private Brush _paneBackground = new SolidColorBrush(Colors.White);
        public Brush PaneBackground
        {
            get => _paneBackground;
            set => SetProperty<Brush>(ref _paneBackground, value);
        }

        public GridLength OpenPaneGridLength
        {
            get { return new GridLength(OpenPaneLength); }
        }

        private double _openPaneLength = 320d;
        public double OpenPaneLength
        {
            get => _openPaneLength;
            set => SetProperty<double>(ref _openPaneLength, value);
        }

        public GridLength CompactPaneGridLength
        {
            get { return new GridLength(CompactPaneLength); }
        }

        private double _compactPaneLength = 48d;
        public double CompactPaneLength
        {
            get => _compactPaneLength;
            set => SetProperty<double>(ref _compactPaneLength, value);
        }

        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set
            {
                if (SetProperty<bool>(ref _isPaneOpen, value))
                {
                    if (HamburgerMenu is not null)
                    { 
                        HamburgerMenu.IsPaneOpen = value; 
                    }
                        
                    VisualState = GetVisualState();
                }
            }
        }

        public List<SplitViewDisplayMode> DisplayModeList
        {
            get => Enum.GetValues(typeof(SplitViewDisplayMode)).Cast<SplitViewDisplayMode>().ToList();
        }

        private SplitViewDisplayMode _selectedDisplayMode = SplitViewDisplayMode.Overlay;
        public SplitViewDisplayMode SelectedDisplayMode
        {
            get => _selectedDisplayMode;
            set
            {
                if (SetProperty<SplitViewDisplayMode>(ref _selectedDisplayMode, value))
                {
                    VisualState = GetVisualState();
                }
            }
        }

        public List<SplitViewPanePlacement> PanePlacementList
        {
            get => Enum.GetValues(typeof(SplitViewPanePlacement)).Cast<SplitViewPanePlacement>().ToList();
        }

        private SplitViewPanePlacement _selectedPanePlacement = SplitViewPanePlacement.Left;
        public SplitViewPanePlacement SelectedPlacement
        {
            get => _selectedPanePlacement;
            set
            {
                if (SetProperty<SplitViewPanePlacement>(ref _selectedPanePlacement, value))
                {
                    VisualState = GetVisualState();
                }
            }
        }

        private string _visualState;
        public string VisualState
        {
            get => _visualState;
            set => SetProperty<string>(ref _visualState, value);
        }
      
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Get State Method
        private bool IsCompact
        {
            get
            {
                return SelectedDisplayMode switch
                {
                    SplitViewDisplayMode.CompactInline or SplitViewDisplayMode.CompactOverlay => true,
                    _ => false,
                };
            }
        }

        private bool IsInline
        {
            get
            {
                return SelectedDisplayMode switch
                {
                    SplitViewDisplayMode.CompactInline or SplitViewDisplayMode.Inline => true,
                    _ => false
                };
            }
        }

        protected virtual string GetVisualState()
        {
            string state;

            if (IsPaneOpen)
            {
                state = "Open";
                state += IsInline ? "Inline" : SelectedDisplayMode.ToString();
            }
            else
            {
                state = "Closed";
                if (IsCompact)
                {
                    state += "Compact";
                }
                //else
                //{
                //    return state;
                //}
            }

            state += SelectedPlacement.ToString();

            return state;
        }
        #endregion

        #region Register BuildWorkspaceView Event
        private void RegisterBuildWorkspaceViewEvents()
        {
            _eventAggregator.GetEvent<OnBuildHamburgerMenuWorkspaceViewEvent>().Subscribe(async (bhmwve) =>
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
                    string viewTypeAssemblyQualifiedName = bhmwve.CurrentMenuItem.ViewName;
                    Type viewType = Type.GetType(viewTypeAssemblyQualifiedName);
                    if (viewType is not null)
                    {
                        IRegion region = _regionManager.Regions[RegionNames.HamburgerMenuWorkspaceRegion];
                        var viewName = viewType.Name;

                        //_currentView = region.GetView(viewTypeAssemblyQualifiedName);
                        _currentView = region.Views.FirstOrDefault(v => v.GetType() == viewType);
                        if (_currentView is null)
                        {
                            _currentView = region.GetView(viewType.FullName);
                        }

                        if (_currentView is not null)
                        {
                            if (bhmwve.CurrentMenuItem.IsCacheable)
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
                                    { "CurrentMenuItem", bhmwve.CurrentMenuItem }
                                };

                                _regionManager.RequestNavigate(RegionNames.HamburgerMenuWorkspaceRegion, viewName, navigationParameters);
                            }
                        }

                        bool CanAddView() => !string.IsNullOrEmpty(bhmwve.CurrentMenuItem.ModuleName) && bhmwve.CurrentMenuItem.SubMenus.Count == 0;
                    }
                    else
                    {
                        //bndwve.CallBack?.Invoke(false);

                        await _dialogViewService.AlertAsync(message: $"Unable to find \"{viewTypeAssemblyQualifiedName}\".", title: $"Error:Missing Type");
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    // bndwve.CallBack?.Invoke(false);

                    await _dialogViewService.AlertAsync(message: $"Unable to loading \"{bhmwve.CurrentMenuItem.ModuleName}\" module.: \"{ex.Message}\"", title: "Error: Load Module");
                }
            }, ThreadOption.UIThread, true);
        }
        #endregion

        #region Create HamburgerMenu ViewModel Method
        private async Task CreateHamburgerMenuViewModelAsync(MenuItem currentMenuItem)
        {
            IsLoading = true;

            try
            {
                var rootMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);

                HamburgerMenu = new(_eventAggregator);
                AddPropertyChanged();

                void AddPropertyChanged()
                {
                    HamburgerMenu.PropertyChanged += (sender, e) =>
                    {
                        if (sender is HamburgerMenuViewModel hmvm)
                        {
                            //if (!hmvm.IsLoading)
                            //{
                            //    IsLoading = false;
                            //}
                            if (e.PropertyName == nameof(HamburgerMenuViewModel.IsLoading) && !hmvm.IsLoading)
                            {
                                IsLoading = false;
                            }
                        }
                    };
                }

                HamburgerMenu.CreateHamburgerMenuItemViewModels(rootMenuItem);
                HamburgerMenu.IsPaneOpen = IsPaneOpen;
                RaisePropertyChanged(nameof(HamburgerMenu));
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertAsync(message: $"Unable to create hamburger menu : \"{ex.Message}\"", title: "Error: Create HamburgerMenu");
            }
            finally
            {
                if (IsLoading)
                {
                    IsLoading = false;
                }
            }
        }
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var parameters = navigationContext.Parameters;
            if (parameters.TryGetValue("CurrentMenuItem", out MenuItem currentMenuItem))
            {
                RegisterBuildWorkspaceViewEvents();

                CreateHamburgerMenuViewModelAsync(currentMenuItem).GetAwaiter().GetResult();
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
