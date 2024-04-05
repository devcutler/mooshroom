using Newtonsoft.Json;

namespace mooshroom;
using mooshroom.API;

partial class Program
{
	public static async Task<VersionManifest> GetManifest(string url)
	{
		HttpClient httpClient = new();
		string manifestJson = await httpClient.GetStringAsync(url);
		VersionManifest? manifest = JsonConvert.DeserializeObject<VersionManifest>(manifestJson, new JsonSerializerSettings()
		{
			MissingMemberHandling = MissingMemberHandling.Ignore
		});
		if (manifest == null)
		{
			Console.WriteLine("Failed to get manifest.");
			Environment.Exit(1);
		}
		return manifest;
	}
}