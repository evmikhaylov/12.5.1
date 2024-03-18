using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace _12._5._3
{
	interface IAccount<T> where T : BankAccount
	{
		T GetSelectedBankAccount(BankAccount bankAccount);
	}

	public class Account:IAccount<BankAccount>, INotifyPropertyChanged
	{
		private BankAccount _selectedBankAccount;
		public BankAccount SelectedBankAccount
		{
			get { return _selectedBankAccount; }
			set
			{
				if (_selectedBankAccount != value)
				{
					_selectedBankAccount = value;
					OnPropertyChanged("SelectedBankAccount");
				}
			}
		}
		private static ulong nextId = 1;
		public string FamilyName { get; set; }
		public string FirstName { get; set; }
		public string Patronymic { get; set; }
		public string NumberPhone { get; set; }
		public string SerialNumberDoc { get; set; }
		public ulong Id { get; private set; }
		public ObservableCollection<BankAccount> BankAccounts { get;  set; }
		public string FullName
		{
			get { return $"{FamilyName} {FirstName} {Patronymic}"; }
		}

		public Account (string familyName, string firstName, string patronymic, string numberPhone, string serialNumberDoc, ObservableCollection<BankAccount> bankAccounts)
		{
			FamilyName = familyName;
			FirstName = firstName;
			Patronymic = patronymic;
			NumberPhone = numberPhone;
			SerialNumberDoc = serialNumberDoc;
			Id=nextId++;
			BankAccounts=bankAccounts;
		}
		public BankAccount GetSelectedBankAccount(BankAccount bankAccount)
		{
			SelectedBankAccount = BankAccounts.FirstOrDefault(ba => ba == bankAccount);
			return SelectedBankAccount;
		}
		public void AddAccount (BankAccount bankAccount)
		{
			BankAccounts.Add(bankAccount);
		}

		public void RemoveAccount (BankAccount bankAccount)
		{
			BankAccounts.Remove(bankAccount);
		}
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	} 

	public class DepositAccount : IAccount<DepositBankAccount> 
	{
		private readonly DepositBankAccount _account;

		public DepositAccount(DepositBankAccount account)
		{
			_account=account;
		}
		public DepositBankAccount GetSelectedBankAccount(BankAccount bankAccount)
		{ return _account; }
	}

	public class NonDepositAccount : IAccount<NonDepositBankAccount>
	{
		private readonly NonDepositBankAccount _account;

		public NonDepositAccount(NonDepositBankAccount account)
		{
			_account=account;
		}
		public NonDepositBankAccount GetSelectedBankAccount(BankAccount bankAccount) { return _account; }
	}
}
