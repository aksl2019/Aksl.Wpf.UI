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

using Aksl.Toolkit.Services;
using Aksl.Toolkit;

namespace Aksl.Modules.Pipeline.ViewModels
{
    public class CoolingPieViewModel : BindableBase
    {
        #region Members
        #endregion

        #region Constructors
        public CoolingPieViewModel()
        {
            //  RaisePropertyChanged(nameof(Status));
        }
        #endregion

        #region Properties
        private int _centerX = 20;
        public int CenterX
        {
            get => _centerX;
            set => SetProperty<int>(ref _centerX, value);
        }

        private int _centerY = 20;
        public int CenterY
        {
            get => _centerY;
            set => SetProperty<int>(ref _centerY, value);
        }

        public List<FanStatus> FanStatusList
        {
            get => Enum.GetValues(typeof(FanStatus)).Cast<FanStatus>().ToList();
        }

        private FanStatus _selectedFanStatus = FanStatus.Turn;
        public FanStatus SelectedFanStatus
        {
            get => _selectedFanStatus;
            set => SetProperty<FanStatus>(ref _selectedFanStatus, value);
        }

        //private FanStatus _fanStatusStatus = FanStatus.Turn;
        //public FanStatus Status
        //{
        //    get => _fanStatusStatus;
        //    set => SetProperty<FanStatus>(ref _fanStatusStatus, value);
        //}
        #endregion
    }
}
