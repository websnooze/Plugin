using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;

namespace CSSPanel;

[MinimumApiVersion(246)]
public partial class CSSPanel : BasePlugin, IPluginConfig<CSSPanelConfig>
{
	public override string ModuleName => "CSS-Panel (Query)";
	public override string ModuleDescription => "Expose server and player list via css_query for CSS-Panel.";
	public override string ModuleAuthor => "CSSPanel";
	public override string ModuleVersion => "2.0.0";

	public CSSPanelConfig Config { get; set; } = new();

	public override void Load(bool hotReload) { }

	public override void Unload(bool hotReload) { }

	public void OnConfigParsed(CSSPanelConfig config)
	{
		Config = config;
	}
}
