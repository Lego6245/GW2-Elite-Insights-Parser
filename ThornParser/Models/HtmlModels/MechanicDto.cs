﻿using ThornParser.Models.ParseModels;
using ThornParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ThornParser.Models.HtmlModels
{  
    public class MechanicDto
    {       
        public string Name;       
        public string ShortName;        
        public string Description;       
        public bool EnemyMech;       
        public bool PlayerMech;

        public static List<int[]> GetMechanicData(HashSet<Mechanic> presMech, ParsedLog log, DummyActor actor, PhaseData phase)
        {
            List<int[]> res = new List<int[]>();

            foreach (Mechanic mech in presMech)
            {
                long timeFilter = 0;
                int filterCount = 0;
                List<MechanicLog> mls = log.MechanicData[mech].Where(x => x.Actor.InstID == actor.InstID && phase.InInterval(x.Time)).ToList();
                int count = mls.Count;
                foreach (MechanicLog ml in mls)
                {
                    if (mech.InternalCooldown != 0 && ml.Time - timeFilter < mech.InternalCooldown)//ICD check
                    {
                        filterCount++;
                    }
                    timeFilter = ml.Time;

                }
                res.Add(new int[] { count - filterCount, count });
            }
            return res;
        }
    }
}
