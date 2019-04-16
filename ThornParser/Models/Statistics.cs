﻿using System.Collections.Generic;
using System.Linq;
using ThornParser.Models.ParseModels;
using ThornParser.Parser;

namespace ThornParser.Models
{
    /// <summary>
    /// Passes statistical information about dps logs
    /// </summary>
    public class Statistics
    {
        public Statistics(ParsedLog log)
        {
            SetPresentBoons(log.CombatData.GetSkills(), log.PlayerList, log.CombatData);
            SetStackCenterPositions(log.FightData.Logic.CanCombatReplay, log.PlayerList);
        }

        public class FinalDPS
        {
            // Total
            public int Dps;
            public int Damage;
            public int CondiDps;
            public int CondiDamage;
            public int PowerDps;
            public int PowerDamage;
            // Actor only
            public int ActorDps;
            public int ActorDamage;
            public int ActorCondiDps;
            public int ActorCondiDamage;
            public int ActorPowerDps;
            public int ActorPowerDamage;
        }

        public class FinalStats
        {
            public int DirectDamageCount;
            public int CritableDirectDamageCount;
            public int CriticalRate;
            public int CriticalDmg;
            public int FlankingRate;
            public int GlanceRate;
            public int Missed;
            public int Interrupts;
            public int Invulned;
        }

        public class FinalStatsAll : FinalStats
        {
            // Rates
            public int Wasted;
            public double TimeWasted;
            public int Saved;
            public double TimeSaved;
            public double StackDist;

            // boons
            public double AvgBoons;
            public double AvgConditions;

            // Counts
            public int SwapCount;
        }

        public class FinalDefenses
        {
            //public long allHealReceived;
            public long DamageTaken;
            public int BlockedCount;
            public int EvadedCount;
            public int DodgeCount;
            public int InvulnedCount;
            public int DamageInvulned;
            public int DamageBarrier;
            public int InterruptedCount;
            public int DownCount;
            public int DownDuration;
            public int DeadCount;
            public int DeadDuration;
            public int DcCount;
            public int DcDuration;
        }

        public class FinalSupport
        {
            //public long allHeal;
            public int Resurrects;
            public double ResurrectTime;
            public int CondiCleanse;
            public double CondiCleanseTime;
        }

        public class FinalBuffs
        {
            public double Uptime;
            public double Generation;
            public double Overstack;
            public double Wasted;
            public double UnknownExtended;
            public double ByExtension;
            public double Extended;
            public double Presence;
        }

        public enum BuffEnum { Self, Group, OffGroup, Squad};

        public class FinalTargetBuffs
        {
            public FinalTargetBuffs(List<Player> plist)
            {
                Uptime = 0;
                Presence = 0;
                Generated = new Dictionary<Player, double>();
                Overstacked = new Dictionary<Player, double>();
                Wasted = new Dictionary<Player, double>();
                UnknownExtension = new Dictionary<Player, double>();
                Extension = new Dictionary<Player, double>();
                Extended = new Dictionary<Player, double>();
                foreach (Player p in plist)
                {
                    Generated.Add(p, 0);
                    Overstacked.Add(p, 0);
                    Wasted.Add(p, 0);
                    UnknownExtension.Add(p, 0);
                    Extension.Add(p, 0);
                    Extended.Add(p, 0);
                }
            }

            public double Uptime;
            public double Presence;
            public readonly Dictionary<Player, double> Generated;
            public readonly Dictionary<Player, double> Overstacked;
            public readonly Dictionary<Player, double> Wasted;
            public readonly Dictionary<Player, double> UnknownExtension;
            public readonly Dictionary<Player, double> Extension;
            public readonly Dictionary<Player, double> Extended;
        }

        public class DamageModifierData
        {
            public int HitCount { get; }
            public int TotalHitCount { get; }
            public double DamageGain { get; }
            public int TotalDamage { get; }

            public DamageModifierData(int hitCount, int totalHitCount, double damageGain, int totalDamage)
            {
                HitCount = hitCount;
                TotalHitCount = totalHitCount;
                DamageGain = damageGain;
                TotalDamage = totalDamage;
            }
        }


