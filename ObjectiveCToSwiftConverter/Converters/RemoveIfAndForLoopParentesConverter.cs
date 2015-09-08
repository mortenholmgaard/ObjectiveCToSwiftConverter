using System;
using System.Text.RegularExpressions;

namespace ObjectiveCToSwiftConverter.Converters
{
	public class RemoveIfAndForLoopParentesConverter : BaseConverter
	{
		public override string Convert(string content)
		{
			content = RemoveParentes(content);
			return content;
		}

		private string RemoveParentes(string content)
		{
			var matches =
				new Regex(@"(if(?<space> *)\(.* *[\){])|(else if(?<space> *)\(.* *[\){])|(for(?<space> *)\(.* *[\){])", RegexOptions.Multiline).Matches(content);
			foreach (Match match in matches)
			{
				var value = match.Value;
				var space = match.Groups["space"];
				var newValue = ReplaceLast(value, ")", "");
				newValue = ReplaceFirst(newValue, space + "(", " ");
				content = content.Replace(value, newValue);
			}
			return content;
		}
	}
}