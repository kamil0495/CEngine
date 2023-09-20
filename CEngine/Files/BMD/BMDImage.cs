using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEngine.Files.BMD
{
    public class BMDImage
    {
        public Bitmap Bitmap { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int Type { get; set; }
        public ColorAlfa[][] pixels { get; set; }
    }
}
