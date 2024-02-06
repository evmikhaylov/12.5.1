using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _12._5._1
{
	public class Account
	{
		public string Id { get; private set; }
		public decimal Balance { get; private set; }
		public string FullName { get; private set; }

		public Account(string id, decimal balance, string fullName)
		{
			Id=id;
			Balance=balance;
			FullName=fullName;
		}

		public void Deposit (decimal amount)
		{
			Balance += amount;
		}

		public void Withdraw (decimal amount)
		{

				Balance -= amount;
			
		}
	}
}
