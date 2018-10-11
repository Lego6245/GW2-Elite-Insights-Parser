﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class PlayerChartDataDto
    {
        [DataMember]
        public List<List<int>> bosses;
        [DataMember]
        public List<int> total;
    }
}
