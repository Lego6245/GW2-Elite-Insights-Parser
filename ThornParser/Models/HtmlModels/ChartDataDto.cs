using System.Collections.Generic;

namespace ThornParser.Models.HtmlModels
{   
    public class ChartDataDto
    {      
        public List<PhaseChartDataDto> Phases = new List<PhaseChartDataDto>();      
        public List<MechanicChartDataDto> Mechanics = new List<MechanicChartDataDto>();
    }
}
