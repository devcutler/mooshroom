namespace mooshroom.API;

public class Latest
{
	public required string Release { get; set; }
	public required string Snapshot { get; set; }
}
public class VersionEntry
{
	public required string Id { get; set; }
	public required string Type { get; set; }
	public required string Url { get; set; }
}
public class VersionManifest
{
	public required Latest Latest { get; set; }
	public required IList<VersionEntry> Versions { get; set; }

	public VersionEntry GetVersionEntry(string id)
	{
		VersionEntry? entry = Versions.ToList().Find(item => item.Id == id);
		if (entry == null)
		{
			return new VersionEntry()
			{
				Id = "NOTFOUND",
				Type = "",
				Url = ""
			};
		}
		return entry;
	}
}