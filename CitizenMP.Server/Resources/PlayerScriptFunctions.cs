using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Neo.IronLua;

namespace CitizenMP.Server.Resources
{
    class PlayerScriptFunctions
    {
        [LuaMember("GetPlayers")]
        static LuaTable GetPlayers()
        {
            var subClients = ClientInstances.Clients.Where(c => c.Value.NetChannel != null).Select(c => c.Value.NetID).ToArray();

            var table = new LuaTable();
            var i = 1;

            foreach (var client in subClients)
            {
                table[i] = client;

                i++;
            }

            return table;
        }

        [LuaMember("GetPlayerName")]
        static string GetPlayerName(int source)
        {
            var player = FindPlayer(source);

            if (player != null)
            {
                return player.Name;
            }

            return null;
        }

        [LuaMember("GetPlayerGuid")]
        static string GetPlayerGuid(int source)
        {
            var player = FindPlayer(source);

            if (player != null)
            {
                var identifier = player.Identifiers.FirstOrDefault();

                player.Log().Warn("Using GetPlayerGuid is deprecated - with NPv2 authentication a client can have multiple identifiers. Please use a 'in'-style match on GetPlayerIdentifiers()'s result instead.");

                if (identifier == null)
                {
                    identifier = player.Guid.PadLeft(16, '0');
                }

                return identifier;
            }

            return null;
        }

        [LuaMember("GetPlayerIdentifiers")]
        static LuaTable GetPlayerIdentifiers(int source)
        {
            var player = FindPlayer(source);

            if (player != null)
            {
                var table = new LuaTable();
                var i = 1;

                foreach (var identifier in player.Identifiers)
                {
                    table[i] = identifier;

                    i++;
                }

                return table;
            }

            return null;
        }

        [LuaMember("GetPlayerPing")]
        static int GetPlayerPing(int source)
        {
            var player = FindPlayer(source);

            if (player != null)
            {
                return player.Ping;
            }

            return -1;
        }

        [LuaMember("GetPlayerEP")]
        static string GetPlayerEP(int source)
        {
            var player = FindPlayer(source);

            if (player != null)
            {
                return player.RemoteEP.ToString();
            }

            return null;
        }

        [LuaMember("GetPlayerLastMsg")]
        static double GetPlayerLastMsg_f(int source)
        {
            var player = FindPlayer(source);

            if (player != null)
            {
                return Time.CurrentTime - player.LastSeen;
            }

            return 99999999;
        }

        [LuaMember("GetHostId")]
        static int GetHostId()
        {
            return ScriptEnvironment.CurrentEnvironment.Resource.Manager.GameServer.GetHostID();
        }

        [LuaMember("DropPlayer")]
        static void DropPlayer(int source, string reason)
        {
            var player = FindPlayer(source);

            if (player != null)
            {
                ScriptEnvironment.CurrentEnvironment.Resource.Manager.GameServer.DropClient(player, reason);
            }
        }

        // similar semantics to IW tempBanClient - bans are not persisted, and last less than an hour, mainly to deter from instant rejoining
        // bans are applied on identifiers, as well
        [LuaMember("TempBanPlayer")]
        static void TempBanPlayer(int source, string reason)
        {
            var player = FindPlayer(source);

            if (player != null)
            {
                var gameServer = ScriptEnvironment.CurrentEnvironment.Resource.Manager.GameServer;
                gameServer.DropClient(player, reason);

                foreach (var identifier in player.Identifiers)
                {
                    gameServer.BanIdentifier(identifier, reason);
                }
           }
        }

        static Client FindPlayer(int source)
        {
            return ClientInstances.Clients.Where(a => a.Value.NetID == source).Select(a => a.Value).FirstOrDefault();
        }
    }
}