        public class Consumable
        {
            public Boon Buff { get; }
            public long Time { get; }
            public int Duration { get; }
            public int Stack { get; set; }

            public Consumable(Boon item, long time, int duration)
            {
                Buff = item;
                Time = time;
                Duration = duration;
                Stack = 1;
            }
        }

        public class DeathRecap
        {
            public class DeathRecapDamageItem
            {
                public long ID;
                public bool IndirectDamage;
                public string Src;
                public int Damage;
                public int Time;
            }

            public int DeathTime;
            public List<DeathRecapDamageItem> ToDown;
            public List<DeathRecapDamageItem> ToKill;
        }

        // present buff
        public readonly List<Boon> PresentBoons = new List<Boon>();//Used only for Boon tables
        public readonly List<Boon> PresentConditions = new List<Boon>();//Used only for Condition tables
        public readonly List<Boon> PresentOffbuffs = new List<Boon>();//Used only for Off Buff tables
        public readonly List<Boon> PresentDefbuffs = new List<Boon>();//Used only for Def Buff tables
        public readonly Dictionary<ushort, HashSet<Boon>> PresentPersonalBuffs = new Dictionary<ushort, HashSet<Boon>>();

        //Positions for group
        public List<Point3D> StackCenterPositions;

        private void SetStackCenterPositions(bool canCombatReplay, List<Player> players)
        {
            if (Properties.Settings.Default.ParseCombatReplay && canCombatReplay)
            {
                StackCenterPositions = new List<Point3D>();
                List<List<Point3D>> GroupsPosList = new List<List<Point3D>>();
                foreach (Player player in players)
                {
                    if (player.Account == ":Conjured Sword")
                    {
                        continue;
                    }
                    GroupsPosList.Add(player.CombatReplay.GetActivePositions());
                }
                for (int time = 0; time < GroupsPosList[0].Count; time++)
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    int activePlayers = GroupsPosList.Count;
                    foreach (List<Point3D> points in GroupsPosList)
                    {
                        Point3D point = points[time];
                        if (point != null)
                        {
                            x += point.X;
                            y += point.Y;
                            z += point.Z;
                        }
                        else
                        {
                            activePlayers--;
                        }

                    }
                    x = x / activePlayers;
                    y = y / activePlayers;
                    z = z / activePlayers;
                    StackCenterPositions.Add(new Point3D(x, y, z, GeneralHelper.PollingRate * time));
                }
            }
        }

        /// <summary>
        /// Checks the combat data and gets buffs that were present during the fight
        /// </summary>
        private void SetPresentBoons(HashSet<long> skillIDs, List<Player> players, CombatData combatData)
        {
            // Main boons
            foreach (Boon boon in Boon.GetBoonList())
            {
                if (skillIDs.Contains(boon.ID))
                {
                    PresentBoons.Add(boon);
                }
            }
            // Main Conditions
            foreach (Boon boon in Boon.GetCondiBoonList())
            {
                if (skillIDs.Contains(boon.ID))
                {
                    PresentConditions.Add(boon);
                }
            }

            // Important class specific boons
            foreach (Boon boon in Boon.GetOffensiveTableList())
            {
                if (skillIDs.Contains(boon.ID))
                {
                    PresentOffbuffs.Add(boon);
                }
            }

            foreach (Boon boon in Boon.GetDefensiveTableList())
            {
                if (skillIDs.Contains(boon.ID))
                {
                    PresentDefbuffs.Add(boon);
                }

            }

            // All class specific boons
            Dictionary<long, Boon> remainingBuffsByIds = Boon.GetRemainingBuffsList().GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList().FirstOrDefault());
            foreach (Player player in players)
            {
                PresentPersonalBuffs[player.InstID] = new HashSet<Boon>();
                foreach (CombatItem item in combatData.GetBoonDataByDst(player.InstID, player.FirstAware, player.LastAware))
                {
                    if (item.DstInstid == player.InstID && item.IsBuffRemove == ParseEnum.BuffRemove.None && remainingBuffsByIds.TryGetValue(item.SkillID, out Boon boon))
                    {
                        PresentPersonalBuffs[player.InstID].Add(boon);
                    }
                }
            }
        }
    }
}
