using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEngine.Models
{
    public class GfxPattern
    {
        public string EditName { get; set; }
        public string[] EditGroups { get; set; }
        public int LogicType { get; set; }
        public string GfxTexture { get; set; }
        public int[] GfxCoordsA { get; set; }
        public int[] GfxCoordsB { get; set; }
    }
}
