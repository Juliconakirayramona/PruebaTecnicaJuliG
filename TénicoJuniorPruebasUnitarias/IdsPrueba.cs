using Xunit;
using TénicoJunior;
namespace TécnicoJuniorPruebasUnitarias
{
	[TestClass]
	public class RegitserParsertests
	{
		[Fact]
		public void ProccesLines_DeberiaAceptarSequenceValida()
		{

			//Arrange
			var lines = new List<String>
			{
				"Id|Sequence|ServiceName|ServiceState|CashValue|TellerId|DateSettlement",
				"1|1|Retiro|Accepted|1500,50|123|2026-03-18"
			};

			var processor = new FileProcessor();

			//Act
			var result = processor.ProcessLines(lines);

			//Assert

			Xunit.Assert.Single(result.Registros);
			Xunit.Assert.Single(result.RegistrosAceptados);
			Xunit.Assert.Empty(result.Errores);
		}
		[Fact]
		public void ProccesLines_DeberiaRechazarSequenceInvalida()
		{

			//Arrange
			var lines = new List<String>
			{
				"Id|Sequence|ServiceName|ServiceState|CashValue|TellerId|DateSettlement",
				"ADC|100|Retiro|Accepted|1500,50|123|2026-03-18"
			};

			var processor = new FileProcessor();

			//Act
			var result = processor.ProcessLines(lines);

			//Assert

			Xunit.Assert.Empty(result.RegistrosAceptados);
			Xunit.Assert.Single(result.Errores);
			Xunit.Assert.Contains("ID invalido", result.Errores[0]);
		}
	}
}
