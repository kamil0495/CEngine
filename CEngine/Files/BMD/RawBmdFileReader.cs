using CEngine.Files.LIB;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace CEngine.Files.BMD
{
    public class RawBmdFileReader
    {
        public RawBmdFile read(Stream ist)
        {
            BinaryReader bin = new BinaryReader(ist);
            BmdHeader header = readBmdHeader(bin);
            if (header.numFrames == 0 && header.numPixels == 0 && header.numRows == 0)
            {
                return new RawBmdFile { header = header, frameInfo = new List<BmdFrameInfo>(), pixels = new byte[0], rowInfo = new List<BmdFrameRow>() };
            }
            else
            {
                List<BmdFrameInfo> frameInfo = readFramesSection(ist);
                byte[] pixels = readPixelsSection(ist);
                List<BmdFrameRow> rowInfo = readRowsSection(ist);
                return new RawBmdFile{header = header, frameInfo = frameInfo, pixels = pixels, rowInfo = rowInfo};
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

        private BmdHeader readBmdHeader(BinaryReader bin)
        {
            int magic = bin.ReadInt32();
            int zero0 = bin.ReadInt32();
            int zero1 = bin.ReadInt32();
            int numFrames = bin.ReadInt32();
            int numPixels = bin.ReadInt32();
            int numRows = bin.ReadInt32();
            int unknown0 = bin.ReadInt32();
            int unknown1 = bin.ReadInt32();
            int zero2 = bin.ReadInt32();
            return new BmdHeader(magic, numFrames, numPixels, numRows);
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


        public class BmdHeader
        {
            public int magic;
            public int numFrames;
            public int numPixels;
            public int numRows;
            public BmdHeader() { }
            public BmdHeader(int magic, int numFrames, int numPixels, int numRows)
            {
                this.magic = magic;
                this.numFrames = numFrames;
                this.numPixels = numPixels;
                this.numRows = numRows;
            }

            public void Write(BinaryWriter bw)
            {
                bw.Write(magic);
                bw.Write(0);
                bw.Write(0);
                bw.Write(numFrames);
                bw.Write(numPixels);
                bw.Write(numRows);
                bw.Write(0);
                bw.Write(0);
                bw.Write(0);
            }
        }


        public class BmdSectionHeader
        {
            public int magic;
            public int length;
        }


        public class BmdFrameInfo
        {
            public static int SIZE = 6 * 4;
            public static int TYPE_NORMAL = 1;
            public static int TYPE_SHADOW = 2;
            public static int TYPE_EXTENDED = 4;

            public int type;
            public int dx;
            public int dy;
            public int width;
            public int len;
            public int off;
        }


        public class BmdFrameRow
        {
            public static int SIZE = 4;
            public int offset; // These are swapped in the docs! First are 22 bits offset and then 10 bits indent!
            public int indent;
            public BmdFrameRow(int zoffset,int zdent)
            {
                offset = zoffset;
                indent = zdent;
            }
        }
    }
}
