using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.HamburgerMenu.ViewModels
{
    public class HamburgerMenuViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        private MenuItem _parentMenuItem;
        #endregion

        #region Constructors
        public HamburgerMenuViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        
            HamburgerMenuItems = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<HamburgerMenuItemViewModel> HamburgerMenuItems { get; private set; }

        private HamburgerMenuItemViewModel _selectedHamburgerMenuItem;
        public HamburgerMenuItemViewModel SelectedHamburgerMenuItem
        {
            get => _selectedHamburgerMenuItem;
            set
            {
                var previewSelectedHamburgerMenuItem = _selectedHamburgerMenuItem;

                if (SetProperty(ref _selectedHamburgerMenuItem, value))
                {
                    if (previewSelectedHamburgerMenuItem is not null && previewSelectedHamburgerMenuItem.IsSelected)
                    {
                        previewSelectedHamburgerMenuItem.IsSelected = false;
                    }

                    if (_selectedHamburgerMenuItem is not null && !_selectedHamburgerMenuItem.IsSelected)
                    {
                        _selectedHamburgerMenuItem.IsSelected = true;
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
                    foreach (var hmi in HamburgerMenuItems)
                    {
                        hmi.IsPaneOpen = value;
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

        #region Create HamburgerMenuItem ViewModel Method
        internal void CreateHamburgerMenuItemViewModels(MenuItem parentMenuItem)
        {
            IsLoading = true;

            _parentMenuItem = parentMenuItem;

            if (HasSubMenu(_parentMenuItem))
            {
                List<MenuItem> allLeafMenuItems = new();
                foreach (var smi in _parentMenuItem.SubMenus)
                {
                    var leafMenuItems = GetAllLeafMenuItems(smi);
                    allLeafMenuItems.AddRange(leafMenuItems);
                }

                int index = 0;
                foreach (var smi in allLeafMenuItems)
                {
                    HamburgerMenuItemViewModel hamburgerMenuItemViewModel = new(_eventAggregator, index++, smi);
                    //AddPropertyChanged();

                    //void AddPropertyChanged()
                    //{
                    //    hamburgerMenuItemViewModel.PropertyChanged += (sender, propertyName) =>
                    //    {
                    //        if (sender is  HamburgerMenuItemViewModel nivm)
                    //        {
                    //            if (!nivm.IsSelected)
                    //            {
                    //                _selectedNavigationItem =null;
                    //                RaisePropertyChanged(nameof(SelectedNavigationItem));
                    //            }
                    //        }
                    //    };
                    //}

                    HamburgerMenuItems.Add(hamburgerMenuItemViewModel);
                }
            }

            bool HasSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any();

            IsLoading = false;
        }

        private IEnumerable<MenuItem> GetAllLeafMenuItems(MenuItem menuItem)
        {
            List<MenuItem> leafMenuItems = new();

            RecursiveSubMenuItem(menuItem);

            void RecursiveSubMenuItem(MenuItem currentMenuItem)
            {
                if (!AnyEqualsMenuItem(leafMenuItems, currentMenuItem) && Isleaf(currentMenuItem) && HasTitle(currentMenuItem))
                {
                    leafMenuItems.Add(currentMenuItem);
                }

                if (HasSubMenu(currentMenuItem))
                {
                    foreach (var smi in currentMenuItem.SubMenus)
                    {
                        RecursiveSubMenuItem(smi);
                    }
                }
            }

            bool HasSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any();

            bool Isleaf(MenuItem mi) => (mi is not null) && mi.SubMenus.Count <= 0;

            bool HasTitle(MenuItem mi) => (mi is not null) && !string.IsNullOrEmpty(mi.Title);

            return leafMenuItems;
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
    }
}
