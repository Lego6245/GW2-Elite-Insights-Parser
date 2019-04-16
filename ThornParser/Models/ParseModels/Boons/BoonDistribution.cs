﻿using System.Collections.Generic;
using System.Linq;

namespace ThornParser.Models.ParseModels
{
    public class BoonDistributionItem
    {
        public long Value { get; set; }
        public long Overstack { get; set; }
        public long Waste { get; set; }
        public long UnknownExtension { get; set; }
        public long Extension { get; set; }
        public long Extended { get; set; }

        public BoonDistributionItem(long value, long overstack, long waste, long unknownExtension, long extension, long extended)
        {
            Value = value;
            Overstack = overstack;
            Waste = waste;
            UnknownExtension = unknownExtension;
            Extension = extension;
            Extended = extended;
        }
    }

    public class BoonDistribution : Dictionary<long, Dictionary<AgentItem, BoonDistributionItem>>
    {
        public bool HasSrc(long boonid, AgentItem src)
        {
            return ContainsKey(boonid) && this[boonid].ContainsKey(src);
        }

        public long GetUptime(long boonid)
        {
            if (!ContainsKey(boonid))
            {
                return 0;
            }
            return this[boonid].Sum(x => x.Value.Value);
        }

        public long GetGeneration(long boonid, AgentItem src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Value;
        }

        public long GetOverstack(long boonid, AgentItem src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Overstack;
        }

        public long GetWaste(long boonid, AgentItem src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Waste;
        }

        public long GetUnknownExtension(long boonid, AgentItem src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].UnknownExtension;
        }

        public long GetExtension(long boonid, AgentItem src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Extension;
        }

        public long GetExtended(long boonid, AgentItem src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Extended;
        }
    }
}
