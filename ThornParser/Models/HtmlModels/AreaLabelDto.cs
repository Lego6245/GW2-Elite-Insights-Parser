using System.ComponentModel;

namespace ThornParser.Models.HtmlModels
{
    public class AreaLabelDto
    {
        [DefaultValue(null)]
        public double Start;
        [DefaultValue(null)]
        public double End;
        public string Label;
        public bool Highlight;
    }
}
