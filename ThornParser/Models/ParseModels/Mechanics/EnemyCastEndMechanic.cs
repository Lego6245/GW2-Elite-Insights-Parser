﻿using ThornParser.Controllers;
using ThornParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ThornParser.Models.ParseModels
{

    public class EnemyCastEndMechanic : Mechanic
    {

        public EnemyCastEndMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions, rule)
        {
        }

        public EnemyCastEndMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, conditions, rule)
        {
            IsEnemyMechanic = true;
        }

        public EnemyCastEndMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public EnemyCastEndMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            MechanicData mechData = log.MechanicData;
            CombatData combatData = log.CombatData;
            HashSet<ushort> playersIds = log.PlayerIDs;
            foreach (CombatItem c in log.CombatData.GetCastDataById(SkillId))
            {
                DummyActor amp = null;
                if (!c.IsActivation.StartCasting() && Keep(c, log))
                {
                    Target target = log.FightData.Logic.Targets.Find(x => x.InstID == c.SrcInstid && x.FirstAware <= c.Time && x.LastAware >= c.Time);
                    if (target != null)
                    {
                        amp = target;
                    }
                    else
                    {
                        AgentItem a = log.AgentData.GetAgent(c.SrcAgent, c.Time);
                        if (playersIds.Contains(a.InstID))
                        {
                            continue;
                        }
                        else if (a.MasterAgent != 0)
                        {
                            AgentItem m = log.AgentData.GetAgent(a.MasterAgent, c.Time);
                            if (playersIds.Contains(m.InstID))
                            {
                                continue;
                            }
                        }
                        if (!regroupedMobs.TryGetValue(a.ID, out amp))
                        {
                            amp = new DummyActor(a);
                            regroupedMobs.Add(a.ID, amp);
                        }
                    }
                }
                if (amp != null)
                {
                    mechData[this].Add(new MechanicLog(log.FightData.ToFightSpace(c.Time), this, amp));
                }
            }
        }
    }
}