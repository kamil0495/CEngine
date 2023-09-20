using CNengine.Files.BMD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CEngine.Files.BMD.RawBmdFileReader;

namespace CEngine.Files.BMD
{
    public class BMDImage
    {
        public int Number { get; set; }
        public Bitmap Bitmap { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int Type { get; set; }
        public ColorAlfa[][] pixels { get; set; }
        //public List<byte> pixelData { get; set; }
        public BmdFrameRow[] rowsData {get;set;}
        public int len;
        public int off;

        public int ComboType()
        {
            throw new NotImplementedException();
        }

        public void GenerateBitmap(Color[] pallets, bool usealpha)
        {
            if (pixels == null) return;
            Bitmap = new Bitmap(pixels[0].Length, pixels.Length);
            for(int i = 0; i < pixels.Length; i++)
            {
                for (int j = 0; j < pixels[i].Length; j++)
                {
                    Bitmap.SetPixel(j, i, Color.FromArgb(usealpha || pixels[i][j].Color == 0 ? pixels[i][j].Alfa:255, pallets[pixels[i][j].Color]));
                }
            }
        }

        public void GeneratePixelData(List<byte> pixelData)
        {
            //pixelData = new List<byte>(List<byte> pixelData);
            if (Bitmap == null) return;
            rowsData = new BmdFrameRow[Bitmap.Height];
            int frameType = Type;
            if (frameType != 1 && frameType != 2 && frameType != 4)
            {
                throw new Exception("Frame type " + frameType + " is not supported!");
            }

            if (true)
            {
                for (int i = 0; i < Bitmap.Height; i++)
                {
                    rowsData[i] = new BmdFrameRow();
                    rowsData[i].offset = pixelData.Count;
                    rowsData[i].indent = 0;

                    List<PixelBlock> pbl = new List<PixelBlock>();
                    PixelBlock pb = new PixelBlock
                    {
                        Count = 1,
                        Empty = pixels[i][0].Alfa == 0,
                        off=0
                    };

                    for (int j = 1; j < Bitmap.Width; j++)
                    {           
                        if (pb.Empty== (pixels[i][j].Alfa == 0))
                        {
                            pb.Count++;
                        }
                        else
                        {
                            pbl.Add(pb);
                            pb = new PixelBlock { Count = 1, Empty = pixels[i][j].Alfa == 0, off=j };
                        }
                    }
                    pbl.Add(pb);

                    for(int p = 0; p < pbl.Count; p++)
                    {
                        if (p==0 && pbl[p].Empty)
                        {
                            rowsData[i].indent = pbl[p].Count;
                        }else if (pbl[p].Empty)
                        {
                            while(pbl[p].Count > 0)
                            {
                                if (pbl[p].Count >= 128)
                                {
                                    pixelData.Add(127+ 128);
                                    pbl[p].Count -= 128;
                                }
                                else
                                {
                                    pixelData.Add(Convert.ToByte(127 + pbl[p].Count));
                                    pbl[p].Count = 0;
                                }
                            }
                        }
                        else
                        {
                            int t = 0;
                            for (int j = pbl[p].off; j < pbl[p].off+ pbl[p].Count; j++)
                            {
                                if (t % 127 == 0)
                                {
                                    if ((j + 127) <= pbl[p].off + pbl[p].Count)
                                        pixelData.Add(127);
                                    else
                                        pixelData.Add(Convert.ToByte(pbl[p].off + pbl[p].Count - j));
                                }
                                if (Type == 4)
                                {
                                    pixelData.Add(pixels[i][j].Color);
                                    pixelData.Add(pixels[i][j].Alfa);
                                }
                                else if (Type == 1)
                                {
                                    pixelData.Add(pixels[i][j].Color);
                                }
                                else
                                {
                                    //Shadow nothing color
                                }                               
                                t++;
                            }
                        }
                    }
                    pixelData.Add(0);
                }
                //for (int i = 0; i < Bitmap.Height; i++)
                //{
                //    rowsData[i] = new BmdFrameRow();
                //    rowsData[i].offset = pixelData.Count;
                //    rowsData[i].indent = 0;

                //    for (int j = 0; j < Bitmap.Width; j++)
                //    {
                //        if (j % 127 == 0)
                //        {
                //            if ((j + 127) <= Bitmap.Width)
                //                pixelData.Add(127);
                //            else
                //                pixelData.Add(Convert.ToByte(Bitmap.Width - j));
                //        }
                //        pixelData.Add(pixels[i][j].Color);
                //        pixelData.Add(pixels[i][j].Alfa);
                //    }
                //    pixelData.Add(0);
                //}
            }
            else
            {
                throw new Exception("Brak obsługi tego typu klatki!");
                for (int i = 0; i < Bitmap.Height; i++)
                {
                    bool empty = true;
                    for (int j = 0; j < Bitmap.Width; j++)
                    {
                        if (pixels[i][j].Alfa == 0) { continue; }
                        if (empty)
                        {
                            empty = false;
                            rowsData[i].indent = j;
                        }
                        rowsData[i].offset = pixelData.Count;

                    }
                    if (empty)
                    {
                        rowsData[i].offset = 0b00111111_11111111_11111111;
                        rowsData[i].indent = 0b00000011_11111111;
                    }
                }
            }

            //int frameStart = frameInfo.off;
            //int frameCount = frameInfo.len;
            //int width = frameInfo.width;

            //byte[] pixels = bmdFile.pixels;
            //int pixelPointer = bmdFile.rowInfo[frameStart].offset;

            
            ////Bitmap bmp = new Bitmap(width, frameCount + 1);

            //for (int row = 0; row < frameCount; row++)
            //{
            //    pix[row] = new ColorAlfa[width];
            //    for (int o = 0; o < width; o++) pix[row][o] = new ColorAlfa { Color = 0, Alfa = 0 };

            //    BmdFrameRow rowInfo = bmdFile.rowInfo[row + frameStart];
            //    if (isEmpty(rowInfo))
            //    {
            //        continue;
            //    }
            //    int indent = rowInfo.indent;
            //    int i = indent;

            //    int pixelBlockLength = pixels[pixelPointer++] & 0xFF;

            //    while (pixelBlockLength != 0)
            //    {
            //        if (pixelBlockLength < 0x80)
            //        {
            //            for (int z = 0; z < pixelBlockLength; z++)
            //            {
            //                byte color, alpha;
            //                if (frameType == BmdFrameInfo.TYPE_EXTENDED)
            //                {
            //                    color = pixels[pixelPointer++];
            //                    if (t4i == Type4AlphaInterpretation.ALPHA || true)
            //                    {
            //                        alpha = pixels[pixelPointer++];
            //                    }
            //                    else
            //                    {
            //                        alpha = 0xFF;
            //                        pixelPointer++;
            //                    }
            //                }
            //                else if (frameType == BmdFrameInfo.TYPE_NORMAL)
            //                {
            //                    alpha = 0xFF;
            //                    color = pixels[pixelPointer++];
            //                }
            //                else if (frameType == BmdFrameInfo.TYPE_SHADOW)
            //                {
            //                    alpha = 0x80;
            //                    color = 0x00;
            //                }
            //                else
            //                {
            //                    alpha = 0xFF;
            //                    color = 0xFF;
            //                }
            //                pix[row][i++] = new ColorAlfa { Alfa = alpha, Color = color };
            //            }

            //        }
            //        else
            //        {
            //            i += (pixelBlockLength - 0x80);
            //        }
            //        pixelBlockLength = pixels[pixelPointer++] & 0xFF;
            //    }
            //}
            
        }

        public void GeneratePixels(Color[] colors)
        {
            pixels = new ColorAlfa[Bitmap.Height][];
            List<Color> colo = colors.ToList();
            for (int i = 0; i < Bitmap.Height; i++)
            {
                pixels[i] = new ColorAlfa[Bitmap.Width]; 
                for (int j = 0; j < Bitmap.Width; j++)
                {
                    pixels[i][j] = new ColorAlfa { Alfa = Bitmap.GetPixel(j, i).A, Color = Convert.ToByte(closestColor2(colo, Bitmap.GetPixel(j, i))) };
                }
            }            
        }

        private int closestColor2(List<Color> colors, Color target)
        {
            var colorDiffs = colors.Select(n => ColorDiff(n, target)).Min(n => n);
            return colors.FindIndex(n => ColorDiff(n, target) == colorDiffs);
        }

        private int ColorDiff(Color c1, Color c2)
        {
            return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                 + (c1.G - c2.G) * (c1.G - c2.G)
                                 + (c1.B - c2.B) * (c1.B - c2.B));
        }

