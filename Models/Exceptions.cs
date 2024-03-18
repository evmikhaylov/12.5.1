using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class NotANumberException : Exception
	{
		public NotANumberException() : base("Введеное значение суммы не является числом")
		{
		}

		public NotANumberException(string message)
			: base(message)
		{
		}

		public NotANumberException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
