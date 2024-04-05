using CommandLine;

namespace mooshroom;

partial class Program
{
	public static readonly string ManifestUrl = "https://launchermeta.mojang.com/mc/game/version_manifest.json";

	public class BaseOptions
	{
		[Option('r', "raw", Required = false, Default = false, HelpText = "Only output data. Good for scripts.")]
		public bool Raw { get; set; }
		[Option('t', "type", HelpText = "The kind of jar you want to download.", Default = "server", Required = false, MetaValue = "<client/server>")]
		public string? JarType { get; set; }
		[Option('o', "output", HelpText = "Output jar to a specific file.", Required = false)]
		public string? Output { get; set; }
	}

	[Verb("download", HelpText = "List available versions.")]
	public class DownloadOptions : BaseOptions
	{
		[Value(0, HelpText = "The version you want to list matching or download.", Required = false, MetaName = "<game version>")]
		public string? Selected { get; set; }
	}
	[Verb("list", HelpText = "Download selected version.")]
	public class ListOptions : BaseOptions
	{
		[Option('f', "filter", HelpText = "The type of version entries you want to filter for.", Required = false)]
		public string? Filter { get; set; }
	}

	static async Task<int> Main(string[] args)
	{
		return await Parser.Default.ParseArguments<DownloadOptions, ListOptions>(args).MapResult(
			async (ListOptions options) => { return await List(options); },
			async (DownloadOptions options) => { return await Download(options); },
			(errors) => Task.FromResult(1)
		);
	}
}