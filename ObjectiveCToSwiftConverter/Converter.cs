using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ObjectiveCToSwiftConverter.Converters;
using ObjectiveCToSwiftConverter.Extractor;

namespace ObjectiveCToSwiftConverter
{
	public class Converter
	{
		private readonly Options _options;

		public Converter(Options options)
		{
			_options = options;
		}

		public void Run()
		{
			if (!Directory.Exists(_options.FolderPath))
				return;

			var mFiles = Directory.GetFiles(_options.FolderPath, "*.m", SearchOption.AllDirectories);
			foreach (var file in mFiles)
			{
				if (file.EndsWith(".m"))
				{
					var content = File.ReadAllText(file);
					var swiftContent = ConvertMFileToSwift(content);
					using (var streamWriter = File.CreateText(file.Replace("." + file.Split('.').Last(), ".swift")))
					{
						streamWriter.Write(swiftContent);
					}
				}
			}

			var hFiles = Directory.GetFiles(_options.FolderPath, "*.h", SearchOption.AllDirectories);
			foreach (var file in hFiles)
			{
				if (file.EndsWith(".h"))
				{
					var swiftFilePath = file.Replace("." + file.Split('.').Last(), ".swift");
					var swiftFileName = file.Replace("." + file.Split('.').Last(), ".swift").Split('\\').Last();
					var content = File.ReadAllText(file);
					var swiftFile = Directory.GetFiles(_options.FolderPath, swiftFileName, SearchOption.AllDirectories).FirstOrDefault();
					if (swiftFile == null)
					{
						//Not handled delegate/protocol - direct transfer to swift file
						using (var streamWriter = File.CreateText(swiftFilePath))
						{
							streamWriter.Write(content);
						}
						continue;
					}
					var swiftContent = File.ReadAllText(swiftFile);
					swiftContent = ConvertHFileToSwiftAndInsertInExistingFile(content, swiftContent);
					using (var streamWriter = File.CreateText(swiftFilePath))
					{
						streamWriter.Write(swiftContent);
					}
				}
			}
		}

		private static string ConvertHFileToSwiftAndInsertInExistingFile(string content, string swiftContent)
		{
			var imports = new ImportsExtractor().Extract(content);//Insert after last #import line
			var lastImportIndex = swiftContent.LastIndexOf("#import", StringComparison.OrdinalIgnoreCase);
			var importsInsertIndex = lastImportIndex == -1 ? 0 : swiftContent.IndexOf("\n", lastImportIndex, StringComparison.OrdinalIgnoreCase) + 1;
			swiftContent = swiftContent.Insert(importsInsertIndex, imports);

			var match = Regex.Match(swiftContent, @"@implementation \w+(.?)");
			if (match.Success)
			{
				var inheritanceInsertIndex = match.Groups[1].Index + 1;
				var properties = new PropertiesExtractor().Extract(content); // after @implementation Controller
				swiftContent = swiftContent.Insert(inheritanceInsertIndex, properties);
				var inheritance = new InheritanceExtractor().Extract(content); // after @implementation Controller
				swiftContent = swiftContent.Insert(inheritanceInsertIndex, inheritance);
			}
			return swiftContent;
		}

		/// <summary>
		/// The order is important
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		private static string ConvertMFileToSwift(string content)
		{
			var swiftContent = content.Replace(";", "");
			swiftContent = swiftContent.Replace("self.", "");
			swiftContent = ReplaceDeclarationWithLet(swiftContent);
			swiftContent = swiftContent.Replace("YES", "true").Replace("NO", "false");
			swiftContent = RenameStdTypes(swiftContent);
			swiftContent = new MethodDeclarationConverter().Convert(swiftContent); //Uncommented only while testing other converters because this converter is slow
			swiftContent = new ObjectInitConverter().Convert(swiftContent);
			swiftContent = new MethodCallConverter().Convert(swiftContent);
			swiftContent = new RemoveIfAndForLoopParentesConverter().Convert(swiftContent);
			swiftContent = new TypeCastConverter().Convert(swiftContent);
			swiftContent = new ObjectAtIndexConverter().Convert(swiftContent);
			swiftContent = new Regex(@"([^%])(@"")").Replace(swiftContent, "$1\"");
			swiftContent = ReplaceSelectorSyntax(swiftContent);
			return swiftContent;
		}

		private static string ReplaceDeclarationWithLet(string content)
		{
			content = new Regex(@"([\t ]+)(?!static )([a-zA-Z ]+\* *)").Replace(content, "$1let ");
			content = new Regex(@"([\t ]+)(static [a-zA-Z ]+\* *)").Replace(content, "$1static let ");
			content = new Regex(@"([\t ]+)(for *\([a-zA-Z ]+\* *)").Replace(content, "$1for (");
			return content;
		}

		private static string RenameStdTypes(string content)
		{
			content = content.Replace("NSString", "String");
			content = content.Replace("NSUInteger", "UInt");
			content = content.Replace("NSMutableArray", "[UknownArrayType]");
			content = content.Replace("NSArray", "[UknownArrayType]");
			content = content.Replace("NSUInteger", "UInt");
			content = content.Replace("NSInteger", "Int");
			content = content.Replace("NSLog", "println");
			//NSNumber?
			return content;
		}

		private static string ReplaceSelectorSyntax(string content)
		{
			var matches = new Regex(@"@selector\(([^\)]*)\)").Matches(content);
			foreach (Match match in matches)
			{
				var value = match.Value;
				var selector = match.Groups[1];
				content = content.Replace(value, string.Format("Selector(\"{0}\")", selector));
			}
			return content;
		}
	}
}