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

namespace _12._5._2
{
	/// <summary>
	/// Логика взаимодействия для TransferWindow.xaml
	/// </summary>
	public partial class TransferWindow : Window
	{
		private readonly List<Account> accounts;
		private readonly TransferService _transferService;
		public TransferWindow(List<Account> accounts, TransferService transferService)
		{
			InitializeComponent();
			this.accounts = accounts;
			_transferService = transferService;
			fromAccountComboBox.ItemsSource = accounts.Select(a => a.FullName);
			toAccountComboBox.ItemsSource = accounts.Select(a => a.FullName);

			fromAccountComboBox.SelectionChanged += FromAccountComboBox_SelectionChanged;
			toAccountComboBox.SelectionChanged += ToAccountComboBox_SelectionChanged;
			_transferService=transferService;
		}

		private void FromAccountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var comboBox = (ComboBox)sender;
			var selectedAccountName = (string)comboBox.SelectedItem;
			var selectedAccount = accounts.FirstOrDefault(a => a.FullName == selectedAccountName);
			fromBankAccounts.ItemsSource = selectedAccount?.BankAccounts.Select(ba => ba.NumberBankAccount);
		}

		private void ToAccountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var comboBox = (ComboBox)sender;
			var selectedAccountName = (string)comboBox.SelectedItem;
			var selectedAccount = accounts.FirstOrDefault(a => a.FullName == selectedAccountName);
			toBankAccounts.ItemsSource = selectedAccount?.BankAccounts.Select(ba => ba.NumberBankAccount);
		}


		private void TransferButton_Click(object sender, RoutedEventArgs e)
		{

			var fromAccountName = (string)fromAccountComboBox.SelectedItem;
			var toAccountName = (string)toAccountComboBox.SelectedItem;

			
			var fromAccount = accounts.FirstOrDefault(a => a.FullName == fromAccountName);
			var toAccount = accounts.FirstOrDefault(a => a.FullName == toAccountName);

			if (fromAccount == null)
			{
				MessageBox.Show("Пожалуйста, выберите аккаунт отправителя.");
				return;
			}

			if (toAccount == null)
			{
				MessageBox.Show("Пожалуйста, выберите аккаунт получателя.");
				return;
			}

			
			var fromBankAccountNumber = fromBankAccounts.SelectedItem as uint?;
			var toBankAccountNumber = toBankAccounts.SelectedItem as uint?;

			if (fromBankAccountNumber == null)
			{
				MessageBox.Show("Пожалуйста, выберите банковский счет отправителя.");
				return;
			}

			if (toBankAccountNumber == null)
			{
				MessageBox.Show("Пожалуйста, выберите банковский счет получателя.");
				return;
			}

			
			var fromBankAccount = fromAccount?.BankAccounts.FirstOrDefault(ba => ba.NumberBankAccount == fromBankAccountNumber);
			var toBankAccount = toAccount?.BankAccounts.FirstOrDefault(ba => ba.NumberBankAccount == toBankAccountNumber);


			
			if (!double.TryParse(TransferAmountTextBox.Text, out double amount))
			{
				MessageBox.Show("Пожалуйста, введите действительную сумму для перевода.");
				return;
			}

			
			if (fromBankAccount.Balance < amount)
			{
				MessageBox.Show("На счету отправителя недостаточно средств для перевода.");
				return;
			}

			
			_transferService.Transfer(fromBankAccount, toBankAccount, amount);
			MessageBox.Show("Перевод успешно оформлен");

			this.Close();
		}

	}
}
