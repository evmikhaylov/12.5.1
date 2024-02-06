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
	/// Логика взаимодействия для AddAccountWindow.xaml
	/// </summary>
	public partial class AddAccountWindow : Window
	{
		public decimal addBalance { get; private set; }
		public string addFullName { get; private set; }
		public AddAccountWindow()
		{
			InitializeComponent();

		}

			private void OkButton_Click(object sender, RoutedEventArgs e)
			{
				decimal balance;
				addFullName = fullNameTextBox.Text;
			string balanceText = addBalanceTextBox.Text.Replace(',','.');
				if (decimal.TryParse(balanceText, NumberStyles.Any, CultureInfo.InvariantCulture, out balance))
				{
				addBalance = balance;
				DialogResult = true;
				}
				else
			{
				MessageBox.Show("Некорректный ввод суммы.");
			}	
				
			}
		
	}
}
