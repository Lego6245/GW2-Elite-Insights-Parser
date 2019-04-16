using ThornParser.Parser;

namespace ThornParser.Models.ParseModels
{
    public class BoonApplicationLog : BoonLog
    {
        public BoonApplicationLog(long time, AgentItem src, long value) : base(time,src,value)
        {
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Add(Value, Src, Time);
        }
    }
}