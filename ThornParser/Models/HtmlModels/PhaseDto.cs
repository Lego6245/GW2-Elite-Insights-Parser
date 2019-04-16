﻿using ThornParser.Models.ParseModels;
using ThornParser.Parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ThornParser.Models.HtmlModels
{
    
    public class PhaseDto
    {
        public string Name;
        [DefaultValue(null)]
        public long Duration;
        [DefaultValue(null)]
        public double Start;
        [DefaultValue(null)]
        public double End;
        public List<int> Targets = new List<int>();

        public List<List<object>> DpsStats;
        public List<List<List<object>>> DpsStatsTargets;
        public List<List<List<object>>> DmgStatsTargets;
        public List<List<object>> DmgStats;
        public List<List<object>> DefStats;
        public List<List<object>> HealStats;

        public List<BoonData> BoonStats;
        public List<BoonData> BoonGenSelfStats;
        public List<BoonData> BoonGenGroupStats;
        public List<BoonData> BoonGenOGroupStats;
        public List<BoonData> BoonGenSquadStats;

        public List<BoonData> OffBuffStats;
        public List<BoonData> OffBuffGenSelfStats;
        public List<BoonData> OffBuffGenGroupStats;
        public List<BoonData> OffBuffGenOGroupStats;
        public List<BoonData> OffBuffGenSquadStats;

        public List<BoonData> DefBuffStats;
        public List<BoonData> DefBuffGenSelfStats;
        public List<BoonData> DefBuffGenGroupStats;
        public List<BoonData> DefBuffGenOGroupStats;
        public List<BoonData> DefBuffGenSquadStats;

        public List<BoonData> PersBuffStats;

        public List<DamageModData> DmgModifiersCommon;
        public List<DamageModData> DmgModifiersItem;
        public List<DamageModData> DmgModifiersPers;

        public List<List<BoonData>> TargetsCondiStats;
        public List<BoonData> TargetsCondiTotals;
        public List<BoonData> TargetsBoonTotals;

        public List<List<int[]>> MechanicStats;
        public List<List<int[]>> EnemyMechanicStats;

        public List<double> MarkupLines;
        public List<AreaLabelDto> MarkupAreas;
        public List<int> SubPhases;
        
        public PhaseDto(PhaseData phaseData, List<PhaseData> phases, ParsedLog log)
        {
            Name = phaseData.Name;
            Duration = phaseData.DurationInMS;
            Start = phaseData.Start / 1000.0;
            End = phaseData.End / 1000.0;
            foreach (Target target in phaseData.Targets)
            {
                Targets.Add(log.FightData.Logic.Targets.IndexOf(target));
            }
            // add phase markup
            MarkupLines = new List<double>();
            MarkupAreas = new List<AreaLabelDto>();
            for (int j = 1; j < phases.Count; j++)
            {
                PhaseData curPhase = phases[j];
                if (curPhase.Start < phaseData.Start || curPhase.End > phaseData.End ||
                    (curPhase.Start == phaseData.Start && curPhase.End == phaseData.End))
                {
                    continue;
                }
                if (SubPhases == null)
                {
                    SubPhases = new List<int>();
                }
                SubPhases.Add(j);
                long start = curPhase.Start - phaseData.Start;
                long end = curPhase.End - phaseData.Start;
                if (curPhase.DrawStart) MarkupLines.Add(start / 1000.0);
                if (curPhase.DrawEnd) MarkupLines.Add(end / 1000.0);
                AreaLabelDto phaseArea = new AreaLabelDto
                {
                    Start = start / 1000.0,
                    End = end / 1000.0,
                    Label = curPhase.Name,
                    Highlight = curPhase.DrawArea
                };
                MarkupAreas.Add(phaseArea);
            }
            if (MarkupAreas.Count == 0) MarkupAreas = null;
            if (MarkupLines.Count == 0) MarkupLines = null;
        }


        // helper methods

        public static List<object> GetDMGStatData(Statistics.FinalStatsAll stats)
        {
            List<object> data = GetDMGTargetStatData(stats);
            data.AddRange(new List<object>
                {
                    // commons
                    stats.TimeWasted, // 9
                    stats.Wasted, // 10

                    stats.TimeSaved, // 11
                    stats.Saved, // 12

                    stats.SwapCount, // 13
                    Math.Round(stats.StackDist, 2) // 14
                });
            return data;
        }

        public static List<object> GetDMGTargetStatData(Statistics.FinalStats stats)
        {
            List<object> data = new List<object>
                {
                    stats.DirectDamageCount, // 0
                    stats.CritableDirectDamageCount, // 1
                    stats.CriticalRate, // 2
                    stats.CriticalDmg, // 3

                    stats.FlankingRate, // 4

                    stats.GlanceRate, // 5

                    stats.Missed,// 6
                    stats.Interrupts, // 7
                    stats.Invulned // 8
                };
            return data;
        }

        public static List<object> GetDPSStatData(Statistics.FinalDPS dpsAll)
        {
            List<object> data = new List<object>
                {
                    dpsAll.Damage,
                    dpsAll.Dps,
                    dpsAll.PowerDamage,
                    dpsAll.PowerDps,
                    dpsAll.CondiDamage,
                    dpsAll.CondiDps
                };
            return data;
        }

        public static List<object> GetSupportStatData(Statistics.FinalSupport support)
        {
            List<object> data = new List<object>()
                {
                    support.CondiCleanse,
                    support.CondiCleanseTime,
                    support.Resurrects,
                    support.ResurrectTime
                };
            return data;
        }

        public static List<object> GetDefenseStatData(Statistics.FinalDefenses defenses, PhaseData phase)
        {
            List<object> data = new List<object>
                {
                    defenses.DamageTaken,
                    defenses.DamageBarrier,
                    defenses.BlockedCount,
                    defenses.InvulnedCount,
                    defenses.InterruptedCount,
                    defenses.EvadedCount,
                    defenses.DodgeCount
                };

            if (defenses.DownDuration > 0)
            {
                TimeSpan downDuration = TimeSpan.FromMilliseconds(defenses.DownDuration);
                data.Add(defenses.DownCount);
                data.Add(downDuration.TotalSeconds + " seconds downed, " + Math.Round((downDuration.TotalMilliseconds / phase.DurationInMS) * 100, 1) + "% Downed");
            }
            else
            {
                data.Add(0);
                data.Add("0% downed");
            }

            if (defenses.DeadDuration > 0)
            {
                TimeSpan deathDuration = TimeSpan.FromMilliseconds(defenses.DeadDuration);
                data.Add(defenses.DeadCount);
                data.Add(deathDuration.TotalSeconds + " seconds dead, " + (100.0 - Math.Round((deathDuration.TotalMilliseconds / phase.DurationInMS) * 100, 1)) + "% Alive");
            }
            else
            {
                data.Add(0);
                data.Add("100% Alive");
            }
            return data;
        }
    }
}
