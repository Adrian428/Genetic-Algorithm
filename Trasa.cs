using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDTIGS
{
    class Trasa
    {
        public int Id { get; set; }
        public List<int> KolejnoscMiast { get; set; }
        public int SumaOdleglosci { get; set; }
        public int Ocena { get; set; }
    }
}
