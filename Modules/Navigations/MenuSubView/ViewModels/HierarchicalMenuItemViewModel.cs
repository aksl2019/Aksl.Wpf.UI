using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;

namespace Aksl.Modules.MenuSub.ViewModels
{
    public class HierarchicalMenuItemViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        protected readonly HierarchicalMenuItemViewModel _parent;
        private readonly MenuItem _menuItem;
        #endregion

        #region Constructors
        public HierarchicalMenuItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem) : this(eventAggregator, menuItem, null)
        {
            RaisePropertyChanged(nameof(Isleaf));
        }

        public HierarchicalMenuItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem, HierarchicalMenuItemViewModel parent)
        {
            _eventAggregator = eventAggregator;
            _menuItem = menuItem;
            _parent = parent;

            _children = new((from child in _menuItem.SubMenus
                             select new HierarchicalMenuItemViewModel(eventAggregator, child, this)).ToList<HierarchicalMenuItemViewModel>());

            RaisePropertyChanged(nameof(Isleaf));
        }
        #endregion

        #region Properties
        public int Id => _menuItem.Id;
        public string IconPath => _menuItem.IconPath; 
        public string Name => _menuItem.Name;
        public string Title => _menuItem.Title;
        public int Level => _menuItem.Level;
        public string NavigationNam => _menuItem.NavigationName;
        public bool IsSelectedOnInitialize => _menuItem.IsSelectedOnInitialize;

        public HierarchicalMenuItemViewModel Parent => _parent;

        protected ObservableCollection<HierarchicalMenuItemViewModel> _children;
        public ObservableCollection<HierarchicalMenuItemViewModel> Children => _children;

        public bool HasTitle => !string.IsNullOrEmpty(_menuItem.Title);
        public bool HasIcon => !string.IsNullOrEmpty(IconPath);
        public bool Isleaf => _children.Count <= 0;
        public bool IsSeparator => _menuItem.IsSeparator;

        protected bool _onlyIsSelected=false;
        public bool OnlyIsSelected
        {
            get => _onlyIsSelected;
            set => SetProperty(ref _onlyIsSelected, value);
        }

        protected bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    if (!OnlyIsSelected && Isleaf && _isSelected)
                    {
                        _eventAggregator.GetEvent<OnMenuSubContentChangedViewEvent>().Publish(new OnMenuSubContentChangedViewEvent { CurrentMenuItem = _menuItem });
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
        public void ExecuteMenuItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Isleaf)
            {
                if (OnlyIsSelected)
                {
                    OnlyIsSelected = false;

                    _eventAggregator.GetEvent<OnMenuSubContentChangedViewEvent>().Publish(new OnMenuSubContentChangedViewEvent { CurrentMenuItem = _menuItem });
                }
                else
                {
                    IsSelected = true; 
                }
            }
        }
        #endregion

        #region Force Child Enabled Method
        internal void ForceChildEnabled(HierarchicalMenuItemViewModel menuItemViewModel, bool isEnabled)
        {
            if (HasChild(menuItemViewModel))
            {
                RecursiveSubMenuItemViewModel(menuItemViewModel);
            }

            void RecursiveSubMenuItemViewModel(HierarchicalMenuItemViewModel parentMenuItemViewModel)
            {
                foreach (var children in parentMenuItemViewModel.Children)
                {
                    children.IsEnabled = isEnabled;

                    ForceChildEnabled(children, isEnabled);
                }
            }

            bool HasChild(HierarchicalMenuItemViewModel hmivm) => hmivm.Children.Any();
        }
        #endregion
    }
}