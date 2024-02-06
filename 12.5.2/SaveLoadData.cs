using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _12._5._2
{
	public interface ISaveLoad
	{
		void SaveData(List<Account> accounts, string filePath);
		List<Account> LoadData(string filePath);
	}
	public class SaveLoadData : ISaveLoad
	{
		public void SaveData(List<Account> accounts, string filePath)
		{
			string json = JsonConvert.SerializeObject(accounts);
			File.WriteAllText(filePath, json);
		}
		public List<Account> LoadData(string filePah)
		{
			var stream = File.Open(filePah, FileMode.OpenOrCreate);
			if (stream.Length == 0)
			{
				stream.Close();
				return new List<Account>();
			}
			stream.Close();
			string json = File.ReadAllText(filePah);
			List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(json);
			return accounts ?? new List<Account> ();

		}
	}
	
}
