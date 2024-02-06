using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using System.Windows.Shapes;

namespace _12._5._2
{
	/// <summary>
	/// Логика взаимодействия для AddBankAccountWindow.xaml
	/// </summary>
	public partial class AddBankAccountWindow : Window
	{
		private readonly IBankAccountFactory _bankAccountFactory;
		private readonly Account _account;
		public BankAccount NewAccount { get; private set; }
		public AddBankAccountWindow(Account account, IBankAccountFactory bankAccountFactory)
		{
			InitializeComponent();
			this._account = account;
			_bankAccountFactory=bankAccountFactory;

			if (account.BankAccounts.Any(ba => ba.BankAccountType == "Депозитный"))
			{
				accountTypeComboBox.Items.Add("Не депозитный");
			}
			
			else if (account.BankAccounts.Any(ba => ba.BankAccountType == "Не депозитный"))
			{
				accountTypeComboBox.Items.Add("Депозитный");
			}
			else
			{
				accountTypeComboBox.Items.Add("Депозитный");
				accountTypeComboBox.Items.Add("Не депозитный");
			}

			accountTypeComboBox.SelectedIndex = 0;
		}

		private void OkButtonNewBankAccount_Click(object sender, RoutedEventArgs e)
		{
			string accountType = accountTypeComboBox.SelectedItem.ToString();
			string amountText = amountTextBox.Text.Replace(",", ".");
			double amount = double.Parse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture);

			NewAccount = _bankAccountFactory.Create(amount, accountType);
			this.Close();

		}
	}
}
