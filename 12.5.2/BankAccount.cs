using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _12._5._2
{

	public class BankAccount
	{
		private static readonly Random random = new Random();
		public double Balance { get; private set; }
		public string BankAccountType { get; private set; }
		public uint NumberBankAccount { get; private set; }
		public BankAccount(double balance, string bankAccountType)
		{
			Balance=balance;
			NumberBankAccount=(uint)random.Next(1, int.MaxValue);
			BankAccountType = bankAccountType;
		}

		public virtual void Deposit(double amount)
		{
			Balance+=amount;
		}
		public virtual void Withdraw(double amount)
		{
			Balance -=amount;
		}
		public static void Transfer(BankAccount fromNumberBankAccount, BankAccount toNumberBankAccount, double amount)
		{
			fromNumberBankAccount.Withdraw(amount);
			toNumberBankAccount.Deposit(amount);
		}
	}

	public interface IBankAccountFactory
	{
		BankAccount Create(double balance, string accountType);
	}

	public class BankAccountFactory : IBankAccountFactory
	{
		public BankAccount Create(double balance, string accountType)
		{
			switch(accountType)
			{
				case "Депозитный":
					return new DepositBankAccount(balance, accountType);
				case "Не депозитный":
					return new NonDepositBankAccount(balance, accountType);
				default:
					return null;
			}
		
		}
	}

	public class TransferService
	{
		public void Transfer(BankAccount from, BankAccount to, double amount)
		{
			BankAccount.Transfer(from, to, amount);
		}
	}

	public class BankAccountService
	{
		public void Deposit (BankAccount bankAccount, double amount)
		{
			bankAccount.Deposit(amount);
		}

		public void Withdraw(BankAccount bankAccount, double amount)
		{
			bankAccount.Withdraw(amount);
		}

	}

	public class DepositBankAccount : BankAccount
	{
		public DepositBankAccount(double balance, string bankAccountType)
			: base(balance, "Депозитный")
		{
		}
	}

	public class NonDepositBankAccount : BankAccount
	{
		private const double ComissionRate = 0.05;
		public NonDepositBankAccount(double balance, string bankAccountType)
			: base(balance * (1- ComissionRate), "Не депозитный")
		{
		}
		public override void Deposit(double amount)
		{
			double amountAfterComission = amount * (1-ComissionRate);
			base.Deposit(amountAfterComission);
		}
	}
}
