using CounterStrikeSharp.API.Modules.Cvars;
using System.Runtime.InteropServices;

namespace CSSPanel;

internal static class ServerInfoHelper
{
	public delegate nint CNetworkSystem_UpdatePublicIp(nint a1);
	private static CNetworkSystem_UpdatePublicIp? _networkSystemUpdatePublicIp;

	/// <summary>Adresse IP publique du serveur (via NetworkSystem).</summary>
	public static string GetServerIp()
	{
		var networkSystem = NativeAPI.GetValveInterface(0, "NetworkSystemVersion001");
		unsafe
		{
			if (_networkSystemUpdatePublicIp == null)
			{
				var funcPtr = *(nint*)(*(nint*)(networkSystem) + 256);
				_networkSystemUpdatePublicIp = Marshal.GetDelegateForFunctionPointer<CNetworkSystem_UpdatePublicIp>(funcPtr);
			}
			var ipBytes = (byte*)(_networkSystemUpdatePublicIp(networkSystem) + 4);
			return $"{ipBytes[0]}.{ipBytes[1]}.{ipBytes[2]}.{ipBytes[3]}";
		}
	}

	/// <summary>Adresse ip:port (convars ip + hostport).</summary>
	public static string GetServerAddress()
	{
		var ip = ConVar.Find("ip")?.StringValue ?? "0.0.0.0";
		var port = ConVar.Find("hostport")?.GetPrimitiveValue<int>().ToString() ?? "0";
		return $"{ip}:{port}";
	}
}
