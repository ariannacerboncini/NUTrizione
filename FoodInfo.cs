using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutrizione
{
    public class FoodInfo
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public string FoodId { get; set; }
        public string ScientificName { get; set; }
        public string EnglishName { get; set; }
        public string Informations { get; set; }
        public string NumberOfSamples { get; set; }
        public string EdiblePart { get; set; }
        public string Portion { get; set; }
        public string Protein { get; set; }
        public string Fat { get; set; }
        public string Carbohydrate { get; set; }
        public string Fiber { get; set; }
        public string Alcohol { get; set; }
        public List<Nutrient> Nutrients { get; set; }
        public List<LangualCode> LangualCodes { get; set; }
    }
}

