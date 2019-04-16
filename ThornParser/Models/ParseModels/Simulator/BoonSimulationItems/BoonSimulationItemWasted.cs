﻿using ThornParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThornParser.Models.ParseModels
{
    public class BoonSimulationItemWasted : AbstractBoonSimulationItemWasted
    {

        public BoonSimulationItemWasted(AgentItem src, long waste, long time) : base(src, waste, time)
        {
        }

        public override void SetBoonDistributionItem(BoonDistribution distribs, long start, long end, long boonid, ParsedLog log)
        {
            Dictionary<AgentItem, BoonDistributionItem > distrib = GetDistrib(distribs, boonid);
            AgentItem agent = Src;
            if (distrib.TryGetValue(agent, out var toModify))
            {
                toModify.Waste += GetValue(start, end);
                distrib[agent] = toModify;
            }
            else
            {
                distrib.Add(agent, new BoonDistributionItem(
                    0,
                    0, GetValue(start, end), 0, 0, 0));
            }
        }
    }
}
