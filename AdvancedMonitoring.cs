using System.Collections.Concurrent;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace AdvancedMonitoring;

[MinimumApiVersion(246)]
public partial class AdvancedMonitoring : BasePlugin, IPluginConfig<AdvancedMonitoringConfig>
{
	public override string ModuleName => "AdvancedMonitoring";
	public override string ModuleDescription => "AdvancedMonitoring for Counter-Strike 2.";
	public override string ModuleAuthor => "Snooze";
	public override string ModuleVersion => "1.0.0";

	public AdvancedMonitoringConfig Config { get; set; } = new();

	private readonly ConcurrentDictionary<uint, DateTime> _playerConnectTimes = new();
	private volatile int _ctScore, _tScore;

	public override void Load(bool hotReload) { }

	public override void Unload(bool hotReload) { }

	public void OnConfigParsed(AdvancedMonitoringConfig config)
	{
		Config = config;
	}

	[GameEventHandler]
	public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
	{
		var player = @event.Userid;
		if (player?.IsValid == true)
			_playerConnectTimes[player.Index] = DateTime.UtcNow;
		return HookResult.Continue;
	}

	[GameEventHandler]
	public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
	{
		var player = @event.Userid;
		if (player?.IsValid == true)
			_playerConnectTimes.TryRemove(player.Index, out _);
		return HookResult.Continue;
	}

	/// <summary>Réinitialise les scores quand le match commence (après warmup).</summary>
	[GameEventHandler]
	public HookResult OnRoundAnnounceMatchStart(EventRoundAnnounceMatchStart @event, GameEventInfo info)
	{
		_ctScore = 0;
		_tScore = 0;
		return HookResult.Continue;
	}

	/// <summary>Compte une manche gagnée pour l'équipe victorieuse (on ignore les rounds du warmup car reset au match start).</summary>
	[GameEventHandler]
	public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
	{
		var winner = @event.Winner;
		if (winner == (int)CsTeam.CounterTerrorist) _ctScore++;
		else if (winner == (int)CsTeam.Terrorist) _tScore++;
		return HookResult.Continue;
	}

	public (int ct, int t) GetTeamScores() => (_ctScore, _tScore);

	public int? GetPlayerConnectTimeSeconds(CCSPlayerController player)
	{
		if (player?.IsValid != true) return null;
		if (!_playerConnectTimes.TryGetValue(player.Index, out var connectTime)) return null;
		return (int)(DateTime.UtcNow - connectTime).TotalSeconds;
	}
}
