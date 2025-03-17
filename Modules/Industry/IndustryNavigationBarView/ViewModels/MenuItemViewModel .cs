using System;
using System.Windows.Input;

using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;

using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;

namespace Aksl.Modules.IndustryNavigationBar.ViewModels
{
    public class MenuItemViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        private readonly MenuItem _menuItem;
        #endregion

        #region Constructors
        //public MenuItemViewModel(MenuItem menuItem)
        //{
        //    _menuItem = menuItem;
        //}

        public MenuItemViewModel(IEventAggregator eventAggregator, int groupIndex, int index, MenuItem menuItem)
        {
            _eventAggregator = eventAggregator;
            GroupIndex = groupIndex;
            Index = index;
            _menuItem = menuItem;
        }
        #endregion

        #region Properties
        public int GroupIndex { get; }

        public int Index { get; }

        public MenuItem MenuItem => _menuItem;

        public string Title => _menuItem.Title;
        public bool Isleaf => _menuItem.SubMenus.Count <= 0;

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
                        _eventAggregator.GetEvent<OnBuildIndustryWorkspaceViewEvent>().Publish(new OnBuildIndustryWorkspaceViewEvent { CurrentMenuItem = _menuItem });
                    }
                }
            }
        }

        protected bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty<bool>(ref _isEnabled, value);
        }
        #endregion
    }
}
