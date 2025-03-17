using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.HamburgerMenuSideBar.ViewModels
{
    public class HamburgerMenuSideBarViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;

        #endregion

        #region Constructors
        public HamburgerMenuSideBarViewModel(IEventAggregator eventAggregator,IMenuService menuService)
        {
            _eventAggregator = eventAggregator;
            _menuService = menuService;

            TopHamburgerMenuSideBarItems = new();
            TopLeafHamburgerMenuSideBarItems = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<HamburgerMenuSideBarItemViewModel> TopHamburgerMenuSideBarItems { get; private set; }
        public ObservableCollection<HamburgerMenuSideBarItemViewModel> TopLeafHamburgerMenuSideBarItems { get; }

        private HamburgerMenuSideBarItemViewModel _previewSelectedHamburgerMenuItem;
        internal HamburgerMenuSideBarItemViewModel PreviewSelectedHamburgerMenuItem => _previewSelectedHamburgerMenuItem;

        internal HamburgerMenuSideBarItemViewModel _selectedHamburgerMenuSideBarItem;
        public HamburgerMenuSideBarItemViewModel SelectedHamburgerMenuSideBarItem
        {
            get => _selectedHamburgerMenuSideBarItem;
            set
            {
                _previewSelectedHamburgerMenuItem = _selectedHamburgerMenuSideBarItem;

                var previewSelectedHamburgerMenuItem = _selectedHamburgerMenuSideBarItem;

                if (SetProperty(ref _selectedHamburgerMenuSideBarItem, value))
                {
                    if (previewSelectedHamburgerMenuItem is not null && previewSelectedHamburgerMenuItem.IsSelected)
                    {
                        previewSelectedHamburgerMenuItem.IsSelected = false;
                    }

                    if (_selectedHamburgerMenuSideBarItem is not null && !_selectedHamburgerMenuSideBarItem.IsSelected)
                    {
                        _selectedHamburgerMenuSideBarItem.IsSelected = true;
                    }
                }
            }
        }

        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set
            {
                if (SetProperty<bool>(ref _isPaneOpen, value))
                {
                    foreach (var hmbi in TopLeafHamburgerMenuSideBarItems)
                    {
                        hmbi.IsPaneOpen = value;
                    }
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Create HamburgerMenuItemBar ViewModel Method
        internal async Task CreateHamburgerMenuBarItemViewModelsAsync()
        {
            IsLoading = true;

            var rootMenuItem = await _menuService.GetMenuAsync("All");

            var subMenuItems = rootMenuItem.SubMenus;
            foreach (var smi in subMenuItems)
            {
                HamburgerMenuSideBarItemViewModel topHamburgerMenuBarItemViewModel = new(_eventAggregator, smi);
                TopHamburgerMenuSideBarItems.Add(topHamburgerMenuBarItemViewModel);

                var topLeafHierarchicalMenuItemViewModels = GetTopLeafHamburgerMenuSideBarItemViewModels(topHamburgerMenuBarItemViewModel);
                TopLeafHamburgerMenuSideBarItems.AddRange(topLeafHierarchicalMenuItemViewModels);
            }

            _selectedHamburgerMenuSideBarItem = TopLeafHamburgerMenuSideBarItems.FirstOrDefault(mi => mi.IsSelectedOnInitialize) ?? TopLeafHamburgerMenuSideBarItems[0];
            if (_selectedHamburgerMenuSideBarItem is not null)
            {
                _selectedHamburgerMenuSideBarItem.IsSelected = true;
            }

            IsLoading = false;
        }

        #region Get Top Leaf HamburgerMenuBarItemViewModel Method
        internal IEnumerable<HamburgerMenuSideBarItemViewModel> GetTopLeafHamburgerMenuSideBarItemViewModels(HamburgerMenuSideBarItemViewModel topHamburgerMenuSideBarItemViewModel)
        {
            List<HamburgerMenuSideBarItemViewModel> topLeafHamburgerMenuSideBarItemViewModels = new();

            RecursiveSubMenuItemViewModel(topHamburgerMenuSideBarItemViewModel);

            void RecursiveSubMenuItemViewModel(HamburgerMenuSideBarItemViewModel currenyHamburgerMenuSideBarItemViewModel)
            {
                if (!AnyEqualsHierarchicalMenuItemViewModel(topLeafHamburgerMenuSideBarItemViewModels, currenyHamburgerMenuSideBarItemViewModel) && currenyHamburgerMenuSideBarItemViewModel.Isleaf && currenyHamburgerMenuSideBarItemViewModel.HasTitle)
                {
                    topLeafHamburgerMenuSideBarItemViewModels.Add(currenyHamburgerMenuSideBarItemViewModel);
                }

                if (HasChild(currenyHamburgerMenuSideBarItemViewModel))
                {
                    foreach (var children in currenyHamburgerMenuSideBarItemViewModel.Children)
                    {
                        RecursiveSubMenuItemViewModel(children);
                    }
                }
            }

            bool HasChild(HamburgerMenuSideBarItemViewModel hmivm) => (hmivm is not null) && hmivm.Children.Any();

            return topLeafHamburgerMenuSideBarItemViewModels;
        }
        #endregion

        #region Contain Methods
        private bool AnyEqualsHierarchicalMenuItemViewModel(IEnumerable<HamburgerMenuSideBarItemViewModel> hamburgerMenuSideBarItemViewModels, HamburgerMenuSideBarItemViewModel hamburgerMenuSideBarItemViewModel)
        {
            var isEquals = hamburgerMenuSideBarItemViewModels.Any(hmivm => IsEqualsNameOrTitle(hmivm.Title, hamburgerMenuSideBarItemViewModel.Title) || IsEqualsNameOrTitle(hmivm.Name, hamburgerMenuSideBarItemViewModel.Name));

            return isEquals;
        }

        private bool AnyEqualsMenuItem(IEnumerable<MenuItem> menuItems, MenuItem menuItem)
        {
            var isEquals = menuItems.Any(mi => IsEqualsNameOrTitle(mi.Title, menuItem.Title) || IsEqualsNameOrTitle(mi.Name, menuItem.Name));

            return isEquals;
        }

        private bool IsEqualsNameOrTitle(string nameOrTitle, string otherNameOrTitle)
        {
            if (string.IsNullOrEmpty(nameOrTitle) || string.IsNullOrEmpty(otherNameOrTitle))
            {
                return false;
            }

            var isEquals = (!string.IsNullOrEmpty(nameOrTitle) && otherNameOrTitle.Equals(nameOrTitle, StringComparison.InvariantCultureIgnoreCase)) ||
                           (!string.IsNullOrEmpty(nameOrTitle) && otherNameOrTitle.Equals(nameOrTitle, StringComparison.InvariantCultureIgnoreCase));

            return isEquals;
        }
        #endregion
        #endregion
    }
}
