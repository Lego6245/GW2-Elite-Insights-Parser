﻿using ThornParser.Parser;

namespace ThornParser.Models.ParseModels
{
    public class BoonExtensionLog : BoonLog
    {

        private readonly long _oldValue;

        public BoonExtensionLog(long time, long value, long oldValue, AgentItem src) : base(time, src, value)
        {
            _oldValue = oldValue;
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Extend(Value, _oldValue, Src, Time);
        }
    }
}