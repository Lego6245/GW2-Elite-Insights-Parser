﻿using ThornParser.Parser;
using ThornParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ThornParser.Models.Logic
{
    public abstract class FightLogic
    {

        public enum ParseMode { Raid, Fractal, Golem, WvW, Unknown };

        private CombatReplayMap _map;
        public readonly List<Mechanic> MechanicList; //Resurrects (start), Resurrect
        public ParseMode Mode { get; protected set; } = ParseMode.Unknown;
        public bool CanCombatReplay { get; set; } = false;
        public string Extension { get; protected set; }
        public string IconUrl { get; protected set; }
        public List<Mob> TrashMobs { get; } = new List<Mob>();
        public List<Target> Targets { get; } = new List<Target>();
        protected readonly ushort TriggerID;

        protected FightLogic(ushort triggerID)
        {
            TriggerID = triggerID;
            CanCombatReplay = GetCombatMap() != null;
            MechanicList = new List<Mechanic>() {
                new PlayerStatusMechanic(SkillItem.DeathId, "Dead", new MechanicPlotlySetting("x","rgb(0,0,0)"), "Dead",0),
                new PlayerStatusMechanic(SkillItem.DownId, "Downed", new MechanicPlotlySetting("cross","rgb(255,0,0)"), "Downed",0),
                new PlayerStatusMechanic(SkillItem.ResurrectId, "Resurrect", new MechanicPlotlySetting("cross-open","rgb(0,255,255)"), "Res",0)
            };
        }

        protected virtual CombatReplayMap GetCombatMapInternal()
        {
            return null;
        }

        public CombatReplayMap GetCombatMap()
        {
            if (_map == null)
            {
                _map = GetCombatMapInternal();
            }
            return _map;
        }

        protected virtual List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                TriggerID
            };
        }

        public virtual string GetFightName()
        {
            Target target = Targets.Find(x => x.ID == TriggerID);
            if (target == null)
            {
                return "UNKNOWN";
            }
            return target.Character;
        }

        private void RegroupTargetsByID(ushort id, AgentData agentData, List<CombatItem> combatItems)
        {
            List<AgentItem> agents = agentData.GetAgentsByID(id);
            List<Target> toRegroup = Targets.Where(x => x.ID == id).ToList();
            if (agents.Count > 1 && toRegroup.Count > 1)
            {
                Targets.RemoveAll(x => x.ID == id);
                AgentItem firstItem = agents.First();
                HashSet<ulong> agentValues = new HashSet<ulong>(agents.Select(x => x.Agent));
                AgentItem newTargetAgent = new AgentItem(firstItem)
                {
                    FirstAware = agents.Min(x => x.FirstAware),
                    LastAware = agents.Max(x => x.LastAware)
                };
                // get unique id for the fusion
                ushort instID = 0;
                Random rnd = new Random();
                while (agentData.InstIDValues.Contains(instID) || instID == 0)
                {
                    instID = (ushort)rnd.Next(ushort.MaxValue / 2, ushort.MaxValue);
                }
                newTargetAgent.InstID = instID;
                agentData.OverrideID(id, newTargetAgent);
                Targets.Add(new Target(newTargetAgent));
                foreach (CombatItem c in combatItems)
                {
                    if (agentValues.Contains(c.SrcAgent))
                    {
                        c.OverrideSrcValues(newTargetAgent.Agent, newTargetAgent.InstID);
                    }
                    if (agentValues.Contains(c.DstAgent))
                    {
                        c.OverrideDstValues(newTargetAgent.Agent, newTargetAgent.InstID);
                    }
                }
            }
        }

        protected abstract HashSet<ushort> GetUniqueTargetIDs();

        public void ComputeFightTargets(AgentData agentData, FightData fightData, List<CombatItem> combatItems)
        {
            List<ushort> ids = GetFightTargetsIDs();
            foreach (ushort id in ids)
            {
                List<AgentItem> agents = agentData.GetAgentsByID(id);
                foreach (AgentItem agentItem in agents)
                {
                    Targets.Add(new Target(agentItem));
                }
            }
            foreach (ushort id in GetUniqueTargetIDs())
            {
                RegroupTargetsByID(id, agentData, combatItems);
            }
        }

        public void SetMaxHealth(ushort instid, long time, int health)
        {
            foreach (Target target in Targets)
            {
                if (target.Health == -1 && target.InstID == instid && target.FirstAware <= time && target.LastAware >= time)
                {
                    target.Health = health;
                    break;
                }
            }
        }

        protected void OverrideMaxHealths(ParsedLog log)
        {
            List<CombatItem> maxHUs = log.CombatData.GetStates(ParseEnum.StateChange.MaxHealthUpdate);
            if (maxHUs.Count > 0)
            {
                foreach (Target tar in Targets)
                {
                    List<CombatItem> subList = maxHUs.Where(x => x.SrcInstid == tar.InstID && x.Time >= tar.FirstAware && x.Time <= tar.LastAware).ToList();
                    if (subList.Count > 0)
                    {
                        tar.Health = subList.Max(x => (int)x.DstAgent);
                    }
                }
            }
        }

        public virtual void AddHealthUpdate(ushort instid, long time, long healthTime, int health)
        {
            foreach (Target target in Targets)
            {
                if (target.InstID == instid && target.FirstAware <= time && target.LastAware >= time)
                {
                    target.HealthOverTime.Add((healthTime, health));
                    break;
                }
            }
        }

        protected List<PhaseData> GetPhasesByInvul(ParsedLog log, long skillID, Target mainTarget, bool addSkipPhases, bool beginWithStart)
        {
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = new List<PhaseData>();
            long last = 0;
            List<CombatItem> invuls = GetFilteredList(log, skillID, mainTarget, beginWithStart);
            for (int i = 0; i < invuls.Count; i++)
            {
                CombatItem c = invuls[i];
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    long end = log.FightData.ToFightSpace(c.Time);
                    phases.Add(new PhaseData(last, end));
                    /*if (i == invuls.Count - 1)
                    {
                        mainTarget.AddCustomCastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None, log);
                    }*/
                    last = end;
                }
                else
                {
                    long end = log.FightData.ToFightSpace(c.Time);
                    if (addSkipPhases)
                    {
                        phases.Add(new PhaseData(last, end));
                    }
                    //mainTarget.AddCustomCastLog(last, -5, (int)(end - last), ParseEnum.Activation.None, (int)(end - last), ParseEnum.Activation.None, log);
                    last = end;
                }
            }
            if (fightDuration - last > 5000)
            {
                phases.Add(new PhaseData(last, fightDuration));
            }
            return phases;
        }

        protected List<PhaseData> GetInitialPhase(ParsedLog log)
        {
            List<PhaseData> phases = new List<PhaseData>();
            long fightDuration = log.FightData.FightDuration;
            phases.Add(new PhaseData(0, fightDuration));
            phases[0].Name = "Full Fight";
            return phases;
        }

        public virtual List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == TriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            return phases;
        }

        protected void AddTargetsToPhase(PhaseData phase, List<ushort> ids, ParsedLog log)
        {
            foreach (Target target in Targets)
            {
                if (ids.Contains(target.ID) && phase.InInterval(Math.Max(log.FightData.ToFightSpace(target.FirstAware),0)))
                {
                    phase.Targets.Add(target);
                }
            }
            phase.OverrideTimes(log);
        }

        public virtual void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
        }

        public virtual void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
        }

        public virtual void ComputeAdditionalTrashMobData(Mob mob, ParsedLog log)
        {
        }

        protected virtual List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>();
        }

        public virtual int IsCM(ParsedLog log)
        {
            return -1;
        }

        public void InitTrashMobCombatReplay(ParsedLog log, int pollingRate)
        {
            List<ParseEnum.TrashIDS> ids = GetTrashMobsIDS();
            List<AgentItem> aList = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => ids.Contains(ParseEnum.GetTrashIDS(x.ID))).ToList();
            foreach (AgentItem a in aList)
            {
                Mob mob = new Mob(a);
                mob.InitCombatReplay(log, pollingRate, true, false);
                TrashMobs.Add(mob);
            }
        }

        protected void SetSuccessByDeath(ParsedLog log, ushort idFirst, params ushort[] ids)
        {
            int success = 0;
            long maxTime = long.MinValue;
            List<ushort> idsToUse = new List<ushort>
            {
                idFirst
            };
            idsToUse.AddRange(ids);
            foreach (ushort id in idsToUse)
            {
                Target target = Targets.Find(x => x.ID == TriggerID);
                if (target == null)
                {
                    throw new InvalidOperationException("Main target of the fight not found");
                }
                CombatItem killed = log.CombatData.GetStatesData(target.InstID, ParseEnum.StateChange.ChangeDead, target.FirstAware, target.LastAware).LastOrDefault();
                if (killed != null)
                {
                    success++;
                    maxTime = Math.Max(killed.Time, maxTime);
                }
            }
            if (success == idsToUse.Count)
            {
                log.FightData.Success = true;
                log.FightData.FightEnd = maxTime;
            }
        }

        public virtual void SetSuccess(ParsedLog log)
        {
            SetSuccessByDeath(log, TriggerID);
        }


        public void ComputeMechanics(ParsedLog log)
        {
            MechanicData mechData = log.MechanicData;
            CombatData combatData = log.CombatData;
            HashSet<ushort> playersIds = log.PlayerIDs;
            Dictionary<ushort, DummyActor> regroupedMobs = new Dictionary<ushort, DummyActor>();
            foreach (Mechanic mech in MechanicList)
            {
                mech.CheckMechanic(log, regroupedMobs);
            }
            mechData.ProcessMechanics(log);
        }

        public virtual void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
        }

        //
        protected static List<CombatItem> GetFilteredList(ParsedLog log, long skillID, AbstractMasterActor target, bool beginWithStart)
        {
            bool needStart = beginWithStart;
            List<CombatItem> main = log.CombatData.GetBoonData(skillID).Where(x => ((x.DstInstid == target.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None) || (x.SrcInstid == target.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.Manual)) && x.Time >= target.FirstAware && x.Time <= target.LastAware).ToList();
            List<CombatItem> filtered = new List<CombatItem>();
            for (int i = 0; i < main.Count; i++)
            {
                CombatItem c = main[i];
                if (needStart && c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    needStart = false;
                    filtered.Add(c);
                }
                else if (!needStart && c.IsBuffRemove == ParseEnum.BuffRemove.Manual)
                {
                    // consider only last remove event before another application
                    if ((i == main.Count - 1) || (i < main.Count - 1 && main[i + 1].IsBuffRemove == ParseEnum.BuffRemove.None))
                    {
                        needStart = true;
                        filtered.Add(c);
                    }
                }
            }
            return filtered;
        }

    }
}
