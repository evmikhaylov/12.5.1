using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Models;
using _12._5._3.ViewModels;

namespace _12._5._3
{
	/// <summary>
	/// Логика взаимодействия для TransferWindow.xaml
	/// </summary>
	public partial class TransferWindow : Window
	{

		public TransferWindow(List<Account> accounts, TransferService<BankAccount> transferService, ChangeLog changeLog, ISaveLoad saveLoad, string filePathChange, Employee employee, IMessageService messageService)
		{
			InitializeComponent();
			var viewModel = new TransferViewModel(accounts, transferService, changeLog, saveLoad, filePathChange, employee, messageService);
			this.DataContext= viewModel;
			viewModel.CloseAction = new Action<bool>((dialogResult) =>
			{
				if (dialogResult)
				{
					this.DialogResult = true;
					this.Close();
				}
			});
		}
	}
	
}
