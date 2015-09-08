using System;
using CommandLine;

namespace ObjectiveCToSwiftConverter
{
	class Program
	{
		static void Main(string[] args)
		{
			var options = new Options();
			if (Parser.Default.ParseArguments(args, options))
			{
				if (options.Verbose)
					Console.WriteLine("Folder path: {0}", options.FolderPath);

				var converter = new Converter(options);
				converter.Run();
			}
		}
	}
}
