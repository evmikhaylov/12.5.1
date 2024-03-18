using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _12._5._3
{
	public interface ISaveLoad
	{
		void SaveData(List<Account> accounts, string filePath);
		List<Account> LoadData(string filePath);
		void SaveLog(Dictionary<string, List<Change>> changeDict, string filePath);
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
		public void SaveLog(Dictionary<string, List<Change>> changesDict, string filePath)
		{
			Dictionary<string, List<Change>> allChanges = new Dictionary<string, List<Change>>();

			if (File.Exists(filePath))
			{
				var json = File.ReadAllText(filePath);
				allChanges =  JsonConvert.DeserializeObject<Dictionary<string, List<Change>>>(json);

				if (allChanges == null)
				{
					allChanges = new Dictionary<string, List<Change>>();
				}
			}

			foreach (var accountId in changesDict.Keys)
			{
				if (!allChanges.ContainsKey(accountId))
				{
					allChanges[accountId] = new List<Change>();
				}

				var lastChangeTime = allChanges[accountId].LastOrDefault()?.Time;

				foreach (var change in changesDict[accountId])
				{
					if (lastChangeTime == null || String.CompareOrdinal(change.Time, lastChangeTime) > 0)
					{
						allChanges[accountId].Add(change);
					}
				}
			}

			var updatedJson = JsonConvert.SerializeObject(allChanges);
			File.WriteAllText(filePath, updatedJson);
		}
	}
	
}
