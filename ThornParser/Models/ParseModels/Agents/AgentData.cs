﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ThornParser.Models.ParseModels
{
    public class AgentData
    {
        private readonly List<AgentItem> _allAgentsList;
        private Dictionary<ulong, List<AgentItem>> _allAgentsByAgent;
        private Dictionary<ushort, List<AgentItem>> _allAgentsByInstID;
        private Dictionary<ushort, List<AgentItem>> _allAgentsByID;
        private Dictionary<AgentItem.AgentType, List<AgentItem>> _allAgentsByType;
        private Dictionary<string, List<AgentItem>> _allAgentsByName;
        public HashSet<ulong> AgentValues => new HashSet<ulong>(_allAgentsList.Select(x => x.Agent));
        public HashSet<ushort> InstIDValues => new HashSet<ushort>(_allAgentsList.Select(x => x.InstID));

        public AgentData(List<AgentItem> allAgentsList)
        {
            _allAgentsList = allAgentsList;
            Refresh();
        }

        public AgentItem AddCustomAgent(long start, long end, AgentItem.AgentType type, string name, string prof, ushort ID)
        {
            Random rnd = new Random();
            ulong agentValue = 0;
            while (AgentValues.Contains(agentValue) || agentValue == 0)
            {
                agentValue = (ulong)rnd.Next(Int32.MaxValue / 2, Int32.MaxValue);
            }
            ushort instID = 0;
            while (InstIDValues.Contains(instID) || instID == 0)
            {
                instID = (ushort)rnd.Next(ushort.MaxValue / 2, ushort.MaxValue);
            }
            AgentItem agent = new AgentItem(agentValue, name, prof, ID, type, 0, 0, 0, 0, 20, 20)
            {
                InstID = instID,
                LastAware = end,
                FirstAware = start,
                MasterAgent = 0
            };
            _allAgentsList.Add(agent);
            Refresh();
            return agent;
        }

        public void RemoveAgent(AgentItem agent)
        {
            _allAgentsList.Remove(agent);
            Refresh();
        }

        public AgentItem GetAgent(ulong agentAddress, long time)
        {
            if (agentAddress != 0)
            {
                if (_allAgentsByAgent.TryGetValue(agentAddress, out List<AgentItem> aList))
                {
                    AgentItem a = aList.Find(x => x.FirstAware <= time && x.LastAware >= time);
                    if (a != null)
                    {
                        return a;
                    }
                }
            }
            return GeneralHelper.UnknownAgent;
        }

        public List<AgentItem> GetAgentsByID(ushort id)
        {
            if (id != 0)
            {
                if (_allAgentsByID.TryGetValue(id, out var list))
                {
                    return list;
                }
            }
            return new List<AgentItem>();
        }

        public List<AgentItem> GetAgentByInstID(ushort instid)
        {
            if (instid != 0)
            {
                if (_allAgentsByInstID.TryGetValue(instid, out var list))
                {
                    return list;
                }
            }
            return new List<AgentItem>();
        }

        public AgentItem GetAgentByInstID(ushort instid, long time)
        {
            if (instid != 0)
            {
                if (_allAgentsByInstID.TryGetValue(instid, out var list))
                {
                    AgentItem a = list.FirstOrDefault(x => x.FirstAware <= time && x.LastAware >= time);
                    if (a != null)
                    {
                        return a;
                    }
                    return GeneralHelper.UnknownAgent;
                }
            }
            return GeneralHelper.UnknownAgent;
        }
        
        public void OverrideID(ushort ID, AgentItem agentItem)
        {
            _allAgentsList.RemoveAll(x => x.ID == ID);
            _allAgentsList.Add(agentItem);
            Refresh();
        }

        public void Refresh()
        {
            _allAgentsByAgent = _allAgentsList.GroupBy(x => x.Agent).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByID = _allAgentsList.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByInstID = _allAgentsList.GroupBy(x => x.InstID).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByType = _allAgentsList.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByName= _allAgentsList.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList());
        }
        
        public List<AgentItem> GetAgentByType(AgentItem.AgentType type)
        {
            if (_allAgentsByType.TryGetValue(type, out var list))
            {
                return list;
            }
            else
            {
                return new List<AgentItem>();
            }
        }
    }
}