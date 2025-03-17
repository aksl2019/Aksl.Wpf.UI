using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Unity;

using Aksl.Modules.HamburgerMenuTreeBar.Views;
using Aksl.Modules.HamburgerMenuTreeBar.ViewModels;

namespace Aksl.Modules.HamburgerMenuTreeBar 
{
    public class HamburgerMenuTreeBarViewModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public HamburgerMenuTreeBarViewModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule 成员
        public void RegisterTypes(IContainerRegistry containerRegistry)
        { 
            containerRegistry.RegisterForNavigation<HamburgerMenuTreeBarHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(HamburgerMenuTreeBarHubView).ToString(),
                                               () => this._container.Resolve<HamburgerMenuTreeBarHubViewModel>());
        }
        #endregion
    }
}
