using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.HamburgerMenuTreeSideBar.ViewModels
{
    public class TreeSideBarViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;
        #endregion

        #region Constructors
        public TreeSideBarViewModel(IEventAggregator eventAggregator, IMenuService menuService)
        {
            _eventAggregator = eventAggregator;
            _menuService = menuService;

            TopTreeSideBarItems = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<TreeSideBarItemViewModel> TopTreeSideBarItems { get; }

        internal TreeSideBarItemViewModel _previewSelectedTreeSideBarItem;
        internal TreeSideBarItemViewModel PreviewSelectedTreeSideBarItem => _previewSelectedTreeSideBarItem;

        private TreeSideBarItemViewModel _selectedTreeSideBarItemViewModel;
        public TreeSideBarItemViewModel SelectedTreeSideBarItem
        {
            get => _selectedTreeSideBarItemViewModel;
            set
            {
                if (SetProperty(ref _selectedTreeSideBarItemViewModel, value))
                {
                    if (_selectedTreeSideBarItemViewModel is not null)
                    {
                        _selectedTreeSideBarItemViewModel.IsSelected = true;
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

        #region Reset/Clear Selected TreeSideBarItem Method
        internal void ClearSelectedTreeSideBarItem()
        {
            if (SelectedTreeSideBarItem is not null)
            {
                SelectedTreeSideBarItem.IsSelected = false;
                SelectedTreeSideBarItem = null;
                _previewSelectedTreeSideBarItem = null;
            }
        }

        internal void ResetSelectedTreeSideBarItem(TreeSideBarItemViewModel selectedTreeSideBarItem)
        {
            if (selectedTreeSideBarItem is not null)
            {
                if (_selectedTreeSideBarItemViewModel is not null)
                {
                    _selectedTreeSideBarItemViewModel.IsSelected = false;
                }

                _previewSelectedTreeSideBarItem = null;
                _selectedTreeSideBarItemViewModel = selectedTreeSideBarItem;
                _selectedTreeSideBarItemViewModel.IsSelected = true;
            }
        }
        #endregion

        #region Create TreeSideBarItem ViewModel Method
        internal async Task CreateTreeSideBarItemViewModelsAsync()
        {
            IsLoading = true;

            var rootMenuItem = await _menuService.GetMenuAsync("All");

            var subMenuItems = rootMenuItem.SubMenus;
            foreach (var smi in subMenuItems)
            {
                TreeSideBarItemViewModel treeSideBarItemViewModel = new(_eventAggregator, smi);
                AddPropertyChanged();

                TopTreeSideBarItems.Add(treeSideBarItemViewModel);

                void AddPropertyChanged()
                {
                    treeSideBarItemViewModel.PropertyChanged += (sender, e) =>
                    {
                        if (sender is TreeSideBarItemViewModel tsbivm)
                        {
                            if (e.PropertyName == nameof(TreeSideBarItemViewModel.IsSelected))
                            {
                                if (tsbivm.IsSelected)
                                {
                                    _selectedTreeSideBarItemViewModel = tsbivm;
                                }
                                else
                                {
                                    _previewSelectedTreeSideBarItem = tsbivm;
                                }
                            }
                        }
                    };
                }
            }

            IsLoading = false;
        }
        #endregion
    }
}
