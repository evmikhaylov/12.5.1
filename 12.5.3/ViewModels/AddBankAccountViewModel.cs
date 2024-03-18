using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using _12._5._3.Models;
using GalaSoft.MvvmLight.Command;
using Models;

namespace _12._5._3.ViewModels
{
	public class AddBankAccountViewModel : INotifyPropertyChanged
	{
		private readonly IBankAccountFactory _bankAccountFactory;
		private readonly Account _account;
		public BankAccount NewBankAccount { get; private set; }
		public string AccountType { get; set;}
		private string _amountText;
		public string AmountText { get { return _amountText; }
			set
			{
				_amountText = value;
				OnPropertyChanged(nameof(AmountText));
				(OkCommand as GalaSoft.MvvmLight.Command.RelayCommand)?.RaiseCanExecuteChanged();
			}
				}
		public List<string> AccountTypes { get; set; }
		public ICommand OkCommand { get; private set; }
		public Action<bool> CloseAction { get ; set; }
		
		public AddBankAccountViewModel(Account account, IBankAccountFactory bankAccountFactory)
		{
			_account = account;
			_bankAccountFactory = bankAccountFactory;
			AccountTypes =new List<string>();
			if (_account.BankAccounts.Any(ba => ba.BankAccountType == "Депозитный"))
			{
				AccountTypes.Add("Не депозитный");
				AccountType = AccountTypes.FirstOrDefault();
			}
			else if (_account.BankAccounts.Any(ba => ba.BankAccountType == "Не депозитный"))
			{
				AccountTypes.Add("Депозитный");
				AccountType = AccountTypes.FirstOrDefault();
			}
			else
			{
				AccountTypes.Add("Депозитный");
				AccountTypes.Add("Не депозитный");
				AccountType = AccountTypes.FirstOrDefault();
			}
			
			
			this.OkCommand = new _12._5._3.Models.RelayCommand(obj => this.OkExecute(), obj => this.CanOkExecute());
		}
		private bool CanOkExecute()
		{
			return !string.IsNullOrEmpty(AmountText);
		}

		private void OkExecute()
		{
			try
			{
				string amountText = AmountText.Replace(",", ".");
				if (!double.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out double amount))
				{
					throw new NotANumberException();
				}

				NewBankAccount = _bankAccountFactory.Create(amount, AccountType);
				CloseAction(true);
			}
			catch(NotANumberException ex)
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
