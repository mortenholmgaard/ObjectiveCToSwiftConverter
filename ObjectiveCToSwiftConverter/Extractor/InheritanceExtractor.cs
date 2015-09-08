using System.Text.RegularExpressions;

namespace ObjectiveCToSwiftConverter.Extractor
{
	public class InheritanceExtractor : BaseExtractor
	{
		public override string Extract(string content)
		{
			var match =
				new Regex(@"@interface .* (?<inheritance>: *.*)", RegexOptions.Multiline).Match(content);
			return match.Groups["inheritance"].Value;
		}
	}
}