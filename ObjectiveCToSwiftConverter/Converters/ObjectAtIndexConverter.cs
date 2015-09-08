using System.Text;
using System.Text.RegularExpressions;

namespace ObjectiveCToSwiftConverter.Converters
{
	public class ObjectAtIndexConverter : BaseConverter
	{
		public override string Convert(string content)
		{
			content = ChangeObjectAtIndex(content);
			return content;
		}

		private string ChangeObjectAtIndex(string content)
		{
			var matches =
				new Regex(@".objectAtIndex\((.*)\)", RegexOptions.Multiline).Matches(content);
			foreach (Match match in matches)
			{
				var value = match.Value;
				var index = match.Groups[1].Value;
				string callOnIndex;
				index = HandleFunctionCallInIndex(index, out callOnIndex);
				content = content.Replace(value, string.Format("[{0}]{1}", index, callOnIndex));
			}
			return content;
		}

		private string HandleFunctionCallInIndex(string index, out string callOnIndex)
		{
			var startParentes = 1;
			var endParentes = 0;

			var stringBuilder = new StringBuilder();
			for (var i = 0; i < index.Length; i++)
			{
				if (index[i] == '(')
					startParentes++;
				if (index[i] == ')')
				{
					endParentes++;
					if (startParentes == endParentes)
					{
						callOnIndex = index.Length > i + 1 ? index.Substring(i+1) + ")" : "";
						return stringBuilder.ToString();
					}
				}
				stringBuilder.Append(index[i]);
			}
			callOnIndex = "";
			return stringBuilder.ToString();
		}
	}
}