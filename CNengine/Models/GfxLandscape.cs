using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEngine.Models
{
    public class GfxLandscape
    {
       public string EditName { get; set; }
  public string[] EditGroups { get; set; }
    public int    LogicType { get; set; }
        public int LogicMaximumValency { get; set; }
        public bool LogicIsWorkable { get; set; }
        public bool logicispileableonmap { get; set; }
        public int[][] LogicWalkBlockArea { get; set; }
        public int [][] LogicBuildBlockArea { get; set; }
        public int [][]LogicWorkArea { get; set; }
        //public BMD GfxBobLibs { get; set; }
        public string[] GfxPalette { get; set; }
        public IntIntArray GfxFrames { get; set; }
        public bool GfxStatic { get; set; }
        public bool GfxLoopAnimation { get; set; }
        public int GfxShadingFactor { get; set; }
        public int GfxUserFXMatrix { get; set; }
        public bool GfxDynamicBackground { get; set; }
        public bool Gfxdrawvoidever { get; set; }
        public IntString GfxTransition { get; set; }
    }
}
