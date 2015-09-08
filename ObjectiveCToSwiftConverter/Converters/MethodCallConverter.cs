using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace ObjectiveCToSwiftConverter.Converters
{
	public class MethodCallConverter : BaseConverter
	{
		//Cases:
		//[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(refreshData) name:NewsCategoriesUpdated object:nil];
		//[self.navigationController pushViewController:aboutController animated:YES]; => self.navigationController(aboutController, animated:YES)
		//[self loadData] => self.loadData()

		public override string Convert(string content)
		{
			int changes;
			do
			{
				content = ConvertMethodCall(content, out changes);
			} while (changes > 0);
			return content;
		}

		private string ConvertMethodCall(string content, out int changes)
		{
			var matches =
				new Regex(@"\[(?<class>[^\[\] ]+) (?<methodName>[^\[\]:]+):*(?<argument1>[^\[\] ]*) *(?<arguments>[^\[\]]*)\]", RegexOptions.Multiline).Matches(content);
			var matchedInvalidSigns = 0;
			foreach (Match match in matches)
			{
				var value = match.Value;
				if (value.Contains("^"))
				{
					matchedInvalidSigns++;
					continue;
				}
				var clazz = match.Groups["class"].Value;//or object
				var methodName = match.Groups["methodName"].Value;
				var argument1 = match.Groups["argument1"].Value;
				var arguments = match.Groups["arguments"].Value.Split(' ').Where(x => x != "").ToList();
				if (methodName == "stringWithFormat")
					content = HandleStringWithFormat(content, value, argument1, arguments);
				else
				{
					var newValue = string.Format("{0}.{1}({2}{3}{4})", clazz, methodName, argument1, arguments.Any() ? ", " : "", string.Join(", ", arguments));
					content = content.Replace(value, newValue);
				}
			}
			changes = matches.Count - matchedInvalidSigns;
			return content;
		}

		private static string HandleStringWithFormat(string content, string value, string argument1, List<string> arguments)
		{
			var newFormat = argument1.Trim(new[] { ',', ' ' });
			foreach (var argument in arguments)
				newFormat = new Regex("%.{1}").Replace(newFormat, string.Format("\\({0})", argument.Trim(new[] { ',', ' ' })), 1);
			content = content.Replace(value, newFormat);
			return content;
		}
	}
}