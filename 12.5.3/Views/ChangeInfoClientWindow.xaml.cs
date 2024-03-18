using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using _12._5._3.ViewModels;

namespace _12._5._3
{
	/// <summary>
	/// Логика взаимодействия для ChangeInfoClientWindow.xaml
	/// </summary>
	public partial class ChangeInfoClientWindow : Window
	{
		public ChangeInfoClientWindow(Account selectedAccount, Employee currentEmployee, ChangeLog changeLog, ISaveLoad saveLoadData, string filePathChange, List<Account> accounts, string filePath)
		{
			InitializeComponent();
			var changeInfoClientViewModel = new ChangeInfoClientViewModel(selectedAccount, currentEmployee, changeLog, saveLoadData, filePathChange, accounts, filePath);
			DataContext = changeInfoClientViewModel;
			changeInfoClientViewModel.CloseAction = new Action<bool>((dialogResult) =>
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
