using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace _12._5._3.ViewModels
{
	public class SerialNumberConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is string serialNumber && values[1] is Employee employee)
			{
				return employee is Manager ? serialNumber : new string('*', serialNumber.Length);
			}
			else if (values[0] is string serialNumberWhenNull)
			{
				return new string('*', serialNumberWhenNull.Length);
			}
			return values[0];
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
