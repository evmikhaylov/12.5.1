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

namespace _12._5._3
{
	/// <summary>
	/// Логика взаимодействия для TransferWindow.xaml
	/// </summary>
	public partial class TransferWindow : Window
	{
		private readonly ChangeLog _changeLog;
		private readonly ISaveLoad _saveLoad;
		private readonly string _filePathChange;
		private readonly Employee _employee;
		private readonly List<Account> _accounts;
		private readonly TransferService<BankAccount> _transferService;

		public TransferWindow(List<Account> accounts, TransferService<BankAccount> transferService, ChangeLog changeLog, ISaveLoad saveLoad, string filePathChange, Employee employee)
		{
			InitializeComponent();
			this._accounts = accounts;
			this._transferService = transferService;
			this._changeLog = changeLog;
			this._saveLoad = saveLoad;
			this._employee = employee;
			this._filePathChange = filePathChange;
			fromAccountComboBox.ItemsSource = accounts.Select(a => $"{a.FamilyName} {a.FirstName} {a.Patronymic} {a.NumberPhone}");
			toAccountComboBox.ItemsSource = accounts.Select(a => $"{a.FamilyName} {a.FirstName} {a.Patronymic} {a.NumberPhone}");

			fromAccountComboBox.SelectionChanged += FromAccountComboBox_SelectionChanged;
			toAccountComboBox.SelectionChanged += ToAccountComboBox_SelectionChanged;
			_transferService=transferService;
		}

		private void FromAccountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var comboBox = (ComboBox)sender;
			var selectedAccountName = comboBox.SelectedItem.ToString();
			var selectedAccount = _accounts.FirstOrDefault(a => $"{a.FamilyName} {a.FirstName} {a.Patronymic} {a.NumberPhone}" == selectedAccountName);
			fromBankAccounts.ItemsSource = selectedAccount?.BankAccounts.Select(ba => ba.NumberBankAccount);
		}

		private void ToAccountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var comboBox = (ComboBox)sender;
			var selectedAccountName = comboBox.SelectedItem.ToString();
			var selectedAccount = _accounts.FirstOrDefault(a => $"{a.FamilyName} {a.FirstName} {a.Patronymic} {a.NumberPhone}" == selectedAccountName);
			toBankAccounts.ItemsSource = selectedAccount?.BankAccounts.Select(ba => ba.NumberBankAccount);
		}


		private void TransferButton_Click(object sender, RoutedEventArgs e)
		{
			var fromAccount = GetSelectedAccount(fromAccountComboBox, _accounts);
			var toAccount = GetSelectedAccount(toAccountComboBox, _accounts);

			if (fromAccount == null || toAccount == null)
			{
				MessageBox.Show("Пожалуйста, выберите аккаунт отправителя и получателя.");
				return;
			}

			var fromBankAccount = GetSelectedBankAccount(fromBankAccounts, fromAccount);
			var toBankAccount = GetSelectedBankAccount(toBankAccounts, toAccount);

			if (fromBankAccount == null || toBankAccount == null)
			{
				MessageBox.Show("Пожалуйста, выберите банковский счет отправителя и получателя.");
				return;
			}

			double amount;
			if (!double.TryParse(TransferAmountTextBox.Text, out amount))
			{
				MessageBox.Show("Пожалуйста, введите действительную сумму для перевода.");
				return;
			}

			if (fromBankAccount.Balance < amount)
			{
				MessageBox.Show("На счету отправителя недостаточно средств для перевода.");
				return;
			}

			PerformTransfer(fromAccount, toAccount, fromBankAccount, toBankAccount, amount);
			MessageBox.Show("Перевод успешно оформлен");
			this.Close();
		}

		private Account GetSelectedAccount(ComboBox comboBox, List<Account> accounts)
		{
			var selectedAccountName = comboBox.SelectedItem?.ToString();
			return accounts.FirstOrDefault(a => $"{a.FamilyName} {a.FirstName} {a.Patronymic} {a.NumberPhone}" == selectedAccountName);
		}

		private BankAccount GetSelectedBankAccount(ComboBox comboBox, Account account)
		{
			var selectedBankAccountNumber = comboBox.SelectedItem as uint?;
			return account?.BankAccounts.FirstOrDefault(ba => ba.NumberBankAccount == selectedBankAccountNumber);
		}

		private void PerformTransfer(Account fromAccount, Account toAccount, BankAccount fromBankAccount, BankAccount toBankAccount, double amount)
		{
			_changeLog.LogChange(Convert.ToString(fromAccount.Id), "Перевод", $"получатель {toAccount.Id} сумма :{amount}", _employee.GetType().Name);
			_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
			_changeLog.LogChange(Convert.ToString(toAccount.Id), "Перевод", $"отправитель {fromAccount.Id} сумма :{amount}", _employee.GetType().Name);
			_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
			_transferService.Transfer(fromBankAccount, toBankAccount, amount);
		}

	}
	
}
