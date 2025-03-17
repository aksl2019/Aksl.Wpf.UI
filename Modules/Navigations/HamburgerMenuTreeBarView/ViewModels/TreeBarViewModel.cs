using System.Collections.ObjectModel;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.HamburgerMenuTreeBar.ViewModels
{
    public class TreeBarViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private MenuItem _parentMenuItem;
        #endregion

        #region Constructors
        public TreeBarViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            TreeBarItems = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<TreeBarItemViewModel> TreeBarItems { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Create TreeBarItem ViewModel Method
        internal void CreateTreeBarItemViewModels(MenuItem parentMenuItem)
        {
            IsLoading = true;

            _parentMenuItem = parentMenuItem;

            foreach (var sm in _parentMenuItem.SubMenus)
            {
                TreeBarItemViewModel treeBarItemViewModel = new(_eventAggregator, sm);

                TreeBarItems.Add(treeBarItemViewModel);
            }

            IsLoading = false;
        }
        #endregion
    }
}
