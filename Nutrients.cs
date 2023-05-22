using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutrizione
{
    public class Nutrient
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public string ValuePer100g { get; set; }
        public string DataSource { get; set; }
        public string Procedures { get; set; }
        public string? References { get; set; }
    }
}
