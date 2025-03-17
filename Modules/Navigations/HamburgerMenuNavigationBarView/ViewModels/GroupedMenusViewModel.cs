using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.HamburgerMenuNavigationBar.ViewModels
{
    public class GroupedMenusViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private MenuItem _parentMenuItem;
        private int _currentGroupeIndex = -1;
        #endregion

        #region Constructors
        public GroupedMenusViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            GroupedMenus = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<GroupedMenuViewModel> GroupedMenus { get; }

        private MenuItemViewModel _selectedMenuItemItem;
        public MenuItemViewModel SelectedMenuItem
        {
            get => _selectedMenuItemItem;
            set
            {
                if (SetProperty(ref _selectedMenuItemItem, value))
                {
                    if (_selectedMenuItemItem is not null)
                    {
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

        #region Create GroupedMenu ViewModels Method
        internal void CreateGroupedMenuViewModels(MenuItem parentMenuItem)
        {
            IsLoading = true;

            _parentMenuItem = parentMenuItem;

            int index = 0;
            foreach (var smi in _parentMenuItem.SubMenus)
            {
                //List<MenuItem>  leafs = new();
                //GetLeafMenuItems(smi, leafs);

                IEnumerable<MenuItem> leafMenuItems = GetAllLeafMenuItems(smi);

                GroupedMenuViewModel groupedMenuViewModel = new(_eventAggregator, index++, smi, leafMenuItems);
                AddPropertyChanged();

                GroupedMenus.Add(groupedMenuViewModel);

                void AddPropertyChanged()
                {
                    groupedMenuViewModel.PropertyChanged += (sender, e) =>
                    {
                        if (sender is GroupedMenuViewModel gmvm)
                        {
                            if (e.PropertyName == nameof(GroupedMenuViewModel.IsLoading))
                            {
                                //最后一个
                                if (gmvm.GroupIndex == GroupedMenus.Count() && !gmvm.IsLoading)
                                {
                                    IsLoading = false;
                                }
                            }

                            if (e.PropertyName == nameof(GroupedMenuViewModel.MenuContent))
                            {
                                if (_currentGroupeIndex == gmvm.GroupIndex)
                                {
                                    SelectedMenuItem = gmvm.MenuContent.SelectedMenuItem;
                                }
                                else
                                {
                                    foreach (var gm in GroupedMenus)
                                    {
                                        if (_currentGroupeIndex == gm.GroupIndex)
                                        {
                                            gm.MenuContent.ClearSelectedMenuItem();
                                        }
                                    }

                                    _currentGroupeIndex = gmvm.GroupIndex;
                                    SelectedMenuItem = gmvm.MenuContent.SelectedMenuItem;
                                }
                            }
                        }
                    };
                }
            }

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

        internal void GetLeafMenuItems(MenuItem currentMenuItem, IList<MenuItem> leafMenuItems)
        {
            if (Isleaf(currentMenuItem) && HasTitle(currentMenuItem))
            {
                leafMenuItems.Add(currentMenuItem);
            }

            if (currentMenuItem.SubMenus.Any())
            {
                RecursiveSubMenuItem(currentMenuItem);
            }

            void RecursiveSubMenuItem(MenuItem parentMenuItem)
            {
                foreach (var smi in parentMenuItem.SubMenus)
                {
                    if (!leafMenuItems.Contains(smi) && Isleaf(smi) && HasTitle(smi))
                    {
                        leafMenuItems.Add(smi);
                    }
                    RecursiveSubMenuItem(smi);
                }
            }

            bool Isleaf(MenuItem mi) => mi.SubMenus.Count <= 0;

            bool HasTitle(MenuItem mi) => !string.IsNullOrEmpty(mi.Title);
        }
        #endregion
    }
}
