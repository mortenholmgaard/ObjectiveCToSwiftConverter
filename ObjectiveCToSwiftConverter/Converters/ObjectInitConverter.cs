using System.Text.RegularExpressions;

namespace ObjectiveCToSwiftConverter.Converters
{
	public class ObjectInitConverter : BaseConverter
	{
		// navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithImage:[UIImage imageNamed:@"preferences"] style:UIBarButtonItemStyleBordered target:self action:@selector(showSettings:)]
		// [[UIBarButtonItem alloc] init]
		// [UIBarButtonItem new]
		// UIBarButtonItem.new

		public override string Convert(string content)
		{
			content = ConvertInitWith(content);
			content = ConvertInit(content);
			content = ConvertNew(content);
			return content;
		}

		private static string ConvertInitWith(string content)
		{
			var matches =
				new Regex(@"\[{1}(?<class>[^\[]*) alloc\] initWith(?<firstLetter>.)", RegexOptions.Multiline).Matches(content);
			foreach (Match match in matches)
			{
				var value = match.Value;
				var clazz = match.Groups["class"];
				var firstInitLetter = match.Groups["firstLetter"].ToString();
				var newValue = string.Format("{0}({1}", clazz, firstInitLetter.ToLowerInvariant());
				content = content.Replace(value, newValue);

				var match2 = new Regex(@"\[(" + newValue.Replace("(", @"\(") + @".*)\]", RegexOptions.Multiline).Match(content);
                if (match2.Value.Length > 0)
				    content = content.Replace(match2.Value, match2.Groups[1] + ")");
			}
			return content;
		}

		private static string ConvertInit(string content)
		{
			var matches =
				new Regex(@"\[\[(?<class>[^\[]*) alloc\] init\]", RegexOptions.Multiline).Matches(content);
			foreach (Match match in matches)
			{
				var value = match.Value;
				var clazz = match.Groups["class"];
				var newValue = string.Format("{0}()", clazz);
				content = content.Replace(value, newValue);
			}
			return content;
		}

		private static string ConvertNew(string content)
		{
			var matches =
				new Regex(@"\[(?<class>[^\[]*) new\]|(?<class>[^\[\t ]*)\.new(?:[\n\r\t\.])", RegexOptions.Multiline).Matches(content);//TODO
			foreach (Match match in matches)
			{
				var value = match.Value;
				var clazz = match.Groups["class"];
				var newValue = string.Format("{0}(){1}", clazz, value.EndsWith(".") ? "." : "");
				content = content.Replace(value, newValue);
			}
			return content;
		}

	}
}