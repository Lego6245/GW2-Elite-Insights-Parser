﻿using ThornParser.Parser;
using ThornParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThornParser.Models.Logic
{
    public abstract class RaidLogic : FightLogic
    {
        protected RaidLogic(ushort triggerID) : base(triggerID)
        {
            Mode = ParseMode.Raid;
        }

        public override void SetSuccess(ParsedLog log)
        {
            HashSet<int> raidRewardsIds = new HashSet<int>
                {
                    55821,
                    60685,
                    914
                };
            CombatItem reward = log.CombatData.GetStates(ParseEnum.StateChange.Reward).FirstOrDefault(x => raidRewardsIds.Contains(x.Value));
            if (reward != null)
            {
                log.FightData.Success = true;
                log.FightData.FightEnd = reward.Time;
            }
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                TriggerID
            };
        }
    }
}
