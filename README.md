# CSS-Panel (Query) – Plugin CS2

Plugin minimal pour [CSS-Panel](https://github.com/CSSPanel/Panel). Il expose une seule commande **`css_query`** qui envoie en JSON dans la console la liste des joueurs et les infos du serveur.

## Fonctionnalité

- **`css_query`** (SERVER_ONLY, `@css/root`)  
  Affiche dans la console serveur un JSON avec :
  - **server** : `map`, `p` (nombre de joueurs), `mP` (maxPlayers), `serverIp`, `address` (ip:port), `pr` (version plugin), `maps` (liste des maps)
  - **players** : tableau d’objets avec `id`, `s64` (Steam64), `t` (team), `k` (kills), `d` (deaths), `s` (score)

Le panel peut appeler cette commande en RCON et parser la sortie pour afficher les serveurs et joueurs.

## Prérequis

- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/) (testé sur v247)
- [CSS-Panel](https://github.com/CSSPanel/Panel)

## Configuration

Au premier lancement, un fichier de configuration est créé ici :  
`addons/counterstrikesharp/configs/plugins/CSS-Panel/CSS-Panel.json`

Il contient pour l’instant uniquement `ConfigVersion`. Tu peux y ajouter plus tard d’autres options si tu étends le plugin.

## Installation

1. Compiler le projet (ou utiliser les binaires fournis).
2. Copier `CSS-Panel.dll` dans `game/csgo/addons/counterstrikesharp/plugins`.
3. Redémarrer le serveur ou recharger les plugins.

## Licence

Voir [LICENSE](LICENSE).