        public override string ToString()
        {
            switch (Type)
            {
                case 0: return Number.ToString() + " Empty";
                case 1: return Number.ToString() + " Normal";
                case 2: return Number.ToString() + " Shadow";
                case 4: return Number.ToString() + " Extended";
            }
            return Number.ToString() + " Error type";
        }

        public void SetType(string text)
        {
            switch (text)
            {
                case "Normal":
                    Type = 1;
                    break;
                case "Shadow":
                    Type = 2;
                    break;
                case "Extended":
                    Type = 4;
                    break;
                default:
                    Type = 0;
                    break;
            }
        }

        public Bitmap GetBitmap(bool @checked)
        {
            if (!@checked) return Bitmap;

            Bitmap bmp = new Bitmap(Bitmap);
            if (x > 0) return bmp;
            if (x < 0-bmp.Width+1) return bmp;
            if (y > 0) return bmp;
            if (y < 0 - bmp.Height+1) return bmp;
            for (int i = (0-x) - 5; i <= (0 - x) + 5; i++)
            {
                if (i > 0 && i<bmp.Width)  bmp.SetPixel(i, (0 - y), Color.Red);
            }

            for (int i = (0 - y) - 5; i <= (0 - y) + 5; i++)
            {
               if (i> 0 && i < bmp.Height) bmp.SetPixel(0-x, i, Color.Red);
            }
            return bmp;
        }

        public string ToTypeString()
        {
            switch (Type)
            {
                case 0: return "Empty";
                case 1: return "Normal";
                case 2: return "Shadow";
                case 4: return "Extended";
            }
            return "Error type";
        }

    }
}
