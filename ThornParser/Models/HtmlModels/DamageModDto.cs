﻿using ThornParser.Models.ParseModels;
using System.Collections.Generic;

namespace ThornParser.Models.HtmlModels
{ 
    public class DamageModDto
    {    
        public long Id;       
        public string Name;       
        public string Icon;
        public string Tooltip;
        public bool NonMultiplier;

        public static List<DamageModDto> AssembleDamageModifiers(ICollection<DamageModifier> damageMods)
        {
            List<DamageModDto> dtos = new List<DamageModDto>();
            foreach (DamageModifier mod in damageMods)
            {
                dtos.Add(new DamageModDto()
                {
                    Id = mod.Name.GetHashCode(),
                    Name = mod.Name,
                    Icon = mod.Url,
                    Tooltip = mod.Tooltip,
                    NonMultiplier = !mod.Multiplier
                });
            }
            return dtos;
        }
    }
}
