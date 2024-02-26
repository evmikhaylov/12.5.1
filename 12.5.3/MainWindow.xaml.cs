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
		private List<Account> _accounts = new List<Account>();
		private readonly string _filePath = "accounts.json";
		readonly IBankAccountFactory _bankAccountFactory = new BankAccountFactory();
		private Employee _employee;
		private List<Account> _accountData;
		private ChangeLog _changeLog;
		private readonly string _filePathChange = "ChangeLog.json";
		public MainWindow()
		{
			InitializeComponent();
			
			_saveLoad = new SaveLoadData();
			_bankAccountService = new BankAccountService();
			_changeLog = new ChangeLog();
			_changeLog.OnActionExecuted += ChangeLog_OnActionExecuted;
			AddBankAccount.IsEnabled = false;
			DeleteBankAccount.IsEnabled = false;
			Transfer.IsEnabled = false;
			DepositButton.IsEnabled = false;
			WithdrawButton.IsEnabled = false;
			AddAccount.IsEnabled = false;
			ChangeInfoClient.IsEnabled = false;
			
			_accountData = _saveLoad.LoadData(_filePath);


			_accounts = _accountData.Select(account => {
				var newAccount = new Account(account.FamilyName, account.FirstName, account.Patronymic, account.NumberPhone, account.SerialNumberDoc, new List<BankAccount>());
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

			
		}
		private void ChangeLog_OnActionExecuted(Change change)
		{
			MessageBox.Show($"Действие: {change.Action}\nВремя: {change.Time}\nСотрудник: {change.ChangedBy}");
		}
		private void SelectedEmployeeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var comboBox = (ComboBox)sender;
			var comboBoxItem = comboBox.SelectedItem as ComboBoxItem;
			var selectedEmployee = comboBoxItem.Content.ToString();

			switch(selectedEmployee)
			{
				case "Консультант":
					_employee = new Consultant();
					AddBankAccount.IsEnabled = false;
					DeleteBankAccount.IsEnabled = false;
					Transfer.IsEnabled = true;
					DepositButton.IsEnabled = true;
					WithdrawButton.IsEnabled = false;
					AddAccount.IsEnabled = false;
					ChangeInfoClient.IsEnabled = true;
					break;

				case "Менеджер":
					_employee = new Manager();
					AddBankAccount.IsEnabled = true;
					DeleteBankAccount.IsEnabled = true;
					Transfer.IsEnabled = true;
					DepositButton.IsEnabled = true;
					AddAccount.IsEnabled = true;
					WithdrawButton.IsEnabled = true;
					ChangeInfoClient.IsEnabled = true;
					break;
					default: MessageBox.Show("Сотрудник не выбран"); break;
			}
			var column = AccountsDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Серия, номер документа");

			if (column != null)
			{
				if (_employee.ViewSerialNumberDoc)
				{
					column.Visibility = Visibility.Visible;
				}
				else
				{
					column.Visibility = Visibility.Hidden;
				}
			}
			AccountsDataGrid.ItemsSource = _accounts;
			AccountsDataGrid.Items.Refresh();
		}


		private void AddAccount_Click(object sender, RoutedEventArgs e)
		{
			
			AddAccountWindow addAccountWindow = new AddAccountWindow(_bankAccountFactory);
			addAccountWindow.ShowDialog();
			if (addAccountWindow.NewAccount != null)
			{ 
				_accounts.Add(addAccountWindow.NewAccount);
				_changeLog.LogChange(Convert.ToString(addAccountWindow.NewAccount.Id),"Добавлен", "Все поля", _employee.GetType().Name);
			}
			_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
			_saveLoad.SaveData(_accounts, _filePath);
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
						_changeLog.LogChange(Convert.ToString(selectedAccount.Id), "Удален счет", $"{selectedBankAccount.BankAccountType} баланс {selectedBankAccount.Balance}", _employee.GetType().Name);
						selectedAccount.BankAccounts.Remove(selectedBankAccount);
						_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
						_saveLoad.SaveData(_accounts, _filePath);
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
			TransferWindow transferWindow = new TransferWindow(_accounts, transferService, _changeLog, _saveLoad, _filePathChange, _employee);
			transferWindow.ShowDialog();
			_saveLoad.SaveData(_accounts, _filePath);
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
					_changeLog.LogChange(Convert.ToString(selectedAccount.Id), "Добавлен счет",
						$"{addBankAccountWindow.NewAccount.BankAccountType}, баланс:{addBankAccountWindow.NewAccount.Balance}", _employee.GetType().Name);
				}
				AccountsDataGrid.Items.Refresh();
				_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
				_saveLoad.SaveData(_accounts, _filePath);
				
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
						_changeLog.LogChange(Convert.ToString(selectedAccount.Id), "Пополнение", Convert.ToString(amount), _employee.GetType().Name);
						_bankAccountService.Deposit(selectedBankAccount, amount);
						_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
						_saveLoad.SaveData(_accounts, _filePath);
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
							_changeLog.LogChange(Convert.ToString(selectedAccount.Id), "Снятие", Convert.ToString(amount), _employee.GetType().Name);
							_bankAccountService.Withdraw(selectedBankAccount, amount);
							_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
							_saveLoad.SaveData(_accounts, _filePath);
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

		private void ChangeInfoClient_Click(object sender, RoutedEventArgs e)
		{
			if(AccountsDataGrid.SelectedItem is Account selectedAccount)
			{
				ChangeInfoClientWindow changeInfoClientWindow = new ChangeInfoClientWindow(selectedAccount, _employee, _changeLog, _saveLoad, _filePathChange);
				changeInfoClientWindow.ShowDialog();
				_saveLoad.SaveData(_accounts, _filePath);
				AccountsDataGrid.Items.Refresh();
			}
			else
			{
				MessageBox.Show("Выберите клиента для обновления информации");
			}
			
		}
	}
	
}
