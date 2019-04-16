﻿using ThornParser.Controllers;
using ThornParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ThornParser.Models.ParseModels
{
    
    public class HitOnEnemyMechanic : Mechanic
    {

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions, rule)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, conditions, rule)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            MechanicData mechData = log.MechanicData;
            CombatData combatData = log.CombatData;
            HashSet<ushort> playersIds = log.PlayerIDs;
            IEnumerable<AgentItem> agents = log.AgentData.GetAgentsByID((ushort)SkillId);
            foreach (AgentItem a in agents)
            {
                List<CombatItem> combatitems = combatData.GetDamageTakenData(a.InstID, a.FirstAware, a.LastAware);
                foreach (CombatItem c in combatitems)
                {
                    if (c.IsBuff > 0 || !c.ResultEnum.IsHit() || !Keep(c, log) )
                    {
                        continue;
                    }
                    foreach (Player p in log.PlayerList)
                    {
                        if (c.SrcInstid == p.InstID || c.SrcMasterInstid == p.InstID )
                        {
                            mechData[this].Add(new MechanicLog(log.FightData.ToFightSpace(c.Time), this, p));
                        }
                    }
                }
            }
        }
    }
}
