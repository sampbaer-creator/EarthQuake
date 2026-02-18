using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USGS_Earthquake_Catalog
{
    public class Earthquake
    {
        public int EQid;
        public string EQLocation { get; set; }
        public double EQSize { get; set; }
    }
}
