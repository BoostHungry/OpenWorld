﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenWorldServer
{
    public static class WorldUtils
    {
        public static void AddSettlement(ServerClient? client, string tileID, string username)
        {
            if (client != null)
            {
                client.homeTileID = tileID;

                foreach(ServerClient sc in Server.savedClients)
                {
                    if (sc.username == client.username)
                    {
                        sc.homeTileID = client.homeTileID;
                        break;
                    }
                }

                PlayerUtils.SavePlayer(client);
            }

            int factionValue = 0;
            foreach (ServerClient sc in Networking.connectedClients)
            {
                if (sc.username == client.username) continue;
                else
                {
                    if (client.faction == null) factionValue = 0;
                    if (sc.faction == null) factionValue = 0;
                    else if (client.faction != null && sc.faction != null)
                    {
                        if (client.faction.name == sc.faction.name) factionValue = 1;
                        else factionValue = 2;
                    }
                }

                string dataString = "SettlementBuilder│AddSettlement│" + tileID + "│" + username + "│" + factionValue;
                Networking.SendData(sc, dataString);
            }

            Server.savedSettlements.Add(client.homeTileID, new List<string> { client.username });

            ConsoleUtils.LogToConsole("Settlement With ID [" + tileID + "] And Owner [" + username + "] Has Been Added");
        }

        public static void RemoveSettlement(ServerClient? client, string tile)
        {
            if (client != null)
            {
                client.homeTileID = null;

                foreach (ServerClient sc in Server.savedClients)
                {
                    if (sc.username == client.username)
                    {
                        sc.homeTileID = null;
                        break;
                    }
                }

                PlayerUtils.SavePlayer(client);
            }

            if (!string.IsNullOrWhiteSpace(tile))
            {
                string dataString = "SettlementBuilder│RemoveSettlement│" + tile;

                foreach (ServerClient sc in Networking.connectedClients)
                {
                    if (client != null)
                    {
                        if (sc.username == client.username) continue;
                    }

                    Networking.SendData(sc, dataString);
                }

                Server.savedSettlements.Remove(tile);

                ConsoleUtils.LogToConsole("Settlement With ID [" + tile + "] Has Been Deleted");
            }
        }

        public static void CheckForTileDisponibility(ServerClient client, string tileID)
        {
            foreach (ServerClient savedClient in Server.savedClients)
            {
                if (savedClient.username == client.username)
                {
                    if (savedClient.homeTileID == tileID) return;

                    else
                    {
                        foreach (KeyValuePair<string, List<string>> pair in Server.savedSettlements)
                        {
                            if (pair.Value[0] == client.username)
                            {
                                RemoveSettlement(client, pair.Key);
                                Thread.Sleep(500);
                                break;
                            }
                        }

                        break;
                    }
                }

                else
                {
                    if (savedClient.homeTileID == tileID)
                    {
                        Networking.SendData(client, "Disconnect│Corrupted");

                        Console.ForegroundColor = ConsoleColor.Red;
                        ConsoleUtils.LogToConsole("Player [" + client.username + "] Tried To Claim Used Tile! [" + tileID + "]");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }

            AddSettlement(client, tileID, client.username);
        }
    }
}
