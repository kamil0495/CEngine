using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace CEngine.Files.PCX
{
    public class PcxFile
    {
        public PcxHeader header;
        public byte[] data;
        public byte[] palette;

        public int getPixel(int x, int y)
        {
            int dataIndex = header.getHeight() * y + x;
            int paletteIndex = data[dataIndex] & 0xFF;
            return getColor(paletteIndex);
        }

        public int getColor(int paletteIndex)
        {
            try
            {
                int paletteDataIndex = paletteIndex * 3;
                int c = 0;
                c |= (0xFF) << 8 * 3;
                c |= (palette[paletteDataIndex + 0] & 0xFF) << 8 * 2;
                c |= (palette[paletteDataIndex + 1] & 0xFF) << 8 * 1;
                c |= (palette[paletteDataIndex + 2] & 0xFF) << 8 * 0;
                return c;
            }
            catch
            {
                return 0;
            }
            
        }




        //public static BufferedImage convertToImage(PcxFile pcx)
        //{
        //    int width = getWidth(pcx.getHeader());
        //    int height = getHeight(pcx.getHeader());
        //    BufferedImage bu = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            int c = pcx.getPixel(x, y);
        //            bu.setRGB(x, y, c);
        //        }
        //    }
        //    return bu;
        //}
    }

    public class PcxHeader
    {
        public byte magic;
        public byte version;
        public byte compression;
        public byte bitsPerPixel;
        public short xMin;
        public short yMin;
        public short xMax;
        public short yMax;
        public short dpiH;
        public short dpiV;
        public byte[] palette;
        public byte reserved;
        public byte colorPlanes;
        public short bytesPerLine;
        public short paletteType;
        public short resH;
        public short resV;
        public byte[] reservedBlock;

        public int getWidth()
        {
            return xMax - xMin + 1;
        }
        public int getHeight()
        {
            return yMax - yMin + 1;
        }
    }


}
