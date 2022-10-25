﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorld
{
    public static class FactionHandler
    {
        public enum MemberRank { Member, Moderator, Leader }

        public static void FactionDetailsHandle(string data)
        {
            string factionName = data.Split('│')[2];
            if (string.IsNullOrWhiteSpace(factionName))
            {
                Main._ParametersCache.hasFaction = false;
                Main._ParametersCache.factionName = "";
                Main._ParametersCache.factionMembers.Clear();
            }

            else
            {
                Main._ParametersCache.hasFaction = true;
                Main._ParametersCache.factionName = factionName;

                Main._ParametersCache.factionMembers.Clear();
                string[] factionMembers = data.Split('│')[3].Split('»');
                foreach (string str in factionMembers)
                {
                    if (string.IsNullOrWhiteSpace(str)) continue;

                    string memberName = str.Split(':')[0];
                    int memberRank = int.Parse(str.Split(':')[1]);
                    Main._ParametersCache.factionMembers.Add(memberName, memberRank);
                }
            }
        }

        public static void FactionKickedHandle()
        {
            Main._ParametersCache.hasFaction = false;
            Main._ParametersCache.factionName = "";
            Main._ParametersCache.factionMembers.Clear();
        }

        public static void FindOnlineFactionInWorld()
        {
            List<Faction> factions = Find.FactionManager.AllFactions.ToList();
            Main._ParametersCache.neutralFaction = factions.Find(fetch => fetch.Name == "Open World Settlements Neutral");
            Main._ParametersCache.allyFaction = factions.Find(fetch => fetch.Name == "Open World Settlements Ally");
            Main._ParametersCache.enemyFaction = factions.Find(fetch => fetch.Name == "Open World Settlements Enemy");

            Main._ParametersCache.allFactions.Add(Main._ParametersCache.neutralFaction);
            Main._ParametersCache.allFactions.Add(Main._ParametersCache.allyFaction);
            Main._ParametersCache.allFactions.Add(Main._ParametersCache.enemyFaction);
        }
    }
}
