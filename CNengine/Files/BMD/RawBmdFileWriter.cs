using CEngine.Files.LIB;
using System;
using System.Collections.Generic;

using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static CEngine.Files.BMD.RawBmdFileReader;

namespace CEngine.Files.BMD
{
    public class RawBmdFileWriter
    {
        public void Write(List<Bitmap> bitmaps, Stream s)
        {
            BinaryWriter bw = new BinaryWriter(s);
            BmdHeader header = new BmdHeader { numFrames = bitmaps.Count };
            
            BmdSectionHeader frameheader = CreateFramesSection(bitmaps.Count);
            List<BmdFrameInfo> frameInfo = new List<BmdFrameInfo>();
            foreach(Bitmap bitmap in bitmaps)
            {
               if (bitmap.Tag is BmdFrameInfo) frameInfo.Add((BmdFrameInfo)bitmap.Tag);
            }
        }



        private List<BmdFrameRow> readRowsSection(Stream sin)
        {
            BinaryReader binReader = new BinaryReader(sin);
            BmdSectionHeader rowsSectionHeader = readBmdSectionHeader(binReader);
            List<BmdFrameRow> rowInfo = new List<BmdFrameRow>();
            for (int i = 0; i < rowsSectionHeader.length / BmdFrameRow.SIZE; i++)
            {
                rowInfo.Add(readBmdFrameRow(binReader));
            }
            return rowInfo;
        }

        private byte[] readPixelsSection(Stream sin)
        {
            BinaryReader binReader = new BinaryReader(sin);
            BmdSectionHeader pixelsSectionHeader = readBmdSectionHeader(binReader);
            return binReader.ReadBytes(pixelsSectionHeader.length);
        }

        private List<BmdFrameInfo> readFramesSection(Stream sin)
        {
            BinaryReader binReader = new BinaryReader(sin);
            BmdSectionHeader framesSectionHeader = readBmdSectionHeader(binReader);
            List<BmdFrameInfo> frameInfo = new List<BmdFrameInfo>();
            for (int i = 0; i < framesSectionHeader.length / BmdFrameInfo.SIZE; i++)
            {
                frameInfo.Add(readBmdFrameInfo(binReader));
            }
            return frameInfo;
        }

        private BmdSectionHeader CreateFramesSection(int frames)
        {
            return new BmdSectionHeader { magic = 0, length = frames * BmdFrameInfo.SIZE };
        }


        private BmdSectionHeader readBmdSectionHeader(BinaryReader bin)
        {
            int magic = bin.ReadInt32();
            int zero0 = bin.ReadInt32();
            int length = bin.ReadInt32();
            return new BmdSectionHeader { magic=magic, length=length };
        }

        private BmdFrameInfo readBmdFrameInfo(BinaryReader bin)
        {
            int type = bin.ReadInt32();
            int meta1 = bin.ReadInt32();
            int meta2 = bin.ReadInt32();
            int width = bin.ReadInt32();
            int len = bin.ReadInt32();
            int off = bin.ReadInt32();
            return new BmdFrameInfo { type=type, dx=meta1, dy=meta2, width=width, len=len, off=off };
        }

        private BmdFrameRow readBmdFrameRow(BinaryReader bin)
        {
            int data = bin.ReadInt32();
            // the lower bits are taken first; so offset with 22 and then the higher order bits are the 10
            // indent
            int offset = data & 0b00111111_11111111_11111111;
            int indent = (data >> 22) & 0b11_11111111;
            return new BmdFrameRow(offset, indent);
        }


      
    }
}
