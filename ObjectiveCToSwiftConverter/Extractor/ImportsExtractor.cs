using System.Text;
using System.Text.RegularExpressions;

namespace ObjectiveCToSwiftConverter.Extractor
{
	public class ImportsExtractor : BaseExtractor
	{
		public override string Extract(string content)
		{
			var swiftImports = new StringBuilder();

			var matches =
				new Regex(@"#(import .*)", RegexOptions.Multiline).Matches(content);
			foreach (Match match in matches)
			{
				swiftImports.AppendLine(match.Groups[0].Value);
			}
			return swiftImports.ToString();
		}
	}
}