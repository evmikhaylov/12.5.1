using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Documents;
using System.Windows.Data;
using GalaSoft.MvvmLight.Views;
using Models;

namespace _12._5._3.ViewModels
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private readonly IMessageService _messageService;
		private readonly BankAccountService _bankAccountService;
		private readonly ISaveLoad _saveLoad;
		private readonly List<Account> _accounts = new List<Account>();
		private readonly string _filePath = "accounts.json";
		readonly IBankAccountFactory _bankAccountFactory = new BankAccountFactory();
		private readonly List<Account> _accountData;
		private readonly ChangeLog _changeLog;
		public string InputAmount { get; set; }
		private readonly string _filePathChange = "ChangeLog.json";
		public ICommand AddAccountCommand { get; private set; }
		public ICommand DeleteBankAccountCommand { get; private set; }
		public ICommand TransferCommand { get; private set; }
		public ICommand AddBankAccountCommand { get; private set; }
		public ICommand DepositCommand { get; private set; }
		public ICommand WithdrawCommand { get; private set; }
		public ICommand ChangeInfoClientCommand { get; private set; }
		public ICommand SelectionChangedCommand { get; }
		private ObservableCollection<Account> _accountCollection;
		public ObservableCollection<Account> AccountCollection
		{
			get { return _accountCollection; }
			set
			{
				_accountCollection = value;
				OnPropertyChanged("AccountCollection");
			}
		}
		public ObservableCollection<BankAccount> _bankAccountCollection;
		public ObservableCollection<BankAccount> BankAccountCollection
		{
			get { return _bankAccountCollection; }
			set
			{
				_bankAccountCollection = value;
				OnPropertyChanged("BankAccountCollection");
			}
		}
		private Account _selectedAccount;
		public Account SelectedAccount
		{
			get { return _selectedAccount; }
			set
			{
				_selectedAccount = value;
				BankAccountCollection = new ObservableCollection<BankAccount>(_selectedAccount.BankAccounts);
				OnPropertyChanged("SelectedAccount");
				OnPropertyChanged("BankAccountCollection"); ;
			}
		}
		private BankAccount _selectedBankAccount;
		public BankAccount SelectedBankAccount
		{
			get { return _selectedBankAccount; }
			set
			{
				_selectedBankAccount = value;
				OnPropertyChanged("SelectedBankAccount");
			}
		}
		private string _selectedEmployee;
		public string SelectedEmployee
		{
			get { return _selectedEmployee; }
			set
			{
				_selectedEmployee = value;
				OnPropertyChanged("SelectedEmployee");
				UpdateEmployeeRole(_selectedEmployee);
			}
		}
		private Employee _currentEmployee;
		public Employee CurrentEmployee
		{
			get { return _currentEmployee; }
			set
			{
				_currentEmployee = value;
				OnPropertyChanged(nameof(CurrentEmployee));
			}
		}
		public bool CanDeposit => _currentEmployee?.CanDeposit ?? false;
		public bool CanWithdraw => _currentEmployee?.CanWithdraw ?? false;
		public bool CanAddAccount => _currentEmployee?.CanAddAccount ?? false;
		public bool CanAddBankAccount => _currentEmployee?.CanAddBankAccount ?? false;
		public bool CanTransfer => _currentEmployee?.CanTransfer ?? false;
		public bool CanDeleteBankAccount => _currentEmployee?.CanDeleteBankAccount ?? false;
		public bool CanChangeInfoClient => _currentEmployee?.CanChangeInfoClient ?? false;


		public MainWindowViewModel(IMessageService messageService)
		{
			_messageService = messageService;
			_saveLoad = new SaveLoadData();
			_bankAccountService = new BankAccountService();
			_changeLog = new ChangeLog();
			_changeLog.OnActionExecuted += ChangeLog_OnActionExecuted;

			_accountData = _saveLoad.LoadData(_filePath);


			_accounts = _accountData.Select(account => {
				var newAccount = new Account(account.FamilyName, account.FirstName, account.Patronymic, account.NumberPhone, account.SerialNumberDoc, new ObservableCollection<BankAccount>());
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

			AddAccountCommand = new RelayCommand<object>(AddAccount);
			DeleteBankAccountCommand = new RelayCommand<object>(DeleteBankAccount);
			TransferCommand = new RelayCommand<object>(Transfer);
			AddBankAccountCommand = new RelayCommand<object>(AddBankAccount);
			DepositCommand = new RelayCommand<object>(Deposit);
			WithdrawCommand =  new RelayCommand<object>(Withdraw);
			ChangeInfoClientCommand =  new RelayCommand<object>(ChangeInfoClient);
			AccountCollection = new ObservableCollection<Account>(_accounts);
			

		}
		

		private void UpdateEmployeeRole(string selectedEmployee)
		{
			switch (selectedEmployee)
			{
				case "Консультант":
					_currentEmployee = new Consultant();
					break;
				case "Менеджер":
					_currentEmployee = new Manager();
					break;
			}
			AccountCollection = new ObservableCollection<Account>(_accounts);
			UpdateProperties();

		}
		private void UpdateProperties()
		{
			foreach (var prop in typeof(Employee).GetProperties())
			{
				if (prop.PropertyType == typeof(bool))
				{
					OnPropertyChanged(prop.Name);
				}
			}
		}

		private void AddAccount(object parameter)
		{
			var addAccountViewModel = new AddAccountViewModel(_bankAccountFactory);
			var addAccountWindow = new AddAccountWindow(_bankAccountFactory);
			addAccountWindow.DataContext = addAccountViewModel;
			addAccountViewModel.CloseAction = new Action<bool>(dialogResult => addAccountWindow.DialogResult = dialogResult);
			var result = addAccountWindow.ShowDialog();
			if(result == true && addAccountViewModel.NewAccount !=null)
			{
				_accounts.Add(addAccountViewModel.NewAccount);
				_changeLog.LogChange(Convert.ToString(addAccountViewModel.NewAccount.Id), "Добавлен", "Все поля", _currentEmployee.GetType().Name);
				_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
				_saveLoad.SaveData(_accounts, _filePath);
				AccountCollection = new ObservableCollection<Account>(_accounts);
			}
		}
		
		private void Transfer(object parameter)
		{
			TransferService<BankAccount> transferService = new TransferService<BankAccount>();
			var transferViewModel = new TransferViewModel(_accounts, transferService, _changeLog, _saveLoad, _filePathChange, _currentEmployee, _messageService);
			var transferWindow = new TransferWindow(_accounts, transferService, _changeLog, _saveLoad, _filePathChange, _currentEmployee, _messageService);
			transferWindow.DataContext = transferViewModel;
			transferViewModel.CloseAction = new Action<bool>(dialogResult => transferWindow.DialogResult = dialogResult);
			var result = transferWindow.ShowDialog();
			if (result == true)
			{
				_messageService.ShowMessage("Перевод успешно оформлен");
				_saveLoad.SaveData(_accounts, _filePath);
				AccountCollection = new ObservableCollection<Account>(_accounts);
			}
		}
		private void ChangeInfoClient(object parameter)
		{
			if (SelectedAccount != null)
			{
				var changeInfoClientWindow = new ChangeInfoClientWindow(SelectedAccount, _currentEmployee, _changeLog, _saveLoad, _filePathChange, _accounts, _filePath);
				var changeInfoClientViewModel = new ChangeInfoClientViewModel(SelectedAccount, _currentEmployee, _changeLog, _saveLoad, _filePathChange, _accounts, _filePath);
				changeInfoClientWindow.DataContext = changeInfoClientViewModel;
				changeInfoClientViewModel.CloseAction = new Action<bool>(dialogResult => changeInfoClientWindow.DialogResult = dialogResult);
				changeInfoClientWindow.ShowDialog();
				AccountCollection = new ObservableCollection<Account>(_accounts);
			}
			else
			{
				_messageService.ShowMessage("Выберите клиента для обновления информации");
			}
		}
		private void AddBankAccount(object parameter)
		{
			if (SelectedAccount != null)
			{


				if (SelectedAccount.BankAccounts.Count>=2)
				{
					_messageService.ShowMessage("У аккаунта уже есть и депоизитный и не депозитный счета");
					return;
				}

				var addBankAccountViewModel = new AddBankAccountViewModel(SelectedAccount, _bankAccountFactory);
				var addBankAccountWindow = new AddBankAccountWindow(addBankAccountViewModel);
				addBankAccountViewModel.CloseAction = new Action<bool>(dialogResult => addBankAccountWindow.DialogResult = dialogResult);
				var result = addBankAccountWindow.ShowDialog();
				if(result == true && addBankAccountViewModel.NewBankAccount != null)
				{
					SelectedAccount.BankAccounts.Add(addBankAccountViewModel.NewBankAccount);
					_changeLog.LogChange(Convert.ToString(SelectedAccount.Id), "Добавлен счёт", $"{addBankAccountViewModel.NewBankAccount.BankAccountType}," +
						$" баланс {addBankAccountViewModel.NewBankAccount.Balance}", _currentEmployee.GetType().Name);
					_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
					_saveLoad.SaveData(_accounts, _filePath);
					AccountCollection = new ObservableCollection<Account>(_accounts);

				}
			}
			else
			{
				_messageService.ShowMessage("Пожалуйста, выберите аккаунт для добавления счёта");
			}
			
		}
		private void DeleteBankAccount(object parameter)
		{
			if (SelectedAccount != null)
			{
				var selectedBankAccount = SelectedAccount.SelectedBankAccount;
				if (selectedBankAccount != null)
				{
					MessageBoxResult MBresult = _messageService.ShowMessage("Вы уверены, что хотите удалить этот счёт?", "Подтверждение удаления", MessageBoxButton.YesNo);
					if (MBresult == MessageBoxResult.Yes)
					{
						_changeLog.LogChange(Convert.ToString(SelectedAccount.Id), "Удален счет", $"{selectedBankAccount.BankAccountType} баланс {selectedBankAccount.Balance}", _currentEmployee.GetType().Name);
						SelectedAccount.BankAccounts.Remove(selectedBankAccount);
						_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
						_saveLoad.SaveData(_accounts, _filePath);
						AccountCollection = new ObservableCollection<Account>(_accounts);
					}
				}
				else
				{
					_messageService.ShowMessage("Выберите счёт для удаления");
				}
			}
			else
			{
				_messageService.ShowMessage("Выберите аккаунт для удаления");
			}
		}
		private void Deposit(object parameter)
		{
			if (SelectedAccount != null)
			{
				var selectedBankAccount = SelectedAccount.SelectedBankAccount;
				if (selectedBankAccount != null)
				{
					var dialogViewModel = new InputBoxDialogViewModel(_messageService);
					var dialog = new InputBoxDialog { DataContext = dialogViewModel };
					dialogViewModel.CloseAction=(bool result) => { dialog.DialogResult = result; };
					

					if (dialog.ShowDialog()==true)
					{
						string amountText = dialogViewModel.InputText.Replace(",", ".");
						double.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out double amount);
							_changeLog.LogChange(Convert.ToString(SelectedAccount.Id), "Пополнение", Convert.ToString(amount), _currentEmployee.GetType().Name);
							_bankAccountService.Deposit(selectedBankAccount, amount);
							_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
							_saveLoad.SaveData(_accounts, _filePath);
							AccountCollection = new ObservableCollection<Account>(_accounts);
							_messageService.ShowMessage("Пополнение успешно выполнено");
						
					}
				}
				else
				{
					_messageService.ShowMessage("Выберите счёт для пополнения");
				}
			}
			else
			{
				_messageService.ShowMessage("Выберите аккаунт для пополнения");
			}
		}
		private void Withdraw(object parameter)
		{
			if (SelectedAccount != null)
			{
				var selectedBankAccount = SelectedAccount.SelectedBankAccount;
				if (selectedBankAccount != null)
				{
					var dialogViewModel = new InputBoxDialogViewModel(_messageService);
					var dialog = new InputBoxDialog { DataContext = dialogViewModel };
					dialogViewModel.CloseAction=(bool result) => { dialog.DialogResult = result; };


					if (dialog.ShowDialog()==true)
					{
						string amountText = dialogViewModel.InputText.Replace(",", ".");
						double.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out double amount);
						
							if (selectedBankAccount.Balance >= amount)
							{
								_changeLog.LogChange(Convert.ToString(SelectedAccount.Id), "Снятие", Convert.ToString(amount), _currentEmployee.GetType().Name);
								_bankAccountService.Withdraw(selectedBankAccount, amount);
								_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
								_saveLoad.SaveData(_accounts, _filePath);
								AccountCollection = new ObservableCollection<Account>(_accounts);
								_messageService.ShowMessage("Снятие успешно выполнено");
							}
							else
							{
								_messageService.ShowMessage("На счету недостаточно средств для снятия.");
							}
					}
				}
				else
				{
					_messageService.ShowMessage("Выберите счёт для снятия");
				}
			}
			else
			{
				_messageService.ShowMessage("Выберите аккаунт для снятия");
			}
		}
		

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void ChangeLog_OnActionExecuted(Change change)
		{
			_messageService.ShowMessage($"Действие: {change.Action}\nВремя: {change.Time}\nСотрудник: {change.ChangedBy}");
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
