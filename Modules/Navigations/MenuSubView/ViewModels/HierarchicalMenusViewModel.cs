using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.MenuSub.ViewModels
{
    public class HierarchicalMenusViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;

        private HierarchicalMenuItemViewModel _selectedTopMenuItem;

        //private Dictionary<HierarchicalMenuItemViewModel, IEnumerable<HierarchicalMenuItemViewModel>> _topLeafHierarchicalMenuItemLookup = [];
        //private Dictionary<HierarchicalMenuItemViewModel, IEnumerable<HierarchicalMenuItemViewModel>> _allLeafHierarchicalMenuItemLookup = [];
        //private Dictionary<HierarchicalMenuItemViewModel, IEnumerable<HierarchicalMenuItemViewModel>> _allLeafHierarchicalSubMenuItemLookup = [];
        #endregion

        #region Constructors
        public HierarchicalMenusViewModel(IEventAggregator eventAggregator,IMenuService menuService)
        {
            _eventAggregator = eventAggregator;
            _menuService = menuService;

           // _topLeafHierarchicalMenuItemViewModels = new();

            TopHierarchicalMenuItems = new();
            TopLeafHierarchicalMenuItems = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<HierarchicalMenuItemViewModel> TopHierarchicalMenuItems { get; }
        public ObservableCollection<HierarchicalMenuItemViewModel> TopLeafHierarchicalMenuItems { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Create HierarchicalMenuItem ViewModels Method
        private MenuItem _parentMenuItem;
        internal void CreateHierarchicalMenuItemViewModels(MenuItem parentMenuItem)
        {
            IsLoading = true;

            _parentMenuItem = parentMenuItem;

            var subMenuItems = _parentMenuItem.SubMenus;
            foreach (var smi in subMenuItems)
            {
                HierarchicalMenuItemViewModel topHierarchicalMenuItemViewModel = new(_eventAggregator, smi);

                TopHierarchicalMenuItems.Add(topHierarchicalMenuItemViewModel);
                var topLeafHierarchicalMenuItemViewModels = GetTopLeafHierarchicalMenuItemViewModels(topHierarchicalMenuItemViewModel);
              
               TopLeafHierarchicalMenuItems.AddRange(topLeafHierarchicalMenuItemViewModels);
            }
          
            foreach (var mivm in TopLeafHierarchicalMenuItems)
            {
                if (mivm.Isleaf)
                {
                    AddPropertyChanged(mivm);
                }
            }

            void AddPropertyChanged(HierarchicalMenuItemViewModel menuItemvm)
            {
                menuItemvm.PropertyChanged += (sender, e) =>
                {
                    if (sender is HierarchicalMenuItemViewModel hmvm)
                    {
                        if (e.PropertyName == nameof(HierarchicalMenuItemViewModel.IsSelected))
                        {
                            var selectedMenuItemCount = TopLeafHierarchicalMenuItems.Count(mi => mi.IsSelected);
                            if ((_selectedTopMenuItem is not null) && (_selectedTopMenuItem.HasTitle && hmvm.HasTitle && _selectedTopMenuItem.Title != hmvm.Title))
                            {
                                _selectedTopMenuItem.IsSelected = false;
                                _selectedTopMenuItem = hmvm;
                            }
                            else
                            {
                                _selectedTopMenuItem = TopLeafHierarchicalMenuItems.FirstOrDefault(mi => mi.IsSelected);
                            }

                            selectedMenuItemCount = TopLeafHierarchicalMenuItems.Count(mi => mi.IsSelected);
                        }
                    }
                };
            }

            IsLoading = false;
        }
        #endregion

        #region Get Top Leaf HierarchicalMenuItemViewModel Method
        internal IEnumerable<HierarchicalMenuItemViewModel> GetTopLeafHierarchicalMenuItemViewModels(HierarchicalMenuItemViewModel topHieMenuItemViewModel)
        {
            List<HierarchicalMenuItemViewModel> topLeafHierarchicalMenuItemViewModels = new();

            RecursiveSubMenuItemViewModel(topHieMenuItemViewModel);

            void RecursiveSubMenuItemViewModel(HierarchicalMenuItemViewModel currenyHieMenuItemViewModel)
            {
                if (!AnyEqualsHierarchicalMenuItemViewModel(topLeafHierarchicalMenuItemViewModels, currenyHieMenuItemViewModel) && currenyHieMenuItemViewModel.Isleaf && currenyHieMenuItemViewModel.HasTitle)
                {
                    topLeafHierarchicalMenuItemViewModels.Add(currenyHieMenuItemViewModel);
                }

                if (HasChild(currenyHieMenuItemViewModel))
                {
                    foreach (var children in currenyHieMenuItemViewModel.Children)
                    {
                        RecursiveSubMenuItemViewModel(children);
                    }
                }
            }

            bool HasChild(HierarchicalMenuItemViewModel hmivm) => (hmivm is not null) && hmivm.Children.Any();

            return topLeafHierarchicalMenuItemViewModels;
        }
        #endregion

        #region Get All Leaf HierarchicalMenuItem Method
        internal async void GetAllLeafHierarchicalMenuItemViewModelsCore(MenuItem currentMenuItem, IList<MenuItem> leafMenuItems, List<HierarchicalMenuItemViewModel> leafHierarchicalMenuItems)
        {
            //if (!leafMenuItems.Contains(currentMenuItem) && IsLeaf(currentMenuItem) && HasTitle(currentMenuItem) && !HasNavigationName(currentMenuItem))
            if (!AnyEqualsMenuItem(leafMenuItems, currentMenuItem) && IsLeaf(currentMenuItem) && HasTitle(currentMenuItem) && !HasNavigationName(currentMenuItem))
            {
                leafHierarchicalMenuItems.Add(new(_eventAggregator, currentMenuItem));
                leafMenuItems.Add(currentMenuItem);
            }

            if (HasNavigationName(currentMenuItem) && IsLeaf(currentMenuItem))
            {
                currentMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);
            }

            if (HasSubMenu(currentMenuItem))
            {
                RecursiveSubMenuItem(currentMenuItem);
            }

            void RecursiveSubMenuItem(MenuItem parentMenuItem)
            {
                foreach (var smi in parentMenuItem.SubMenus)
                {
                    //if (!leafMenuItems.Contains(smi) && IsLeaf(smi) && HasTitle(smi) && !HasNavigationName(smi))
                    //{
                    //    leafMenuItems.Add(smi);
                    //    leafHierarchicalMenuItems.Add(new(_eventAggregator, smi));
                    //}
                    //RecursiveSubMenuItem(smi);

                    GetAllLeafHierarchicalMenuItemViewModelsCore(smi, leafMenuItems, leafHierarchicalMenuItems);
                }
            }

            bool HasSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any();

            bool IsLeaf(MenuItem mi) => (mi is not null) && mi.SubMenus.Count <= 0;

            bool HasTitle(MenuItem mi) => !string.IsNullOrEmpty(mi.Title);

            bool HasNavigationName(MenuItem mi) => (mi is not null) && !string.IsNullOrEmpty(mi.NavigationName);
        }
        #endregion

        #region Contain Methods
        private bool AnyEqualsHierarchicalMenuItemViewModel(IEnumerable<HierarchicalMenuItemViewModel> hieMenuItemViewModels, HierarchicalMenuItemViewModel hieMenuItemViewModel)
        {
            var isEquals = hieMenuItemViewModels.Any(hmivm => IsEqualsNameOrTitle(hmivm.Title, hieMenuItemViewModel.Title) || IsEqualsNameOrTitle(hmivm.Name, hieMenuItemViewModel.Name));

            return isEquals;
        }

        private bool AnyEqualsMenuItem(IEnumerable<MenuItem> menuItems, MenuItem menuItem)
        {
            var isEquals = menuItems.Any(mi => IsEqualsNameOrTitle(mi.Title, menuItem.Title) || IsEqualsNameOrTitle(mi.Name, menuItem.Name));

            return isEquals;
        }

        private bool NameOrTitleEqualsHierarchicalMenuItemViewModel(HierarchicalMenuItemViewModel hieMenuItemViewModel, string nameOrTitle)
        {
            var isEquals = IsEqualsNameOrTitle(hieMenuItemViewModel.Name, nameOrTitle) || IsEqualsNameOrTitle(hieMenuItemViewModel.Title, nameOrTitle);

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
