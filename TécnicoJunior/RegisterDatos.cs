using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TénicoJunior
{
	public class RegisterDatos
	{
		public Int32 Id { get; set; }
		public Int32 Sequence { get; set; }
		public String ServiceName { get; set; }
		public String ServiceState { get; set; }
		public Decimal CashValue { get; set; }
		public Int16 TellerId { get; set; }
		public DateTime DateSettlement { get; set; }
	}

}
	