using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CEngine.Files.BMD.RawBmdFileReader;

namespace CEngine.Files.BMD
{
    public class RawBmdFile
    {
        public BmdHeader header;
        public List<BmdFrameInfo> frameInfo;
        public byte[] pixels;
        public List<BmdFrameRow> rowInfo;
    }

}
