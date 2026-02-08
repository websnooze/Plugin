using CounterStrikeSharp.API.Modules.Cvars;

namespace AdvancedMonitoring;

internal static class ServerInfoHelper
{
	/// <summary>Adresse IP du serveur (convar ip). Note : souvent 0.0.0.0 si le serveur écoute sur toutes les interfaces.</summary>
	public static string GetServerIp()
	{
		return ConVar.Find("ip")?.StringValue ?? "0.0.0.0";
	}

	/// <summary>Adresse ip:port (convars ip + hostport).</summary>
	public static string GetServerAddress()
	{
		var ip = ConVar.Find("ip")?.StringValue ?? "0.0.0.0";
		var port = ConVar.Find("hostport")?.GetPrimitiveValue<int>().ToString() ?? "0";
		return $"{ip}:{port}";
	}
}
