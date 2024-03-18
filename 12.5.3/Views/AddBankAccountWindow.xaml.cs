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
using _12._5._3.ViewModels;

namespace _12._5._3
{
	/// <summary>
	/// Логика взаимодействия для AddBankAccountWindow.xaml
	/// </summary>
	public partial class AddBankAccountWindow : Window
	{
		public AddBankAccountWindow(AddBankAccountViewModel addBankAccountViewModel)
		{
			InitializeComponent();
			DataContext = addBankAccountViewModel;
			addBankAccountViewModel.CloseAction = new Action<bool>((dialogResult) =>
			{
				if (dialogResult)
				{
					this.DialogResult = true;
					this.Close();
				}
			});
		}


	}
}
