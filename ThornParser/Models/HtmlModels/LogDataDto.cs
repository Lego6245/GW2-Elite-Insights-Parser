﻿using System.Collections.Generic;

namespace ThornParser.Models.HtmlModels
{   
    public class LogDataDto
    { 
        public List<TargetDto> Targets = new List<TargetDto>();
        public readonly List<PlayerDto> Players = new List<PlayerDto>();
        public readonly List<EnemyDto> Enemies = new List<EnemyDto>();
        public readonly List<PhaseDto> Phases = new List<PhaseDto>();
        public readonly List<long> Boons = new List<long>();
        public readonly List<long> OffBuffs = new List<long>();
        public readonly List<long> DefBuffs = new List<long>();
        public readonly List<long> DmgModifiersItem = new List<long>();
        public readonly List<long> DmgModifiersCommon = new List<long>();
        public readonly Dictionary<string, List<long>> DmgModifiersPers = new Dictionary<string, List<long>>();
        public readonly Dictionary<string, List<long>> PersBuffs = new Dictionary<string, List<long>>();
        
        public readonly List<long> Conditions = new List<long>();
        public string EncounterDuration;
        public bool Success;
        public bool Wvw;
        public string FightName;
        public string FightIcon;
        public bool LightTheme;
        public bool NoMechanics;
        public bool SingleGroup;
    }
}
