namespace mooshroom;
using mooshroom.API;

partial class Program
{
	static async Task<int> List(ListOptions opts)
	{
		if (!opts.Raw)
		{
			Console.WriteLine("Listing available versions" + (opts.Filter == null ? "" : (" with filter " + opts.Filter)) + ":");
		}
		VersionManifest manifest = await GetManifest(ManifestUrl);
		IList<VersionEntry> entries = opts.Filter switch
		{
			null => manifest.Versions,
			"release_candidate" or "rc" => manifest.Versions.Where(entry => entry.Id.Contains("rc")).ToList(),
			_ => manifest.Versions.Where(entry => entry.Type == opts.Filter).ToList()
		};
		foreach (VersionEntry version in entries.Reverse())
		{
			if (!opts.Raw)
			{
				Console.Write(version.Type + ": ");
			}
			Console.WriteLine(version.Id);
		}
		if (!opts.Raw)
		{
			Console.WriteLine();
			Console.WriteLine("Latest release: " + manifest.Latest.Release);
		}
		return 0;
	}
}