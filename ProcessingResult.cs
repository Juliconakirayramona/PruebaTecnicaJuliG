using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TénicoJunior
{
	public class ProcessingResult
	{
		public List<RegisterDatos> Registros { get; set; } = new();
		public List<RegisterDatos> RegistrosAceptados { get; set; } = new();
		public List<String> Errores { get; set; } = new();
	}
}
