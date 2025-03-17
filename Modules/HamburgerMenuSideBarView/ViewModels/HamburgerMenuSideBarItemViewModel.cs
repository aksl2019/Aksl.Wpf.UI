using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;

using Aksl.Toolkit.Controls;
using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;

namespace Aksl.Modules.HamburgerMenuSideBar.ViewModels
{
    public class HamburgerMenuSideBarItemViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        protected readonly HamburgerMenuSideBarItemViewModel _parent;
        private readonly MenuItem _menuItem;
        #endregion

        #region Constructors
        public HamburgerMenuSideBarItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem) : this(eventAggregator, menuItem, null)
        {
            RaisePropertyChanged(nameof(Isleaf));
        }

        public HamburgerMenuSideBarItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem, HamburgerMenuSideBarItemViewModel parent)
        {
            _eventAggregator = eventAggregator;
            _menuItem = menuItem;
            _parent = parent;

            _children = new((from child in _menuItem.SubMenus
                             select new HamburgerMenuSideBarItemViewModel(eventAggregator, child, this)).ToList<HamburgerMenuSideBarItemViewModel>());

            RaisePropertyChanged(nameof(Isleaf));
        }
        #endregion

        #region Properties
        public MenuItem MenuItem => _menuItem;
        public string IconPath => _menuItem.IconPath;
        public string Name => _menuItem.Name;
        public string Title => _menuItem.Title;
        public int Level => _menuItem.Level;
        public string NavigationNam => _menuItem.NavigationName;
        public bool IsSelectedOnInitialize => _menuItem.IsSelectedOnInitialize;

        public HamburgerMenuSideBarItemViewModel Parent => _parent;
        protected ObservableCollection<HamburgerMenuSideBarItemViewModel> _children;
        public ObservableCollection<HamburgerMenuSideBarItemViewModel> Children => _children;

        public bool HasTitle => !string.IsNullOrEmpty(_menuItem.Title);
        public bool HasIcon => !string.IsNullOrEmpty(IconPath);
        public bool Isleaf => _children.Count <= 0;

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    if (Isleaf && _isSelected)
                    {
                        _eventAggregator.GetEvent<OnBuildHamburgerMenuSideBarWorkspaceViewEvent>().Publish(new() { CurrentMenuItem = _menuItem });
                    }
                }
            }
        }

        public PackIconKind IconKind 
        {
            get
            {
               PackIconKind kind = PackIconKind.None;

                _ = Enum.TryParse(_menuItem.IconKind, out kind);

                return kind;
            }
        }

        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => SetProperty<bool>(ref _isPaneOpen, value);
        }

        protected bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;

            set => SetProperty<bool>(ref _isEnabled, value);
        }
        #endregion

        #region Mouse Left Button Down Event
        public void ExecuteNavigationItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Isleaf)
            {
                IsSelected = true;
                //if (SetProperty<bool>(ref _isSelected, value))
                //{
                //    if (Isleaf && _isSelected)
                //    {
                //        _eventAggregator.GetEvent<OnBuildHamburgerMenuSideBarWorkspaceViewEvent>().Publish(new() { CurrentMenuItem = _menuItem });
                //    }
                //}
            }
        }
        #endregion
    }
}
