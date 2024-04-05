using mooshroom.API;
using Newtonsoft.Json;

namespace mooshroom;

partial class Program
{
	static async Task<int> Download(DownloadOptions opts)
	{
		VersionManifest manifest = await GetManifest(ManifestUrl);
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

		string entrydata = await new HttpClient().GetStringAsync(entry.Url);
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
		stream = await new HttpClient().GetStreamAsync(url);
		Console.WriteLine(entry.Id);
		using FileStream file = new(opts.Output ?? "minecraft_server." + entry.Id + ".jar", FileMode.OpenOrCreate);
		stream.CopyTo(file);
		return 0;
	}
}