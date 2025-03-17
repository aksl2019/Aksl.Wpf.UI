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
        private readonly Dictionary<string, string> _errors;
        #endregion

        #region Constructors
        public AirCompresserViewModel()
        {
            _errors = new();

            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            CreateLoginCommand();
        }
        #endregion

        #region Properties

        private string _userName;
        [Required(ErrorMessage = "用户名不能为空")]
        [RegularExpression("^[a-zA-Z]{1}([a-zA-Z0-9]){3,15}$", ErrorMessage = "用户名必须是4到16个字母或者\n\r数字,且以字母开头.")]
        public string UserName
        {
            get => _userName;
            set => SetProperty<string>(ref _userName, value);
        }

        private string _password;
        [Required(ErrorMessage = "密码不能为空")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[$@$!%#?&])[a-zA-Z\d$@$!%#?&]{8,}$", ErrorMessage = "密码至少8个字符,必须包含一个字母,\n\r一个数字,一个特殊字符.")]
        public string Password
        {
            get => _password;
            set => SetProperty<string>(ref _password, value);
        }

        private bool _isLoading=false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty<bool>(ref _isLoading, value))
                {
                    (LoginCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool HasErrors => _errors.Count > 0;
        #endregion

        #region Login Command
        public ICommand LoginCommand { get; private set; }

        private void CreateLoginCommand()
        {
            LoginCommand = new DelegateCommand(async () =>
            {
               await  ExecuteLoginCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecuteLoginCommand();
                return canExecute;
            });
        }

        private async Task ExecuteLoginCommandAsync()
        {
            IsLoading = true;

            try
            {
                StatusMessage = "Logining.......";


            
                await Task.Delay(TimeSpan.FromMilliseconds(1000)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "Login In Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecuteLoginCommand()
        {
            return !IsLoading && !HasErrors;
        }
        #endregion

        #region Remove LoginView Method
 
        #endregion
    }
}
