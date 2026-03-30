using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TénicoJunior
{
	public class FileProcessor
	{
		public ProcessingResult ProcessLines(List<String> lines)
		{
			String archivoResumen = @"C:\Users\jgarciag\Downloads\Resumen_archivo_prueba.txt";

			var registerDataList = new List<RegisterDatos>();
			var registerDataListAccepted = new List<RegisterDatos>();
			var registerStatesServicesAccepted = new List<RegisterDatos>();
			var registerErroresDataList = new List<String>();
			HashSet<Int32> idLeidos = new HashSet<Int32>();
			HashSet<Int32> sequenceLeidas = new HashSet<Int32>();

			String header = lines[0];
			String[] columns = header.Split("|");

			//String line;
			Int16 numeroLinea = 0;

			foreach (var line in lines.Skip(1))
			{
				String[] values = line.Split("|");
				String filaCompleta = String.Join("|", values);
				Boolean lineValida = true;
				numeroLinea++;

				var readedData = new RegisterDatos();

				//Validaciones

				//Id
				if (!Int32.TryParse(values[Array.IndexOf(columns, "Id")], out Int32 Id) || Id <= 0)
				{
					registerErroresDataList.Add
						($"Línea {numeroLinea}: ID invalido {filaCompleta}");
					lineValida = false;
				}
				else if (idLeidos.Contains(Id))
				{
					registerErroresDataList.Add($"Línea {numeroLinea}: ID repetido -> '{Id}' | Fila: {filaCompleta}");
					lineValida = false;
				}
				else
				{
					idLeidos.Add(Id);
					readedData.Id = Id;
					//registerDataList.Add([values[Array.IndexOf(columns, "Id")]]);
				}

				//Secuencia
				if (!Int32.TryParse(values[Array.IndexOf(columns, "Sequence")], out Int32 Sequence) || Sequence <= 0)
				{
					registerErroresDataList.Add
						($"Línea {numeroLinea}: Secuencia inválida -> '{values[Array.IndexOf(columns, "Sequence")]}'");
					lineValida = false;
				}
				else if (sequenceLeidas.Contains(Sequence))
				{
					registerErroresDataList.Add($"Línea {numeroLinea}: Sequence repetido -> '{Sequence}' | Fila: {filaCompleta}");
					lineValida = false;
				}
				else
				{
					sequenceLeidas.Add(Sequence);
					readedData.Sequence = Sequence;
				}

				//ServiceName
				String serviceNameValue = values[Array.IndexOf(columns, "ServiceName")];

				if (String.IsNullOrWhiteSpace(serviceNameValue))
				{
					registerErroresDataList.Add
						($"Línea {numeroLinea}: Nombre del Servicio inválido -> '{serviceNameValue}'");
					lineValida = false;
				}
				else
				{
					readedData.ServiceName = serviceNameValue;
				}

				//ServiceState
				String serviceStateValue = values[Array.IndexOf(columns, "ServiceState")];

				if (serviceStateValue != "Accepted" && serviceStateValue != "Rejected")
				{
					registerErroresDataList.Add
						($"Línea {numeroLinea}: Estado inválido -> '{serviceStateValue}'");
					lineValida = false;
					readedData.ServiceState = serviceStateValue;
				}
				else
				{
					readedData.ServiceState = serviceStateValue;
				}

				//CashValue
				String cashValueStr = values[Array.IndexOf(columns, "CashValue")];

				if (cashValueStr.Contains("."))
				{
					registerErroresDataList.Add
						($"Línea {numeroLinea}: Valor de Dinero inválido -> '{cashValueStr}'");
					lineValida = false;
					readedData.CashValue = Convert.ToDecimal(cashValueStr);
				}
				else if (!Decimal.TryParse(cashValueStr, NumberStyles.Number, new CultureInfo("es-Es"), out Decimal CashValue) || CashValue < 0)
				{
					registerErroresDataList.Add
						($"Línea {numeroLinea}: Valor de Dinero inválido -> '{cashValueStr}'");
					lineValida = false;
					readedData.CashValue = CashValue;

				}
				else
				{
					if (serviceStateValue == "Rejected" && CashValue != 0.00m)
					{
						registerErroresDataList.Add(
							$"Línea {numeroLinea}: CashValue debe ser 0,00 cuando el estado es 'Rejected' -> " +
							$"'{cashValueStr}' | Fila: {filaCompleta}"
							);
						lineValida = false;
					}
					readedData.CashValue = CashValue;
				}


				//TellerId
				String tellerIdValues = values[Array.IndexOf(columns, "TellerId")];
				Int16 tellerIdCount = (Int16)tellerIdValues.ToString().Length;

				if (tellerIdCount == 2 || tellerIdCount == 4)
				{
					registerErroresDataList.Add
						($"Línea {numeroLinea}: TellerId inválido -> '{values[Array.IndexOf(columns, "TellerId")]}'");
					lineValida = false;
				}
				else if (!Int16.TryParse(tellerIdValues, NumberStyles.Number, new CultureInfo("es-Es"), out Int16 TellerId) || TellerId < 0)
				{
					registerErroresDataList.Add
						($"Línea {numeroLinea}: TellerId inválido -> '{values[Array.IndexOf(columns, "TellerId")]}'");
					lineValida = false;
				}
				else
				{
					readedData.TellerId = TellerId;
				}

				String dateSettElementFormat = "yyyy-MM-dd";
				String dateSettElementValues = values[Array.IndexOf(columns, "DateSettlement")]?.Trim();

				DateTime DateSettlement;
				Boolean dateTimeValid = DateTime.TryParseExact(
					dateSettElementValues,
					dateSettElementFormat,
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out DateSettlement
					);

				if (!dateTimeValid)
				{
					registerErroresDataList.Add
						($"Línea {numeroLinea}: Fecha inválida -> '{values[Array.IndexOf(columns, "DateSettlement")]}'");
					lineValida = false;
				}
				else
				{
					readedData.DateSettlement = DateSettlement;
				}
				if (lineValida)
				{
					registerDataListAccepted.Add(readedData);
				}
				registerDataList.Add(readedData);

				if (String.Equals(readedData.ServiceState, "Accepted",
					StringComparison.OrdinalIgnoreCase))
				{
					registerStatesServicesAccepted.Add(readedData);
				}
			}

			try
			{

				Int16 cuentaTotalTodos = (Int16)registerDataList.Count;

				//foreach (var p in registerDataListAccepted)
				//{
				//	Console.WriteLine($"{p.Id} - {p.Sequence} - {p.ServiceName} - {p.ServiceState} - {p.CashValue} - {p.TellerId} - {p.DateSettlement}");
				//}
				Int16 cuentaTotalrechazados = (Int16)registerErroresDataList.Count;
				Int16 total = (Int16)(cuentaTotalTodos - cuentaTotalrechazados);

				var totalCashValue = registerDataListAccepted.Where
					(s => s.ServiceState.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
					.GroupBy(f => f.DateSettlement.Date)
					.Select(g => new
					{
						date = g.Key.ToString("yyyy-MM-dd"),
						totalCashValues = g.Sum(c => c.CashValue)
					})
					.OrderBy(f => f.date)
					.ToList();
				foreach (var item in totalCashValue)
				{
					Console.WriteLine($"Fecha: {item.date: yyyy-MM-dd}, Total: {item.totalCashValues:N2}");
				}

				var totalCashValueAtm = registerDataList.Where
					(s => s.ServiceState.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
					.GroupBy(f => f.TellerId)
					.Select(g => new
					{
						CodigoAtm = g.Key,
						totalCashValues = g.Sum(c => c.CashValue),
					})
					.OrderBy(f => f.CodigoAtm)
					.ToList();
				foreach (var item in totalCashValueAtm)
				{
					Console.WriteLine($"Estados: {item.CodigoAtm}, Total: {item.totalCashValues:N2}");
				}

				//File.WriteAllLines(archivePathErrores, registerErroresDataList);
				//Console.WriteLine($"Total registros: {registerDataList.Count}");
				//Console.WriteLine($"Total de registros Aceptados: {registerDataListAccepted.Count}");
				//Console.WriteLine($"Total de registros rechazados: {registerErroresDataList.Count}");
				//Console.WriteLine($"Archivo creado en: {archivePathErrores}");
				//Console.WriteLine($"Archivo creado en: {archivePathErrores}");

				var resultado = new
				{
					TotalRegistros = registerDataList.Count,
					TotalAceptados = total,
					TotalRechazados = registerErroresDataList.Count,
					SumatoriaFechas = totalCashValue,
					SumatoriasCajeros = totalCashValueAtm
				};

				var jsonOptions = new JsonSerializerOptions
				{
					WriteIndented = true
				};

				String json = JsonSerializer.Serialize(resultado, jsonOptions);
				Console.WriteLine(json);
				File.WriteAllText(archivoResumen, json);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			return new ProcessingResult
			{
				Registros = registerDataList,
				RegistrosAceptados = registerDataListAccepted,
				Errores = registerErroresDataList
			};
		}
	}
}

