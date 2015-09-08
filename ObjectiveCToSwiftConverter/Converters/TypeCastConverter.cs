using System.Text.RegularExpressions;

namespace ObjectiveCToSwiftConverter.Converters
{
	public class TypeCastConverter : BaseConverter
	{
		public override string Convert(string content)
		{
			content = ChangeTypeCasts(content);
			return content;
		}
		/// <summary>
		/// Note: It does not handle this case well: ((News *) visibleNews.objectAtIndex((NSUInteger), indexPath.row)).call() 
		/// => (visibleNews.objectAtIndex((NSUInteger), indexPath.row)).call() as! News // TODO Type cast is properly placed wrong
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		private static string ChangeTypeCasts(string content)
		{
			var matches =
				new Regex(@"\((?<class>[^ \*(]+) *\*\)", RegexOptions.Multiline).Matches(content);
			foreach (Match match in matches)
			{
				var value = match.Value;
				var startParentes = match.Groups["startParentes"];
				var clazz = match.Groups["class"];
				var cast = match.Groups["cast"].Value;

				if (value.Contains(@"@"""))
				{
					content = content.Replace(value, value + string.Format("// as! {0} // Not auto converted because of string", clazz));
					continue;
				}
				var typeCastValue = string.Format(" as! {0}", clazz);
				content = content.Replace(value, (cast.Length > 0 ? value.Replace(cast, "") : "") + typeCastValue 
					+ (startParentes.Value.Length > 0 ? " // TODO Type cast is properly placed wrong" : ""));
			}
			return content;
		}
	}
}