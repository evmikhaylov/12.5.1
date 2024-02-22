using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace _12._5._3
{

	public class Employee
	{
		public virtual bool ViewSerialNumberDoc { get;}
	}
	public class Consultant: Employee
	{
		public Consultant() { }
		public override bool ViewSerialNumberDoc => false;
	}
	public class Manager : Employee
	{
		public Manager() { }
		public override bool ViewSerialNumberDoc => true;
	}
	
}
