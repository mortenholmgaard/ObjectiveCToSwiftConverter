using System.Text;
using System.Text.RegularExpressions;

namespace ObjectiveCToSwiftConverter.Extractor
{
	public class PropertiesExtractor : BaseExtractor
	{
		public override string Extract(string content)
		{
			var swiftProperties = new StringBuilder();

			var matches =
				new Regex(@"@property *\(.*\) *(?<ib>IB\w+)* *(?<class>\w+) *\** *(?<varName>.+);", RegexOptions.Multiline).Matches(content);
			foreach (Match match in matches)
			{
				var ib = match.Groups["ib"].Value;
				var clazz = match.Groups["class"];
				var varName = match.Groups["varName"];
				var ibFormattet = ib.Length > 0 ? string.Format("@{0} weak ", ib) : "";
				swiftProperties.AppendFormat("\t{0}var {1}: {2}?", ibFormattet, varName, clazz).AppendLine();
			}
			return swiftProperties.ToString();
		}
	}
}