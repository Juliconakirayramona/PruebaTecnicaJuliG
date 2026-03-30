
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using TénicoJunior;

class TecnicoJuniorProgram
{
	static void Main()
	{
		String archivePath = @"C:\Users\jgarciag\Downloads\archivo_prueba.txt";
		String archivePathErrores = @"C:\Users\jgarciag\Downloads\Rechazos_archivo_prueba.txt";
		
		
		if(!File.Exists(archivePath))
		{
			Console.WriteLine("El archivo no existe");
			return;
		}
		var lines = File.ReadAllLines(archivePath).ToList();

		var processor = new FileProcessor();
		var result = processor.ProcessLines(lines);

		Int16 totalRechazados = (Int16)result.Errores.Count;
		Int16 totalAceptados = (Int16)result.Registros.Count;
		Int16 totalResultados = (Int16)(totalAceptados - totalRechazados);

		



		Console.WriteLine($"Total registros: {result.Registros.Count}");
		Console.WriteLine($"Total registros aceptados: {totalResultados}");
		Console.WriteLine($"Total registros rechazado: {result.Errores.Count}");

		File.WriteAllLines(archivePathErrores, result.Errores);
		

	}
}

