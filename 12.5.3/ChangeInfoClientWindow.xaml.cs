using System;
using System.Collections.Generic;
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

namespace _12._5._3
{
	/// <summary>
	/// Логика взаимодействия для ChangeInfoClientWindow.xaml
	/// </summary>
	public partial class ChangeInfoClientWindow : Window
	{
		private Account _account;
		private Employee _employee;
		private ChangeLog _changeLog;
		private ISaveLoad _saveLoad;
		private string _filePathChange;
		public ChangeInfoClientWindow(Account account, Employee employee, ChangeLog changeLog, ISaveLoad saveLoad, string filePathChange)
		{
			InitializeComponent();
			this._account=account;
			this._employee=employee;
			this._changeLog=changeLog;
			this._saveLoad=saveLoad;
			this._filePathChange = filePathChange;
			NewFamilyName.Text = _account.FamilyName;
			NewFirstName.Text = _account.FirstName;
			NewPatronymic.Text = _account.Patronymic;
			NewNumberPhone.Text = _account.NumberPhone;
			NewSerialDoc.Text = _account.SerialNumberDoc.Split(' ')[0];
			NewNumberDoc.Text = _account.SerialNumberDoc.Split(' ')[1];
			NewFamilyName.IsEnabled = false;
			NewFirstName.IsEnabled = false;
			NewPatronymic.IsEnabled = false;
			NewSerialDoc.IsEnabled = false;
			NewNumberDoc.IsEnabled = false;
			NewNumberPhone.IsEnabled = false;
			if (_employee is Consultant)
			{
				NewNumberPhone.IsEnabled = true;
				NewSerialDoc.Visibility = Visibility.Hidden;
				NewNumberDoc.Visibility = Visibility.Hidden;
			}
			else
			{
				NewFamilyName.IsEnabled = true;
				NewFirstName.IsEnabled = true;
				NewPatronymic.IsEnabled = true;
				NewSerialDoc.IsEnabled = true;
				NewNumberDoc.IsEnabled = true;
				NewNumberPhone.IsEnabled = true;
				NewSerialDoc.Visibility = Visibility.Visible;
				NewNumberDoc.Visibility = Visibility.Visible;
			}
			_saveLoad=saveLoad;
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			var fields = new Dictionary<string, (string OldValue, string NewValue)>
			{
				{"FamilyName", (_account.FamilyName, NewFamilyName.Text) },
				{ "FirstName", (_account.FirstName, NewFirstName.Text) },
				{ "Patronymic", (_account.Patronymic, NewPatronymic.Text) },
				{ "NumberPhone", (_account.NumberPhone, NewNumberPhone.Text) },
				{ "SerialNumberDoc", (_account.SerialNumberDoc, NewSerialDoc.Text + " " + NewNumberDoc.Text) }
			};
			StringBuilder sb = new StringBuilder();
			foreach(var field in fields)
			{
				if(field.Value.OldValue != field.Value.NewValue)
				{
					sb.Append($"{field.Key}: {field.Value.OldValue}, ");
				}
			}
			if (sb.Length > 0)
			{
				sb.Remove(sb.Length - 2, 2);
				_changeLog.LogChange(Convert.ToString(_account.Id), "Изменено", sb.ToString(), _employee.GetType().Name);
			}
			_account.FamilyName = NewFamilyName.Text;
			_account.FirstName = NewFirstName.Text;
			_account.Patronymic = NewPatronymic.Text;
			_account.NumberPhone = NewNumberPhone.Text;
			_account.SerialNumberDoc = NewSerialDoc.Text + " " + NewNumberDoc.Text;
			_saveLoad.SaveLog(_changeLog.Changes, _filePathChange);
			
			this.Close();
		}
	}
}
