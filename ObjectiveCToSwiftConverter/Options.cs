using CommandLine;
using CommandLine.Text;

namespace ObjectiveCToSwiftConverter
{
	public class Options
	{
		[Option('f', "folder path", Required = true,
			HelpText = "Path to folder to be processed.")]
		public string FolderPath { get; set; }

		[Option('v', "verbose", DefaultValue = true,
			HelpText = "Prints all messages to standard output.")]
		public bool Verbose { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this,
			                          (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}