using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEngine.Files.BMD
{
    public class SemiBMDFile
    {
        public List<BMDImage> frames = new List<BMDImage>();
        public SemiBMDFile(BmdFile bmdFile)
        {
            foreach(var a in bmdFile.rawBmdFile.frameInfo)
            {
                frames.Add(new BMDImage
                {

                });
            }
        }
        public SemiBMDFile()
        {

        }
    }
}
