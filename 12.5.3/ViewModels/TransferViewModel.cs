using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using _12._5._3.Models;
using GalaSoft.MvvmLight.Command;
using Models;

namespace _12._5._3.ViewModels
{
	public class TransferViewModel: INotifyPropertyChanged
	{
		private readonly ChangeLog _changeLog;
		private readonly ISaveLoad _saveLoad;
		private readonly string _filePathChange;
		private readonly Employee _employee;
		private List<Account> _accounts;
		private readonly TransferService<BankAccount> _transferService;
		private Account _fromAccount;
		private Account _toAccount;
		private IMessageService _messageService;
		
		public ICommand TransferCommand { get; }
		private string _amount;
		public string Amount { get { return _amount; } 
			set
			{
				_amount = value;
				OnPropertyChanged();
				(TransferCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			}
		}
		public List<Account> Accounts
		{
			get { return _accounts; }
			set
			{
				_accounts = value;
				OnPropertyChanged(nameof(Accounts));
			}
		}
		public Account FromAccount
		{ 
			get { return _fromAccount; }
			set
			{
				_fromAccount = value;
				UpdateFromBankAccounts();
				OnPropertyChanged(nameof(FromAccount));
				(TransferCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			}
		}
		public Account ToAccount 
		{
			get { return _toAccount; }
			set
			{
				_toAccount = value;
				UpdateToBankAccounts();
				OnPropertyChanged(nameof(ToAccount));
				(TransferCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			}
				}
		public List<BankAccount> FromBankAccounts { get; set; }
		public List<BankAccount> ToBankAccounts { get; set; }
		private BankAccount _selectedFromBankAccount;
		public BankAccount SelectedFromBankAccount { get { return _selectedFromBankAccount; } set
			{
				_selectedFromBankAccount = value;
				OnPropertyChanged();
				(TransferCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();

			} }
		private BankAccount _selectedToBankAccount;
		public BankAccount SelectedToBankAccount { get { return _selectedToBankAccount; }
			set
			{
				_selectedToBankAccount = value;
				OnPropertyChanged();
				(TransferCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			}
			}
		public Action<bool> CloseAction { get; set; }
		public TransferViewModel(List<Account> accounts, TransferService<BankAccount> transferService, ChangeLog changeLog, ISaveLoad saveLoad, string filePathChange, Employee employee, IMessageService messageService)
		{
			this._accounts = accounts;
			this._transferService = transferService;
			this._changeLog = changeLog;
			this._saveLoad = saveLoad;
			this._employee = employee;
			this._filePathChange = filePathChange;
			this.TransferCommand = new _12._5._3.Models.RelayCommand(param => this.PerformTransfer(), obj => CanPerformTransfer());
			this.Accounts = accounts;
			this.FromBankAccounts = new List<BankAccount>();
			this.ToBankAccounts = new List<BankAccount>();
			this._messageService = messageService;
		}
		private void UpdateFromBankAccounts()
		{
			
			var fromAccount = GetSelectedAccount(FromAccount);
			
			if (fromAccount != null)
			{
				FromBankAccounts = new List<BankAccount>(fromAccount.BankAccounts);				
			}
			OnPropertyChanged(nameof(FromBankAccounts));
		}

		private void UpdateToBankAccounts()
		{
			
			var toAccount = GetSelectedAccount(ToAccount);
			
			if (toAccount != null)
			{
				ToBankAccounts = new List<BankAccount>(toAccount.BankAccounts);
			}
			OnPropertyChanged(nameof(ToBankAccounts));
		}
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		

		private Account GetSelectedAccount(Account selectedAccount)
		{
			return _accounts.FirstOrDefault(a => a == selectedAccount);
		}
		public bool CanPerformTransfer()
		{
			return FromAccount != null && ToAccount != null && SelectedFromBankAccount != null && SelectedToBankAccount != null && Amount != null;

		}
		public void PerformTransfer()
		{
			try
			{
				Account fromAccount = GetSelectedAccount(FromAccount);
				Account toAccount = GetSelectedAccount(ToAccount);
				BankAccount fromBankAccount = fromAccount.GetSelectedBankAccount(SelectedFromBankAccount);
				BankAccount toBankAccount = toAccount.GetSelectedBankAccount(SelectedToBankAccount);
				if (!double.TryParse(Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out double amount))
				{
					throw new NotANumberException();
				}
				_changeLog.LogChange(Convert.ToString(fromAccount.Id), "Перевод", $"получатель {toAccount.Id} сумма :{Amount}", _employee.GetType().Name);
				_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
				_changeLog.LogChange(Convert.ToString(toAccount.Id), "Перевод", $"отправитель {fromAccount.Id} сумма :{Amount}", _employee.GetType().Name);
				_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
				_transferService.Transfer(fromBankAccount, toBankAccount, amount);
				CloseAction(true);
			}
			catch(NotANumberException ex)
			{
				_messageService.ShowMessage(ex.Message);
			}
			
			
		}

	}
}
