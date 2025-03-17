using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unity;

using Aksl.Infrastructure;
using Aksl.Toolkit.Services;
using Aksl.Infrastructure.Events;

namespace Aksl.Modules.AirCompresser.ViewModels
{
    public class AirCompresserViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogViewService _dialogViewService; 
        #endregion

        #region Constructors
        public AirCompresserViewModel()
        {
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();
        }
        #endregion

        #region Properties
        public IEnumerable<CompressStatus> CompressStatusList
        {
            get => Enum.GetValues(typeof(CompressStatus)).Cast<CompressStatus>().ToList();
        }

        private CompressStatus _selectedCompressStatus = CompressStatus.Normal;
        public CompressStatus SelectedCompressStatus
        {
            get => _selectedCompressStatus;
            set => SetProperty<CompressStatus>(ref _selectedCompressStatus, value);
        }
        #endregion
    }
}
