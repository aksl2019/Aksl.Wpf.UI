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

namespace Aksl.Modules.Pipeline.ViewModels
{
    public class PipelineHubViewModel : BindableBase,INavigationAware
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogViewService _dialogViewService;
        #endregion

        #region Constructors
        public PipelineHubViewModel()
        {
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            PipelineViewModel = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<PipelineViewModel>();

            //CreateWestToEastFlowCommand();
            //CreateEastToWestFlowCommand();
            //CreatePauseFlowCommand();
            CreateSetCommand();

            RegisterPropertyChanged();

            //RaisePropertyChanged(nameof(PipelineViewModel));
            //Pipeline.Direction = WaterDirection.EastToWest;
        }
        #endregion

        #region Properties
        public PipelineViewModel PipelineViewModel { get; private set; }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty<bool>(ref _isLoading, value))
                {
                    //(WestToEastFlowCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                    //(EastToWestFlowCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                    //(PauseFlowCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                    (SetCommand as DelegateCommand<object>)?.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region RegisterPropertyChanged Method
        private void RegisterPropertyChanged()
        {
            PipelineViewModel.PropertyChanged += (sender, e) =>
            {
                if (sender is PipelineViewModel plvm)
                {
                    if (e.PropertyName == nameof(PipelineViewModel.SelectedWaterDirection))
                    {
                        //(WestToEastFlowCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                        //(EastToWestFlowCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                        //(PauseFlowCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                        (SetCommand as DelegateCommand<object>)?.RaiseCanExecuteChanged();
                    }
                }
            };
        }
        #endregion

        #region Set Command
        public ICommand SetCommand { get; private set; }

        private void CreateSetCommand()
        {
            SetCommand = new DelegateCommand<object>(async (o) =>
            {

                await ExecuteSetCommandAsync(o);
            },
            (o) =>
            {
                var canExecute =  CanExecuteSetCommand(o);
                return canExecute;
            });
        }

        private async Task ExecuteSetCommandAsync(object direction)
        {
            IsLoading = true;

            try
            {
                //if (direction is not null && (WaterDirection)direction is WaterDirection wd)
                //{
                //    PipelineViewModel.SelectedWaterDirection = wd;
                //}

                if ((string)direction is string d)
                {
                    if (d.Equals("WE", StringComparison.InvariantCultureIgnoreCase))
                    {
                        PipelineViewModel.SelectedWaterDirection = WaterDirection.WestToEast;
                    }
                    else if (d.Equals("EW", StringComparison.InvariantCultureIgnoreCase))
                    {
                        PipelineViewModel.SelectedWaterDirection = WaterDirection.EastToWest;
                    }
                    else if (d.Equals("PL", StringComparison.InvariantCultureIgnoreCase))
                    {
                        PipelineViewModel.SelectedWaterDirection = WaterDirection.Pause;
                    }
                }

                await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "West To East Flow Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecuteSetCommand(object direction)
        {
            var canExecute =false;

            //if (direction is not null && (WaterDirection)direction is WaterDirection wd)
            //{
            //    canExecute = !IsLoading && PipelineViewModel.SelectedWaterDirection != wd;
            //}
            if ((string)direction is string d)
            {
                if (d.Equals("WE", StringComparison.InvariantCultureIgnoreCase))
                {
                    canExecute = !IsLoading && !(PipelineViewModel.SelectedWaterDirection == WaterDirection.WestToEast);
                }
                else if (d.Equals("EW", StringComparison.InvariantCultureIgnoreCase))
                {
                    canExecute = !IsLoading && !(PipelineViewModel.SelectedWaterDirection == WaterDirection.EastToWest);
                }
                else if (d.Equals("PL", StringComparison.InvariantCultureIgnoreCase))
                {
                    canExecute = !IsLoading && !(PipelineViewModel.SelectedWaterDirection == WaterDirection.Pause);
                }
            }

            return canExecute;
        }
        #endregion

        #region WestToEast Flow Command
        public ICommand WestToEastFlowCommand { get; private set; }

        private void CreateWestToEastFlowCommand()
        {
            WestToEastFlowCommand = new DelegateCommand(async () =>
            {
                await ExecuteWestToEastFlowCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecuteWestToEastFlowCommand();
                return canExecute;
            });
        }

        private async Task ExecuteWestToEastFlowCommandAsync()
        {
            IsLoading = true;

            try
            {

                PipelineViewModel.SelectedWaterDirection = WaterDirection.WestToEast;

                await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "West To East Flow Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecuteWestToEastFlowCommand()
        {
            return !IsLoading && !(PipelineViewModel.SelectedWaterDirection == WaterDirection.WestToEast);
        }
        #endregion

        #region EastToWest Flow Command
        public ICommand EastToWestFlowCommand { get; private set; }

        private void CreateEastToWestFlowCommand()
        {
            EastToWestFlowCommand = new DelegateCommand(async () =>
            {
                await ExecuteEastToWestFlowCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecuteEastToWestFlowCommand();
                return canExecute;
            });
        }

        private async Task ExecuteEastToWestFlowCommandAsync()
        {
            IsLoading = true;

            try
            {

                PipelineViewModel.SelectedWaterDirection = WaterDirection.EastToWest;

                await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "East To West Flow Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecuteEastToWestFlowCommand()
        {
            return !IsLoading && !(PipelineViewModel.SelectedWaterDirection == WaterDirection.EastToWest);
        }
        #endregion

        #region Pause Flow Command
        public ICommand PauseFlowCommand { get; private set; }

        private void CreatePauseFlowCommand()
        {
            PauseFlowCommand = new DelegateCommand(async () =>
            {
                await ExecutePauseFlowCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecutePauseFlowCommand();
                return canExecute;
            });
        }

        private async Task ExecutePauseFlowCommandAsync()
        {
            IsLoading = true;

            try
            {

                PipelineViewModel.SelectedWaterDirection = WaterDirection.Pause;

                await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "Pause Flow Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecutePauseFlowCommand()
        {
            return !IsLoading && !(PipelineViewModel.SelectedWaterDirection is WaterDirection.Pause);
        }
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
        #endregion
    }
}
