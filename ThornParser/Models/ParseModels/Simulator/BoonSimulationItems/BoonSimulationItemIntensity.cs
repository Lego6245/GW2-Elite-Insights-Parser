﻿using ThornParser.Parser;
using System.Collections.Generic;
using System.Linq;
using static ThornParser.Models.ParseModels.BoonSimulator;

namespace ThornParser.Models.ParseModels
{
    public class BoonSimulationItemIntensity : BoonSimulationItem
    {
        private List<BoonSimulationItemDuration> _stacks = new List<BoonSimulationItemDuration>();
        private List<AgentItem> _sources;

        public BoonSimulationItemIntensity(List<BoonStackItem> stacks) : base(stacks[0].Start, 0)
        {
            foreach (BoonStackItem stack in stacks)
            {
                _stacks.Add(new BoonSimulationItemDuration(stack));
            }
            Duration = _stacks.Max(x => x.Duration);
            _sources = new List<AgentItem>();
            foreach (BoonSimulationItemDuration item in _stacks)
            {
                _sources.AddRange(item.GetSources());
            }
        }

        public override void SetEnd(long end)
        {
            foreach (BoonSimulationItemDuration stack in _stacks)
            {
                stack.SetEnd(end);
            }
            Duration = _stacks.Max(x => x.Duration);
        }

        public override int GetStack()
        {
            return _stacks.Count;
        }

        public override void SetBoonDistributionItem(BoonDistribution distribs, long start, long end, long boonid, ParsedLog log)
        {
            foreach (BoonSimulationItemDuration item in _stacks)
            {
                item.SetBoonDistributionItem(distribs, start, end, boonid, log);
            }
        }

        public override List<AgentItem> GetSources()
        {
            return _sources;
        }
    }
}
