using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Models;
using GalaSoft.MvvmLight.Views;

namespace _12._5._3.ViewModels
{
	public static class ViewModelLocator
	{
		private static IMessageService _messageService = new MessageBoxMessageService();

		public static MainWindowViewModel MainWindowViewModel => new MainWindowViewModel(_messageService);
	}
}
