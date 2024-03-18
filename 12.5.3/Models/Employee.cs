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

	public abstract class Employee
	{
		public virtual bool ViewSerialNumberDoc { get;}
		public virtual bool CanDeposit { get;}
		public virtual bool CanWithdraw { get; }
		public virtual bool CanAddAccount { get; }
		public virtual bool CanAddBankAccount { get; }
		public virtual bool CanDeleteBankAccount { get; }
		public virtual bool CanChangeInfoClient { get; }
		public virtual bool CanTransfer { get; }

	}
	public class Consultant: Employee
	{
		public Consultant() { }
		public override bool ViewSerialNumberDoc => false;
		public override bool CanDeposit => true;
		public override bool CanWithdraw => false;
		public override bool CanAddAccount => false;
		public override bool CanAddBankAccount => true;
		public override bool CanDeleteBankAccount => false;
		public override bool CanChangeInfoClient => true;
		public override bool CanTransfer => true;

	}
	public class Manager : Employee
	{
		public Manager() { }
		public override bool ViewSerialNumberDoc => true;
		public override bool CanDeposit => true;
		public override bool CanWithdraw => true;
		public override bool CanAddAccount => true;
		public override bool CanAddBankAccount => true;
		public override bool CanDeleteBankAccount => true;
		public override bool CanChangeInfoClient => true;
		public override bool CanTransfer => true;

	}
	
}
