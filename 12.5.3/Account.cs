using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _12._5._3
{
	interface IAccount<out T> where T : BankAccount
	{
		T GetAccount();
	}

	public class Account:IAccount<BankAccount>
	{
		public BankAccount SelectedBankAccount { get; set; }
		private static ulong nextId = 1;
		public string FamilyName { get; set; }
		public string FirstName { get; set; }
		public string Patronymic { get; set; }
		public string NumberPhone { get; set; }
		public string SerialNumberDoc { get; set; }
		public ulong Id { get; private set; }
		public List<BankAccount> BankAccounts { get;  set; }

		public Account (string familyName, string firstName, string patronymic, string numberPhone, string serialNumberDoc, List<BankAccount> bankAccounts)
		{
			FamilyName = familyName;
			FirstName = firstName;
			Patronymic = patronymic;
			NumberPhone = numberPhone;
			SerialNumberDoc = serialNumberDoc;
			Id=nextId++;
			BankAccounts=bankAccounts;
		}
		public BankAccount GetAccount()
		{
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
	} 

	public class DepositAccount : IAccount<DepositBankAccount> 
	{
		private readonly DepositBankAccount _account;

		public DepositAccount(DepositBankAccount account)
		{
			_account=account;
		}
		public DepositBankAccount GetAccount()
		{ return _account; }
	}

	public class NonDepositAccount : IAccount<NonDepositBankAccount>
	{
		private readonly NonDepositBankAccount _account;

		public NonDepositAccount(NonDepositBankAccount account)
		{
			_account=account;
		}
		public NonDepositBankAccount GetAccount () { return _account; }
	}
}
