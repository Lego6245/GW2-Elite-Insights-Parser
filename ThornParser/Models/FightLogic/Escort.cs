﻿using ThornParser.Parser;
using ThornParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ThornParser.Models.Logic
{
    public class Escort : RaidLogic
    {
        public Escort(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            }
            );
            Extension = "escort";
            IconUrl = "https://wiki.guildwars2.com/images/b/b5/Mini_McLeod_the_Silent.png";
        }

        /*protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/RZbs21b.png",
                            (1099, 1114),
                            (-5467, 8069, -2282, 11297),
                            (-12288, -27648, 12288, 27648),
                            (1920, 12160, 2944, 14464));
        }*/

        public override string GetFightName() {
            return "Escort";
        }
    }
}
