using System.Collections.ObjectModel;
using System.Linq;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;

namespace Aksl.Modules.HamburgerMenuTreeSideBar.ViewModels
{
    public class TreeSideBarItemViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        protected readonly TreeSideBarItemViewModel _parent;
        private readonly MenuItem _menuItem;
        #endregion

        #region Constructors
        public TreeSideBarItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem) : this(eventAggregator, menuItem, null)
        {
        }

        public TreeSideBarItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem, TreeSideBarItemViewModel parent)
        {
            _eventAggregator = eventAggregator;
            _menuItem = menuItem;
            _parent = parent;

            _children = new((from child in _menuItem.SubMenus
                             select new TreeSideBarItemViewModel(eventAggregator, child, this)).ToList<TreeSideBarItemViewModel>());
        }
        #endregion

        #region Properties
        public string Title => _menuItem.Title;
        public int Level => _menuItem.Level;
        public TreeSideBarItemViewModel Parent => _parent;

        protected ObservableCollection<TreeSideBarItemViewModel> _children;
        public ObservableCollection<TreeSideBarItemViewModel> Children => _children;

        protected bool Isleaf => _children.Count <= 0;

        protected bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                SetProperty<bool>(ref _isExpanded, value);

                if (_isExpanded && Parent is not null)
                {
                    Parent.IsExpanded = true;
                }
            }
        }

        protected bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    if (Isleaf && _isSelected)
                    {
                        _eventAggregator.GetEvent<OnBuildHamburgerMenuTreeSideBarWorkspaceViewEvent>().Publish(new (){ CurrentMenuItem = _menuItem });
                    }
                }
            }
        }

        protected bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (SetProperty<bool>(ref _isEnabled, value))
                {
                    //ForceChildEnabled(this, _isEnabled);
                    foreach (var children in this.Children)
                    {
                        children.IsEnabled = _isEnabled;
                    }
                }
            }
        }
        #endregion

        #region Label Mouse Left Button Down Event
        public void ExecuteTreeSideBarItemClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Isleaf)
            {
                IsSelected = true;

                //_eventAggregator.GetEvent<OnBuildWorkspaceViewEvent>().Publish(new OnBuildWorkspaceViewEvent { CurrentMenuItem = _menuItem });
            }
        }
        #endregion
    }
}