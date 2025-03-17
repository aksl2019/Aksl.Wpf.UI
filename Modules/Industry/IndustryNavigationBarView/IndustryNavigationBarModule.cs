using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

using Aksl.Modules.IndustryNavigationBar.Views;
using Aksl.Modules.IndustryNavigationBar.ViewModels;

namespace Aksl.Modules.IndustryNavigationBar
{
    public class IndustryNavigationBarModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public IndustryNavigationBarModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule 成员
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<IndustryNavigationBarHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(IndustryNavigationBarHubView).ToString(),
                                               () => this._container.Resolve<IndustryNavigationBarHubViewModel>());
        }
        #endregion
    }
}
