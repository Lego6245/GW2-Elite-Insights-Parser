using System.Collections.Generic;

namespace ThornParser.Models.HtmlModels
{  
    public class PhaseChartDataDto
    {    
        public List<PlayerChartDataDto> Players = new List<PlayerChartDataDto>();      
        public List<TargetChartDataDto> Targets = new List<TargetChartDataDto>();
    }
}
