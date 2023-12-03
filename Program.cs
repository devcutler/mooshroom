using CommandLine;

namespace mooshroom;

using CommandLine.Text;
using mooshroom.API;
using Newtonsoft.Json;

class Program
{
	public static readonly string ManifestUrl = "https://launchermeta.mojang.com/mc/game/version_manifest.json";
	public class Options
	{
		[Option('r', "raw", Required = false, Default = false, HelpText = "Only output data. Good for scripts.")]
		public bool Raw { get; set; }
		[Option('l', "list", Required = false, Default = false, HelpText = "List available versions.", SetName = "flow")]
		public bool List { get; set; }
		[Option('f', "filter", HelpText = "The type of version entries you want to filter for.", Required = false)]
		public string? Filter { get; set; }
		[Option('d', "download", Required = false, Default = false, HelpText = "Download selected version.", SetName = "flow")]
		public bool Download { get; set; }
		[Value(0, HelpText = "The version you want to list matching or download.", Required = false, MetaName = "<game version>")]
		public string? Selected { get; set; }
		[Value(1, HelpText = "The kind of jar you want to download.", Default = "server", Required = false, MetaName = "<type>")]
		public string? JarType { get; set; }
		[Option('o', "output", HelpText = "Output jar to a specific file.", Required = false, Default = "minecraft_<type>.<game version>.jar")]
		public string? Output { get; set; }
	}
	static async Task<int> Main(string[] args)
	{
		ParserResult<Options> parserResult = Parser.Default.ParseArguments<Options>(args);
		if (args.Length == 0)
		{
			HelpText helpText = HelpText.AutoBuild(parserResult, e => e, h => h, false, Console.WindowWidth);
			Console.Write(helpText);
			return 0;
		}
		Options opts = parserResult.Value;
		HttpClient httpClient = new();
		string manifestJson = await httpClient.GetStringAsync(ManifestUrl);
		VersionManifest? manifest = JsonConvert.DeserializeObject<VersionManifest>(manifestJson, new JsonSerializerSettings()
		{
			MissingMemberHandling = MissingMemberHandling.Ignore
		});
		if (manifest == null)
		{
			Console.WriteLine("Failed to get manifest.");
			return 1;
		}
		if (opts.List)
		{
			if (!opts.Raw)
			{
				Console.WriteLine("Listing available versions" + (opts.Filter == null ? "" : (" with filter " + opts.Filter)) + ":");
			}
			IList<VersionEntry> entries = opts.Filter switch
			{
				null => manifest.Versions,
				"release_candidate" or "rc" => manifest.Versions.Where(entry => entry.Id.Contains("rc")).ToList(),
				_ => manifest.Versions.Where(entry => entry.Type == opts.Filter).ToList()
			};
			foreach (VersionEntry version in entries.Reverse())
			{
				if (opts.Raw)
				{
					Console.WriteLine(version.Id);
				}
				else
				{
					Console.WriteLine(version.Type + ": " + version.Id);
				}
			}
			if (!opts.Raw)
			{
				Console.WriteLine();
				Console.WriteLine("Latest release: " + manifest.Latest.Release);
			}
		}
		if (!opts.Download)
		{
			return 0;
		}
		VersionEntry entry = opts.Selected switch
		{
			"latest" or null => manifest.GetVersionEntry(manifest.Latest.Release),
			"snapshot" => manifest.GetVersionEntry(manifest.Latest.Snapshot),
			_ => manifest.GetVersionEntry(opts.Selected),
		};
		if (entry.Id == "NOTFOUND")
		{
			Console.WriteLine("No entry found with id: " + opts.Selected);
			return 1;
		}

		string entrydata = await httpClient.GetStringAsync(entry.Url);
		GameVersion? versiondata;
		try
		{
			versiondata = JsonConvert.DeserializeObject<GameVersion>(entrydata, new JsonSerializerSettings()
			{
				MissingMemberHandling = MissingMemberHandling.Ignore
			});
		}
		catch (Exception)
		{
			Console.WriteLine("Unable to parse JSON manifest.");
			return 1;
		}
		if (versiondata == null)
		{
			Console.WriteLine("Unable to get version info.");
			return 1;
		}
		Stream stream;
		string url = opts.JarType switch
		{
			"client" => versiondata.Downloads.Client.Url,
			"server" or _ => versiondata.Downloads.Server.Url
		};
		stream = await httpClient.GetStreamAsync(url);
		using FileStream file = new(opts.Output ?? "minecraft_server." + entry.Id + ".jar", FileMode.OpenOrCreate);
		stream.CopyTo(file);
		file.Close();
		return 0;
	}
}