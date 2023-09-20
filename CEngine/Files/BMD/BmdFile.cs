using CEngine.Files.BMD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CEngine.Files.BMD.RawBmdFileReader;

namespace CEngine.Files.BMD
{
    public class BmdFile
    {
        public RawBmdFile rawBmdFile;

        public int getSize()
        {
            return rawBmdFile.frameInfo.Count;
        }

        public Bitmap getFrame(int frame, byte[] palette, Type4AlphaInterpretation t4i)

        {
            try
            {
                BmdFrameInfo frameInfo = rawBmdFile.frameInfo[frame];
                if (frameInfo.type == 0)
                {
                    return null;
                }
                else
                {
                    //Bitmap bmp = extractFrame(rawBmdFile, frameInfo, palette, t4i);
                    return extractFrame(rawBmdFile, frameInfo, palette, t4i);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to decode frame " + frame, ex);
            }
        }

        // TODO Speedup would be to decode the frame without palette to be able to change it dynamically
        private Bitmap extractFrame(
            RawBmdFile bmdFile, BmdFrameInfo frameInfo, byte[] palette, Type4AlphaInterpretation t4i)
        {
            int frameType = frameInfo.type;
            if (frameType != 1 && frameType != 2 && frameType != 4)
            {
                throw new Exception("Frame type " + frameType + " is not supported!");
            }
            int frameStart = frameInfo.off;
            int frameCount = frameInfo.len;
            int width = frameInfo.width;

            byte[] pixels = bmdFile.pixels;
            int pixelPointer = bmdFile.rowInfo[frameStart].offset;

            Bitmap bmp = new Bitmap(width, frameCount + 1);

            for (int row = 0; row < frameCount; row++)
            {
                BmdFrameRow rowInfo = bmdFile.rowInfo[row + frameStart];
                if (isEmpty(rowInfo))
                {
                    continue;
                }
                int indent = rowInfo.indent;
                int i = indent;

                int pixelBlockLength = pixels[pixelPointer++] & 0xFF;

                while (pixelBlockLength != 0)
                {
                    if (pixelBlockLength < 0x80)
                    {
                        for (int z = 0; z < pixelBlockLength; z++)
                        {
                            int color, alpha;
                            if (frameType == BmdFrameInfo.TYPE_EXTENDED)
                            {
                                color = getFromPalette(palette, pixels[pixelPointer++] & 0xFF);
                                if (t4i == Type4AlphaInterpretation.ALPHA)
                                {
                                    alpha = pixels[pixelPointer++] & 0xFF;
                                }
                                else
                                {
                                    alpha = 0xFF;
                                    pixelPointer++;
                                }
                            }
                            else if (frameType == BmdFrameInfo.TYPE_NORMAL)
                            {
                                alpha = 0xFF;
                                color = getFromPalette(palette, pixels[pixelPointer++] & 0xFF);
                            }
                            else if (frameType == BmdFrameInfo.TYPE_SHADOW)
                            {
                                alpha = 0x80;
                                color = 0x000000;
                            }
                            else
                            {
                                alpha = 0xFF;
                                color = 0xFF0000;
                            }
                            bmp.SetPixel(i++, row, Color.FromArgb(color | (alpha << 3 * 8)));
                        }

                    }
                    else
                    {
                        i += (pixelBlockLength - 0x80);
                    }
                    pixelBlockLength = pixels[pixelPointer++] & 0xFF;
                }
            }
            return bmp;
        }

        private ColorAlfa[][] extractFrameWithoutColors(
            RawBmdFile bmdFile, BmdFrameInfo frameInfo, Type4AlphaInterpretation t4i)
        {
            int frameType = frameInfo.type;
            if (frameType != 1 && frameType != 2 && frameType != 4)
            {
                throw new Exception("Frame type " + frameType + " is not supported!");
            }
            int frameStart = frameInfo.off;
            int frameCount = frameInfo.len;
            int width = frameInfo.width;

            byte[] pixels = bmdFile.pixels;
            int pixelPointer = bmdFile.rowInfo[frameStart].offset;

            ColorAlfa[][] pix=new ColorAlfa[frameCount][];
            //Bitmap bmp = new Bitmap(width, frameCount + 1);

            for (int row = 0; row < frameCount; row++)
            {
                pix[row]=new ColorAlfa[width];
                for (int o = 0; o < width; o++) pix[row][o] = new ColorAlfa { Color = 0, Alfa = 0 };
                BmdFrameRow rowInfo = bmdFile.rowInfo[row + frameStart];
                if (isEmpty(rowInfo))
                {                   
                    continue;
                }
                int indent = rowInfo.indent;
                int i = indent;

                int pixelBlockLength = pixels[pixelPointer++] & 0xFF;

                while (pixelBlockLength != 0)
                {
                    if (pixelBlockLength < 0x80)
                    {
                        for (int z = 0; z < pixelBlockLength; z++)
                        {
                            byte color, alpha;
                            if (frameType == BmdFrameInfo.TYPE_EXTENDED)
                            {
                                color = pixels[pixelPointer++];
                                if (t4i == Type4AlphaInterpretation.ALPHA)
                                {
                                    alpha = pixels[pixelPointer++];
                                }
                                else
                                {
                                    alpha = 0xFF;
                                    pixelPointer++;
                                }
                            }
                            else if (frameType == BmdFrameInfo.TYPE_NORMAL)
                            {
                                alpha = 0xFF;
                                color = pixels[pixelPointer++];
                            }
                            else if (frameType == BmdFrameInfo.TYPE_SHADOW)
                            {
                                alpha = 0x80;
                                color = 0x00;
                            }
                            else
                            {
                                alpha = 0xFF;
                                color = 0xFF;
                            }
                            pix[row][i++] = new ColorAlfa { Alfa = alpha, Color = color };
                        }

                    }
                    else
                    {
                        i += (pixelBlockLength - 0x80);
                    }
                    pixelBlockLength = pixels[pixelPointer++] & 0xFF;
                }
            }
            return pix;
        }


        private bool isEmpty(BmdFrameRow rowInfo)
        {
            return rowInfo.offset == 0b00111111_11111111_11111111
                && rowInfo.indent == 0b00000011_11111111;
        }

        private int getFromPalette(byte[] palette, int idx)
        {
            int pointer = idx * 3;
            int r = palette[pointer] & 0xFF;
            int g = palette[pointer + 1] & 0xFF;
            int b = palette[pointer + 2] & 0xFF;

            int color = 0x00000000;
            color |= r << 8 * 2;
            color |= g << 8 * 1;
            color |= b << 8 * 0;
            return color;
        }

        //private BufferedImage convertToImage(Bitmap bmp)
        //{
        //    if (bmp.getW() == 0 || bmp.getH() == 0)
        //    {
        //        return new BufferedImage(1, 1, BufferedImage.TYPE_INT_ARGB);
        //    }

        //    BufferedImage bu = new BufferedImage(bmp.getW(), bmp.getH(), BufferedImage.TYPE_INT_ARGB);
        //    bu.getRaster().setDataElements(0, 0, bmp.getW(), bmp.getH(), bmp.getColors());
        //    return bu;
        //}

        public enum Type4AlphaInterpretation
        {
            ALPHA,
            IGNORE
        }
    }
}
