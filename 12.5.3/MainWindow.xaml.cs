using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _12._5._3
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly BankAccountService _bankAccountService;
		private readonly ISaveLoad _saveLoad;
		private readonly List<Account> accounts = new List<Account>();
		private readonly string filePath = "accounts.json";
		readonly IBankAccountFactory bankAccountFactory = new BankAccountFactory();
		public MainWindow()
		{
			InitializeComponent();
			_saveLoad = new SaveLoadData();
			_bankAccountService = new BankAccountService();
			var accountData = _saveLoad.LoadData(filePath);

			accounts = accountData.Select(account => {
				var newAccount = new Account(account.FullName, new List<BankAccount>());
				foreach (var bankAccount in account.BankAccounts)
				{
					if (bankAccount.BankAccountType == "Не депозитный")
					{
						newAccount.BankAccounts.Add(new NonDepositBankAccount(bankAccount.Balance, bankAccount.BankAccountType));
					}
					else
					{
						newAccount.BankAccounts.Add(new DepositBankAccount(bankAccount.Balance, bankAccount.BankAccountType));
					}
				}
				return newAccount;
			}).ToList();
			AccountsDataGrid.ItemsSource = accounts;
			AccountsDataGrid.Items.Refresh();
		}


		private void AddAccount_Click(object sender, RoutedEventArgs e)
		{
			
			AddAccountWindow addAccountWindow = new AddAccountWindow(bankAccountFactory);
			addAccountWindow.ShowDialog();
			if (addAccountWindow.NewAccount != null)
			{ accounts.Add(addAccountWindow.NewAccount); }
			_saveLoad.SaveData(accounts, filePath);
			AccountsDataGrid.Items.Refresh();

		}
		private void BankAccountsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var dataGrid = (DataGrid)sender;
			Account account = (Account)dataGrid.DataContext;
			account.SelectedBankAccount = (BankAccount)dataGrid.SelectedItem;
		}

		private void DeleteBankAccount_Click(object sender, RoutedEventArgs e)
		{
			if (AccountsDataGrid.SelectedItem is Account selectedAccount)
			{
				var selectedBankAccount = selectedAccount.SelectedBankAccount;
				if (selectedBankAccount != null)
				{
					MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот счёт?", "Подтверждение удаления", MessageBoxButton.YesNo);
					if (result == MessageBoxResult.Yes)
					{
						selectedAccount.BankAccounts.Remove(selectedBankAccount);
						_saveLoad.SaveData(accounts, filePath);
						AccountsDataGrid.Items.Refresh();
					}
				}
				else
				{
					MessageBox.Show("Выберите счёт для удаления");
				}
			}
			else
			{
				MessageBox.Show("Выберите аккаунт для удаления");
			}
		}

		private void Transfer_Click(object sender, RoutedEventArgs e)
		{
			TransferService<BankAccount> transferService = new TransferService<BankAccount>();
			TransferWindow transferWindow = new TransferWindow(accounts, transferService);
			transferWindow.ShowDialog();
			_saveLoad.SaveData(accounts, filePath);
			AccountsDataGrid.Items.Refresh();
		}


		private void AddBankAccount_Click(object sender, RoutedEventArgs e)
		{

			if (AccountsDataGrid.SelectedItem is Account selectedAccount)
			{


				if (selectedAccount.BankAccounts.Count>=2)
				{
					MessageBox.Show("У аккаунта уже есть и депоизитный и не депозитный счета");
					return;
				}

				IBankAccountFactory bankAccountFactory = new BankAccountFactory();
				AddBankAccountWindow addBankAccountWindow = new AddBankAccountWindow(selectedAccount, bankAccountFactory);
				addBankAccountWindow.ShowDialog();

				if (addBankAccountWindow.NewAccount != null)
				{
					selectedAccount.BankAccounts.Add(addBankAccountWindow.NewAccount);
				}
				AccountsDataGrid.Items.Refresh();
				_saveLoad.SaveData(accounts, filePath);
				
			}
			else
			{
				MessageBox.Show("Пожалуйста, выберите аккаунт для добавления счёта");
			}
			AccountsDataGrid.Items.Refresh();
		}
		private void DepositButton_Click(object sender, RoutedEventArgs e)
		{
			if (AccountsDataGrid.SelectedItem is Account selectedAccount)
			{
				var selectedBankAccount = selectedAccount.SelectedBankAccount;
				if (selectedBankAccount != null)
				{
					string input = InputBox.Show("Пополнение счета", "Введите сумму для пополнения:");
					string amountText = input.Replace(",", ".");
					if (double.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out double amount))
					{
						
						_bankAccountService.Deposit(selectedBankAccount, amount);
						_saveLoad.SaveData(accounts, filePath);
						AccountsDataGrid.Items.Refresh();
						MessageBox.Show("Пополнение успешно выполнено");
					}
					else
					{
						MessageBox.Show("Введенная сумма недействительна. Пожалуйста, введите действительное число.");
					}
				}
				else
				{
					MessageBox.Show("Выберите счёт для пополнения");
				}
			}
			else
			{
				MessageBox.Show("Выберите аккаунт для пополнения");
			}
		}
		
		private void WithdrawButton_Click(object sender, RoutedEventArgs e)
		{
			if (AccountsDataGrid.SelectedItem is Account selectedAccount)
			{
				var selectedBankAccount = selectedAccount.SelectedBankAccount;
				if (selectedBankAccount != null)
				{
					
					string input = InputBox.Show("Снятие со счета", "Введите сумму для снятия:");
					string amountText = input.Replace(",", ".");
					if (double.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out double amount))
					{
						if (selectedBankAccount.Balance >= amount)
						{
							_bankAccountService.Withdraw(selectedBankAccount, amount);
							_saveLoad.SaveData(accounts, filePath);
							AccountsDataGrid.Items.Refresh();
							MessageBox.Show("Снятие успешно выполнено");
						}
						else
						{
							MessageBox.Show("На счету недостаточно средств для снятия.");
						}
					}
					else
					{
						MessageBox.Show("Введенная сумма недействительна. Пожалуйста, введите действительное число.");
					}
				}
				else
				{
					MessageBox.Show("Выберите счёт для снятия");
				}
			}
			else
			{
				MessageBox.Show("Выберите аккаунт для снятия");
			}

		}
		public static class InputBox
		{
			public static string Show(string title, string promptText)
			{
				InputBoxDialog ib = new InputBoxDialog
				{
					Title = title,
					textBlock = { Text = promptText }
				};
				if (ib.ShowDialog() == true)
					return ib.inputTextBox.Text;
				else
					return null;
			}
		}
	}
	
}
