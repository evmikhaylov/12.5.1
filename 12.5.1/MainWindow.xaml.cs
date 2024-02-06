using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _12._5._1
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Bank bank = new Bank();
		public MainWindow()
		{
			InitializeComponent();
			bank.LoadAccounts();
			AccountsDataGrid.ItemsSource = bank.GetAccounts();
		}

		private void AddAccount_Click(object sender, RoutedEventArgs e)
		{

			var addAccountWindow = new AddAccountWindow();
			if (addAccountWindow.ShowDialog() == true)
			{
				bank.OpenAccount(addAccountWindow.addBalance, addAccountWindow.addFullName);
				AccountsDataGrid.Items.Refresh();
			}

		}

		private void DeleteAccount_Click(object sender, RoutedEventArgs e)
		{
			if (AccountsDataGrid.SelectedItem is Account selectedAccount)
			{
				MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите закрыть этот счет?", "Подтверждение удаления", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					bank.CloseAccount(selectedAccount.Id);
					AccountsDataGrid.Items.Refresh();

				}
			}
			else
			{
				MessageBox.Show("Пожалуйста, выберите счет для удаления.");
			}
		}

		private void Transfer_Click(object sender, RoutedEventArgs e)
		{
			var transferWindow = new TransferWindow(bank.GetAccounts().Select(a => a.Id).ToList(), bank);
			if (transferWindow.ShowDialog() == true)
			{
				bank.Transfer(transferWindow.FromAccountId, transferWindow.ToAccountId, transferWindow.Amount);
				AccountsDataGrid.Items.Refresh();
			}
		}
	}

}
