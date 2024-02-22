﻿using System;
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
using System.Windows.Shapes;

namespace _12._5._3
{
	/// <summary>
	/// Логика взаимодействия для AddAccountWindow.xaml
	/// </summary>
	public partial class AddAccountWindow : Window
	{
		private readonly IBankAccountFactory _bankAccountFactory;
		public Account NewAccount { get; private set; }
		public AddAccountWindow(IBankAccountFactory bankAccountFactory)
		{
			InitializeComponent();
			_bankAccountFactory = bankAccountFactory;
			accountTypeComboBox.Items.Add("Депозитный");
			accountTypeComboBox.Items.Add("Не депозитный");
		}

		private void OkButtonNewAccount_Click(object sender, RoutedEventArgs e)
		{
			string familyName = familyNameTextBox.Text;
			string firstname = firstNameTextBox.Text;
			string patronymic = patronymicTextBox.Text;
			string numberPhone = numberPhoneTextBox.Text;
			string serialDoc = serialDocTextBox.Text;
			string numberDoc = NumberDocTextBox.Text;
			string serialNumberDoc = serialDoc + " " + numberDoc;
			string balanceText = addBalanceTextBox.Text.Replace(",", ".");
			double balance = double.Parse(balanceText, NumberStyles.Any, CultureInfo.InvariantCulture);
			string accountType = accountTypeComboBox.SelectedItem.ToString();

			NewAccount = new Account(familyName, firstname, patronymic, numberPhone, serialNumberDoc, new List<BankAccount> { });
			BankAccount newBankAccount = _bankAccountFactory.Create(balance, accountType);
			NewAccount.BankAccounts.Add(newBankAccount);
			

			this.Close();
		}

	}
}
