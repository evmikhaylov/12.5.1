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
	/// Логика взаимодействия для RoleSelectionWindow.xaml
	/// </summary>
	public partial class RoleSelectionWindow : Window
	{
		public string SelectedRole { get; private set; }

		public RoleSelectionWindow()
		{
			InitializeComponent();

			RoleComboBox.Items.Add("Консультант");
			RoleComboBox.Items.Add("Менеджер");
		}


		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			SelectedRole = RoleComboBox.SelectedItem.ToString();
			this.DialogResult = true;
			this.Close();
		}
	}
}
