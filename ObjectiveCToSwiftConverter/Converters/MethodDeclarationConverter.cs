using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ObjectiveCToSwiftConverter.Converters
{
	public class MethodDeclarationConverter : BaseConverter
	{
		public override string Convert(string content)
		{
			var matches = new Regex(@"^([-+]+ ?\(.*)", RegexOptions.Multiline).Matches(content);
			foreach (Match match in matches)
			{
				var value = match.Value;
				var result = ConvertMethodDeclaration(value);
				if (result == null)
					content = content.Replace(value, value + " // Failed to auto convert");
				else
				{
					result = result.Split(value.Contains("{") ? '\n' : '{').First();
					content = content.Replace(value, result);
				}
			}
			return content;
		}

		private string ConvertMethodDeclaration(string methodDeclaration)
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				string methodDeclarationSwift = null;
				var task = client.PostAsJsonAsync<object>("http://objectivec2swift.net/api/Converter/ConvertCode", new
				{
					value = methodDeclaration,

				}).ContinueWith((taskwithresponse) =>
				{
					var response = taskwithresponse.Result;
					var jsonString = response.Content.ReadAsStringAsync();
					jsonString.Wait();
					var result = JsonConvert.DeserializeObject<JObject>(jsonString.Result);
					methodDeclarationSwift = result["response"].Value<string>();
				});
				task.Wait();

				return methodDeclarationSwift;
			}
		}
	}
}