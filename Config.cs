using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace CSSPanel;

public class CSSPanelConfig : BasePluginConfig
{
	[JsonPropertyName("ConfigVersion")]
	public override int Version { get; set; } = 1;

	// Paramètres futurs possibles (exemples)
	// [JsonPropertyName("QueryIncludeBots")] public bool QueryIncludeBots { get; set; } = false;
	// [JsonPropertyName("QueryExtraFields")] public List<string> QueryExtraFields { get; set; } = new();
}
