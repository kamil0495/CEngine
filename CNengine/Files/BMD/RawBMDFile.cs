using System;
using System.Collections.Generic;
using System.IO;
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

        public void Save(BinaryWriter bw)
        {
            header.Write(bw);
            CreateHeaderSection(frameInfo.Count * BmdFrameInfo.SIZE).Write(bw);
            for(int i= 0; i < frameInfo.Count; i++)
            {
                frameInfo[i].Write(bw);
            }
            CreateHeaderSection(pixels.Length).Write(bw);
            bw.Write(pixels);
            CreateHeaderSection(rowInfo.Count * BmdFrameRow.SIZE).Write(bw);
            for (int i = 0; i < rowInfo.Count; i++)
            {
                rowInfo[i].Write(bw);
            }

        }

        private BmdSectionHeader CreateHeaderSection(int count)
        {
            return new BmdSectionHeader { magic = 1001, length = count };
        }
    }

}
