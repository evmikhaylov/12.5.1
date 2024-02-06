using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _12._5._1
{
	public class Bank
	{
		private Dictionary<string, Account> accounts = new Dictionary<string, Account>();
		private string filePath = "Accounts.txt";
		private string nextIdPath = "NextId.txt";
		private int nextId = 1;

		public void OpenAccount (decimal balance, string fullName)
		{
			string id = nextId.ToString();
			nextId++;
			
			accounts[id] = new Account(id, balance,	fullName);
			SaveAccounts();
			
			
		}
		
		public void CloseAccount (string id)
		{
				accounts.Remove(id);
				SaveAccounts();
			
		}
		public void Transfer (string fromId, string toId, decimal amount)
		{
				accounts[fromId].Withdraw(amount);
				accounts[toId].Deposit(amount);
				SaveAccounts();
			
		}

		private void SaveAccounts()
		{
			var lines = accounts.Values.Select(a => $"{a.Id};{a.Balance};{a.FullName}");
			File.WriteAllLines(filePath, lines);
			File.WriteAllText("NextId.txt", nextId.ToString());
		}

		public void LoadAccounts()
		{
			using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
			{ fs.Close(); }
			using (FileStream fs = new FileStream(nextIdPath, FileMode.OpenOrCreate))
			{ fs.Close(); }

			nextId = int.Parse(File.ReadAllText(nextIdPath));

			var lines = File.ReadAllLines(filePath);
			foreach(var line in lines)
				{
					var parts = line.Split(';');
					if(parts.Length == 3)
					{
						string id = parts[0];
						decimal balance = decimal.Parse(parts[1]);
						string fullName = parts[2];
						accounts[id] = new Account(id, balance, fullName);
					}
				}
			
		}
		public Account GetAccount(string id)
		{
				return accounts[id];
		}
		public IEnumerable<Account> GetAccounts()
		{
			return accounts.Values;
		}
	}
}
