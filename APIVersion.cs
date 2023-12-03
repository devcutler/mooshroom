namespace mooshroom.API;

public class Client
{
	public required string Sha1 { get; set; }
	public required int Size { get; set; }
	public required string Url { get; set; }
}
public class ClientMappings
{
	public required string Sha1 { get; set; }
	public required int Size { get; set; }
	public required string Url { get; set; }
}
public class Server
{
	public required string Sha1 { get; set; }
	public required int Size { get; set; }
	public required string Url { get; set; }
}
public class ServerMappings
{
	public required string Sha1 { get; set; }
	public required int Size { get; set; }
	public required string Url { get; set; }
}
public class Downloads
{
	public required Client Client { get; set; }
	public required ClientMappings ClientMappings { get; set; }
	public required Server Server { get; set; }
	public required ServerMappings ServerMappings { get; set; }
}
public class GameVersion
{
	public required Downloads Downloads { get; set; }
}