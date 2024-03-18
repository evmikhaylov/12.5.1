using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace _12._5._3.ViewModels
{
	public class ChangeInfoClientViewModel : INotifyPropertyChanged
	{
		private readonly Account _account;
		private readonly Employee _employee;
		private readonly ChangeLog _changeLog;
		private readonly ISaveLoad _saveLoad;
		private readonly string _filePathChange;


		private string _newFamilyName;
		public string NewFamilyName { get { return _newFamilyName; }
			set
			{
				_newFamilyName = value;
				OnPropertyChanged(nameof(NewFamilyName));
				(OkCommand as RelayCommand)?.RaiseCanExecuteChanged();
			} }
		private string _newFirstName;
		public string NewFirstName { get { return _newFirstName; }
			set
			{
				_newFirstName = value;
				OnPropertyChanged(nameof(NewFirstName));
				(OkCommand as RelayCommand)?.RaiseCanExecuteChanged();
			}
		}
		private string _newPatronymic;
		public string NewPatronymic { get { return _newPatronymic; } set
			{
				_newPatronymic = value;
				OnPropertyChanged(nameof(NewPatronymic));
				(OkCommand as RelayCommand)?.RaiseCanExecuteChanged();
			}
		}
		private string _newNumberPhone;
		public string NewNumberPhone { get { return _newNumberPhone; } set
			{
				_newNumberPhone = value;
				OnPropertyChanged(nameof(NewNumberPhone));
				(OkCommand as RelayCommand)?.RaiseCanExecuteChanged();
			}
		}
		private string _newSerialDoc;
		public string NewSerialDoc { get { return _newSerialDoc; } set

			{
				_newSerialDoc = value;
				OnPropertyChanged(nameof(NewSerialDoc));
				(OkCommand as RelayCommand)?.RaiseCanExecuteChanged();
			}
		}
		private string _newNumberDoc;
		public string NewNumberDoc { get { return _newNumberDoc; } set
			{
				_newNumberDoc = value;
				OnPropertyChanged(nameof(NewNumberDoc));
				(OkCommand as RelayCommand)?.RaiseCanExecuteChanged();
			}
		}

		public ICommand OkCommand { get; private set; }
		public Action<bool> CloseAction { get; set; }
		private List<Account> _accounts;
		private string _filePath;
		public bool IsManager
		{
			get { return _employee is Manager; }
		}

		public ChangeInfoClientViewModel(Account account, Employee employee, ChangeLog changeLog, ISaveLoad saveLoad, string filePathChange, List<Account> accounts, string filePath)
		{
			_account = account;
			_employee = employee;
			_changeLog = changeLog;
			_saveLoad = saveLoad;
			_filePathChange = filePathChange;
			_accounts = accounts;
			_filePath = filePath;
			NewFamilyName = _account.FamilyName;
			NewFirstName = _account.FirstName;
			NewPatronymic = _account.Patronymic;
			NewSerialDoc = _account.SerialNumberDoc.Split(' ')[0];
			NewNumberDoc = _account.SerialNumberDoc.Split(' ')[1];

			NewNumberPhone = _account.NumberPhone;

			OkCommand = new _12._5._3.Models.RelayCommand(obj => OkExecute(), obj => CanOkExecute());
		}

		private bool CanOkExecute()
		{
			return _account.FamilyName != NewFamilyName ||
		   _account.FirstName != NewFirstName ||
		   _account.Patronymic != NewPatronymic ||
		   _account.NumberPhone != NewNumberPhone ||
		   _account.SerialNumberDoc != NewSerialDoc + " " + NewNumberDoc;
		}

		private void OkExecute()
		{
			var fields = new Dictionary<string, (string OldValue, string NewValue)>
		{
			{"FamilyName", (_account.FamilyName, NewFamilyName) },
			{ "FirstName", (_account.FirstName, NewFirstName) },
			{ "Patronymic", (_account.Patronymic, NewPatronymic) },
			{ "NumberPhone", (_account.NumberPhone, NewNumberPhone) },
			{ "SerialNumberDoc", (_account.SerialNumberDoc, NewSerialDoc + " " + NewNumberDoc) }
		};
			StringBuilder sb = new StringBuilder();
			foreach (var field in fields)
			{
				if (field.Value.OldValue != field.Value.NewValue)
				{
					sb.Append($"{field.Key}: {field.Value.OldValue}, ");
				}
			}
			if (sb.Length > 0)
			{
				sb.Remove(sb.Length - 2, 2);
				_changeLog.LogChange(Convert.ToString(_account.Id), "Изменено", sb.ToString(), _employee.GetType().Name);
			}
			_account.FamilyName = NewFamilyName;
			_account.FirstName = NewFirstName;
			_account.Patronymic = NewPatronymic;
			_account.NumberPhone = NewNumberPhone;
			_account.SerialNumberDoc = NewSerialDoc + " " + NewNumberDoc;
			_saveLoad.SaveData(_accounts, _filePath);
			_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);

			CloseAction(true);
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
