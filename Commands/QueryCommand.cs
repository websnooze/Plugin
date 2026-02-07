using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using Newtonsoft.Json;

namespace CSSPanel;

public partial class CSSPanel
{
	/// <summary>
	/// Affiche en JSON dans la console : infos serveur (map, maxPlayers, serverIp, address, etc.) et liste des joueurs (players[]).
	/// Réservé au serveur (RCON / console). Utilisé par le panel.
	/// </summary>
	[ConsoleCommand("css_query")]
	[CommandHelper(whoCanExecute: CommandUsage.SERVER_ONLY)]
	[RequiresPermissions("@css/root")]
	public void OnQueryCommand(CCSPlayerController? caller, CommandInfo command)
	{
		var allConnected = Utilities.GetPlayers()
			.Where(p => p != null && p.IsValid && p.Connected == PlayerConnectedState.PlayerConnected && !p.IsHLTV)
			.ToList();
		var playersToTarget = caller == null
			? allConnected
			: allConnected.Where(p => caller.CanTarget(p)).ToList();

		string map = Server.MapName;
		int playerCount = playersToTarget.Count;
		int maxPlayers = Server.MaxPlayers;
		string[] maps;
		try { maps = Server.GetMapList(); }
		catch { maps = Array.Empty<string>(); }

		string serverIp = "";
		string address = "";
		try
		{
			serverIp = ServerInfoHelper.GetServerIp();
			address = ServerInfoHelper.GetServerAddress();
		}
		catch { /* convars/native peuvent être indisponibles */ }

		var server = new
		{
			map,
			p = playerCount,
			mP = maxPlayers,
			serverIp,
			address,
			pr = ModuleVersion,
			maps
		};

		var playerList = playersToTarget
			.Where(p => !p.IsBot && !p.IsHLTV && !string.IsNullOrEmpty(p.PlayerName))
			.Select(p =>
			{
				var stats = p.ActionTrackingServices?.MatchStats;
				return (object)new
				{
					id = p.UserId,
					s64 = p.AuthorizedSteamID?.SteamId64.ToString(),
					t = p.Team,
					k = stats?.Kills.ToString() ?? "0",
					d = stats?.Deaths.ToString() ?? "0",
					s = p.Score
				};
			})
			.ToList();

		var json = JsonConvert.SerializeObject(new { server, players = playerList });
		Server.PrintToConsole(json);
	}
}
