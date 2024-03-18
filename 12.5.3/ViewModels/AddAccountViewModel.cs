using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
	public class AddAccountViewModel: INotifyPropertyChanged
	{
		private readonly IBankAccountFactory _bankAccountFactory;
		public Account NewAccount { get; private set; }
		private string _familyName;
		public string FamilyName { get { return _familyName; }
			set
			{
				_familyName= value;
				OnPropertyChanged(nameof(FamilyName)); 
				(AddAccountCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			}
		}
		private string _firstName;
		public string FirstName { get { return _firstName; }
			set
			{
				_firstName= value;
				OnPropertyChanged(nameof(FirstName));
				(AddAccountCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			} 
		}
		private string _patronomic;
		public string Patronymic { get { return _patronomic; }
			set
				{
				_patronomic= value;
				OnPropertyChanged(nameof(Patronymic));
				(AddAccountCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			}
			}
		private string _numberPhone;
		public string NumberPhone { get { return _numberPhone; }
			set
			{
				_numberPhone= value;
				OnPropertyChanged(nameof(NumberPhone));
				(AddAccountCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			} 
		}
		private string _serialDoc;
		public string SerialDoc { get { return _serialDoc; }
			set
			{
				_serialDoc= value;
				OnPropertyChanged(nameof(SerialDoc));
				(AddAccountCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			} }
		private string _numberDoc;
		public string NumberDoc { get { return _numberDoc; }
			set
			{
				_numberDoc= value;
				OnPropertyChanged(nameof(NumberDoc));
				(AddAccountCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			}
		}
		private string _balanceText;
		public string BalanceText { get { return _balanceText; }
			set
			{
				_balanceText= value;
				OnPropertyChanged(nameof(BalanceText));
				(AddAccountCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			} }
		private string _accountType;
		public string AccountType { get { return _accountType; }
			set
			{
				_accountType = value;
				OnPropertyChanged(nameof(AccountType));
				(AddAccountCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			}
				 }
		public List<string> AccountTypes { get; set; }
		public Action<bool> CloseAction { get; set; }

		public ICommand AddAccountCommand { get; private set; }
		public AddAccountViewModel(IBankAccountFactory bankAccountFactory)
		{
			_bankAccountFactory = bankAccountFactory;
			this.AddAccountCommand = new _12._5._3.Models.RelayCommand(obj => this.OkExecute(obj), obj => this.CanOkExecute(obj));
			AccountTypes = new List<string> { "Депозитный", "Не депозитный" };
		}
		private bool CanOkExecute(object obj)
		{
			return !string.IsNullOrEmpty(FamilyName) &&
		   !string.IsNullOrEmpty(FirstName) &&
		   !string.IsNullOrEmpty(Patronymic) &&
		   !string.IsNullOrEmpty(NumberPhone) &&
		   !string.IsNullOrEmpty(SerialDoc) &&
		   !string.IsNullOrEmpty(NumberDoc) &&
		   !string.IsNullOrEmpty(BalanceText) &&
		   !string.IsNullOrEmpty(AccountType);
		}
		private void OkExecute(object obj)
		{
			try
			{
				string serialNumberDoc = SerialDoc + " " + NumberDoc;
				if (!double.TryParse(BalanceText.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double balance))
				{
					throw new NotANumberException();
				}

				NewAccount = new Account(FamilyName, FirstName, Patronymic, NumberPhone, serialNumberDoc, new ObservableCollection<BankAccount> { });
				BankAccount newBankAccount = _bankAccountFactory.Create(balance, AccountType);
				NewAccount.BankAccounts.Add(newBankAccount);
				CloseAction(true);
			}
			catch (NotANumberException ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
