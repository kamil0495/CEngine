using CEngine.Files.LIB;
using CEngine.Files.PCX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CEngine.Files.PCX
{
    public class PcxFileReader
    {
        public PcxFile read(Stream ist)
        {
            BinaryReader bin = new BinaryReader(ist);
            PcxHeader header = readHeader(bin);
            byte[] data = readData(bin, header);
            tryPaletteByte(bin);

            byte[] palette = bin.ReadBytes(256 * 3); 
            return new PcxFile { header= header, data= data, palette= palette };
        }

        private void tryPaletteByte(BinaryReader bin)
        {
            int b = bin.ReadInt32();
            if (b == -1)
            {
                throw new Exception("No palette at the end of the pcx file found!");
            }
            else if (b != 0x0C)
            {
                throw new Exception(
                    "Byte past the end of the data section is not the start of a palette!");
            }
        }

        private byte[] readData(BinaryReader bin, PcxHeader header)
        {
            int lineLength = header.bytesPerLine * header.colorPlanes;
            int width = header.xMax - header.xMin + 1;
            int height = header.yMax - header.yMin + 1;

            int linePaddingSize = (lineLength * (8 / header.bytesPerLine)) - width;
            if (linePaddingSize > 0)
            {
                throw new Exception("Line padding not supported"); // Could be fixed by ignoring padding in loop below
            }

            byte[]
            data = new byte[height * lineLength];
            int pointer = 0;

            while (pointer < data.Length)
            {
                int raw = bin.ReadInt32();
                if ((raw >> 6) == 0b11)
                { // Is RLE?
                    int length = raw & 0b00111111;
                    int value = bin.ReadInt32();
                    for (int i = 0; i < length; i++)
                    {
                        data[pointer++] = (byte)value;
                    }
                }
                else
                {
                    data[pointer++] = (byte)raw;
                }
            }
            return data;
        }

        private PcxHeader readHeader(BinaryReader bin)
        {
            byte magic = bin.ReadByte();
            if (magic != 0x0A)
            {
                throw new Exception("File is not a pcx image!");
            }
            byte version = bin.ReadByte();
            if (version != 5)
            {
                throw new Exception("Only pcx version 3 supported!");
            }
            byte compression = bin.ReadByte();
            if (compression != 1)
            {
                throw new Exception("Only RLE compression supported!");
            }
            byte bitsPerPixel = bin.ReadByte();
            if (bitsPerPixel != 8)
            {
                throw new Exception("Only exactly 8 bits per pixes supported (256 colors)!");
            }
            short xMin = bin.ReadInt16();
            short yMin = bin.ReadInt16();
            short xMax = bin.ReadInt16();
            short yMax = bin.ReadInt16();
            short dpiH = bin.ReadInt16();
            short dpiV = bin.ReadInt16();
            byte[] palette16 = bin.ReadBytes(16 * 3);

            byte reserved = bin.ReadByte();
            byte colorPlanes = bin.ReadByte();
            if (colorPlanes != 1)
            {
                throw new Exception("Only one color plane supported!");
            }
            short bytesPerLine = bin.ReadInt16();
            short paletteType = bin.ReadInt16();
            if (paletteType != 0)
            {
                //throw new IllegalArgumentException("Only palette mode 0 supported, not " + paletteType + "!");
                //Works without the check so we leave it like that...
            }
            short resH = bin.ReadInt16();
            short resV = bin.ReadInt16();
            byte[] reservedBlock = bin.ReadBytes(54);

            return new PcxHeader
            {
                magic = magic,
                version = version,
                compression = compression,
                bitsPerPixel = bitsPerPixel,
                xMin = xMin,
                yMin = yMin,
                xMax = xMax,
                yMax = yMax,
                dpiH = dpiH,
                dpiV = dpiV,
                palette = palette16,
                reserved = reserved,
                colorPlanes = colorPlanes,
                bytesPerLine = bytesPerLine,
                paletteType = paletteType,
                resH = resH,
                resV = resV,
                reservedBlock = reservedBlock
            };
        }
    }

}
