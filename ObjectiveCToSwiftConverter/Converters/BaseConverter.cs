using System;

namespace ObjectiveCToSwiftConverter.Converters
{
	public abstract class BaseConverter
	{
		public abstract string Convert(string content);

		public string ReplaceLast(string source, string find, string replace)
		{
			var place = source.LastIndexOf(find, StringComparison.InvariantCulture);

			if (place == -1)
				return string.Empty;

			string result = source.Remove(place, find.Length).Insert(place, replace);
			return result;
		}

		public string ReplaceFirst(string source, string find, string replace)
		{
			var pos = source.IndexOf(find, StringComparison.InvariantCulture);
			if (pos < 0)
				return source;

			return source.Substring(0, pos) + replace + source.Substring(pos + find.Length);
		}
	}
}