import { RconClient } from "rconify";
import { config } from "./config";

Bun.serve({
  port: 3000,
  async fetch(req) {
    const url = new URL(req.url);
    if (url.pathname !== "/adv_query") {
      return new Response("Not found", { status: 404 });
    }

    const client = new RconClient({
      host: config.host as string,
      port: config.port as number,
      password: config.password as string,
      ignoreInvalidAuthResponse: true,
    });

    try {
      await client.connect();
      const response = await client.sendCommand("adv_query");
      client.disconnect();

      const trimmed = response.trim();
      const json = JSON.parse(trimmed);
      return Response.json(json);
    } catch (err) {
      client.disconnect();
      return Response.json(
        { error: err instanceof Error ? err.message : "Unknown error" },
        { status: 500 },
      );
    }
  },
});

console.log("API RCON démarrée sur http://localhost:3000");
console.log(
  "GET http://localhost:3000/adv_query pour récupérer les infos serveur",
);
