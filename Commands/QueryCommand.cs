using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json;

namespace AdvancedMonitoring;

public partial class AdvancedMonitoring
{
	/// <summary>
	/// Affiche en JSON dans la console : infos serveur (map, maxPlayers, serverIp, address, etc.) et liste des joueurs (players[]).
	/// Réservé au serveur (RCON / console). Utilisé par le panel.
	/// </summary>
	[ConsoleCommand("adv_query")]
	[CommandHelper(whoCanExecute: CommandUsage.SERVER_ONLY)]
	[RequiresPermissions("@css/root")]
	public void OnQueryCommand(CCSPlayerController? caller, CommandInfo command)
	{
		var allConnected = Utilities.GetPlayers()
			.Where(p => p != null && p.IsValid && p.Connected == PlayerConnectedState.PlayerConnected && !p.IsHLTV)
			.ToList();
		var playersToTarget = caller == null
			? allConnected
			: allConnected.Where(p => AdminManager.CanPlayerTarget(caller, p)).ToList();

		string map = Server.MapName;
		int playerCount = playersToTarget.Count();
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

		var (ctScore, tScore) = GetTeamScores();
		var teamScores = new { ct = ctScore, t = tScore };

		var server = new
		{
			map,
			playerCount,
			maxPlayers,
			teamScores,
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
				var kills = stats?.Kills ?? 0;
				var deaths = stats?.Deaths ?? 0;
				var headShotKills = stats?.HeadShotKills ?? 0;
				var hsPct = kills > 0 ? Math.Round((double)headShotKills / kills * 100, 1) : 0.0;
				var kd = deaths > 0 ? Math.Round((double)kills / deaths, 2) : (double)kills;
				return (object)new
				{
					id = p.UserId,
					steamid64 = p.AuthorizedSteamID?.SteamId64.ToString(),
					team = p.Team == CsTeam.Terrorist ? "T" : "CT",
					connectTime = GetPlayerConnectTimeSeconds(p) ?? 0,
					kills,
					deaths,
					hsPct,
					kd,
					score = p.Score
				};
			})
			.ToList();

		var json = JsonSerializer.Serialize(new { server, players = playerList });
		Server.PrintToConsole(json);
	}
}
