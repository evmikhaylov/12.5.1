using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _12._5._3
{
	public class Change
	{
		public string AccountId { get; set; }
		public string Action { get; set; }
		public string Fields { get; set; }
		public string Time { get; set; }
		public string ChangedBy { get; set; }
	}
	public class ChangeLog
	{
		public Dictionary<string, List<Change>> Changes { get; private set; }
		public delegate void ActionExecutedHandler(Change change);
		public event ActionExecutedHandler OnActionExecuted;
		public ChangeLog()
		{
			Changes = new Dictionary<string, List<Change>>();
		}

		public void LogChange(string accountId, string action, string fields, string changeBy)
		{

			if(!Changes.ContainsKey(accountId))
			{
				Changes[accountId] = new List<Change>();
			}
			var change = new Change
			{
				AccountId = accountId,
				Action = action,
				Fields = fields,
				Time = DateTime.Now.ToString(),
				ChangedBy = changeBy
			};
			Changes[accountId].Add(change);

			// Вызываем событие
			OnActionExecuted?.Invoke(change);
		}
	}

}
