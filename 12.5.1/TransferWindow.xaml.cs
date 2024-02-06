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
using System.Windows.Shapes;

namespace _12._5._1
{
	/// <summary>
	/// Логика взаимодействия для transferWindow.xaml
	/// </summary>
	public partial class TransferWindow : Window
	{
		private Bank bank;
		public string FromAccountId { get; private set; }
		public string ToAccountId { get; private set; }
		public decimal Amount { get; private set; }
		public TransferWindow(List<string> accountIds, Bank bank)
		{
			InitializeComponent();

			fromAccountComboBox.ItemsSource = accountIds;
			toAccountComboBox.ItemsSource = accountIds;
				this.bank = bank;
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			FromAccountId = fromAccountComboBox.SelectedItem.ToString();
			ToAccountId = toAccountComboBox.SelectedItem.ToString();

			decimal amount;

			if (decimal.TryParse(amountTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
			{
				var fromAccount = bank.GetAccount(FromAccountId);
				if (fromAccount.Balance>=amount)
				{
					Amount = amount;
					DialogResult = true;
				}
				else
				{
					MessageBox.Show("Недостаточно средств");
				}
			}
			else
			{
				MessageBox.Show("Некорректная сумма");
			}
        }
    }
}
