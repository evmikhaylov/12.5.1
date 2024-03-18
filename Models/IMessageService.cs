using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace Models
{
	public interface IMessageService
	{
		void ShowMessage(string message);
		MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton buttons);
	}

	public class MessageBoxMessageService : IMessageService
	{
		public void ShowMessage(string message)
		{
			MessageBox.Show(message);
		}

		public MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton buttons)
		{
			return MessageBox.Show(message, caption, buttons);
		}
	}
}
