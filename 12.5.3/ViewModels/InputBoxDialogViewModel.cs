using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Models;


namespace _12._5._3.ViewModels
{
	public class InputBoxDialogViewModel : INotifyPropertyChanged
	{
		private string _inputText;
		public string InputText
		{
			get { return _inputText; }
			set
			{
				if (_inputText != value)
				{
					_inputText = value;
					OnPropertyChanged();
					(OkCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
				}
			}
		}
		public Action<bool> CloseAction { get; set; }
		public ICommand OkCommand { get; private set; }
		public IMessageService _messageService { get; private set; }

		public InputBoxDialogViewModel(IMessageService messageService)
		{
			this._messageService = messageService;
			OkCommand = new _12._5._3.Models.RelayCommand(obj => OkExecute(), obj => CanOkExecute());
		}
		private bool CanOkExecute()
		{
			return !string.IsNullOrEmpty(InputText);
		}


		private void OkExecute()
		{
			if (string.IsNullOrEmpty(_inputText) || !double.TryParse(_inputText.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out _))
			{
				_messageService.ShowMessage("Введите корректную сумму");
			}
			else
			{
				CloseAction(true);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
