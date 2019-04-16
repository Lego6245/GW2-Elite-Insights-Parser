﻿using ThornParser.Parser;
using ThornParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using static ThornParser.Parser.ParseEnum.TrashIDS;

namespace ThornParser.Models.Logic
{
    class Freezie : RaidLogic
    {
        public Freezie(ushort triggerID) : base(triggerID)
        {
            Extension = "freezie";
            IconUrl = "https://wiki.guildwars2.com/images/thumb/8/8b/Freezie.jpg/189px-Freezie.jpg";
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Freezie);
            Target heartTarget = Targets.Find(x => x.ID == (ushort)FreeziesFrozenHeart);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 895, mainTarget, true, true));
            string[] namesFreezie = new [] { "Phase 1", "Heal 1", "Phase 2", "Heal 2", "Phase 3", "Heal 3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesFreezie[i - 1];
                if (i == 1 || i == 3 || i == 5)
                {
                    phase.Targets.Add(mainTarget);
                }
                else
                {
                    phase.Targets.Add(heartTarget);
                }
            }
            return phases;
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Freezie,
                (ushort)FreeziesFrozenHeart
            };
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Freezie,
                (ushort)FreeziesFrozenHeart
            };
        }
    }
}
