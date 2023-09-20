using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CEngine.Files.BMD.RawBmdFileReader;

namespace CEngine.Files.BMD
{
    public class SemiBMDFile
    {
        public List<BMDImage> frames = new List<BMDImage>();
        public SemiBMDFile(BmdFile bmdFile, bool useAlpha)
        {
            int i = 1;
            foreach(var a in bmdFile.rawBmdFile.frameInfo)
            {
                frames.Add(new BMDImage
                {
                    Type = a.type,
                    x = a.dx,
                    y = a.dy,
                    pixels = bmdFile.getFrame(a, useAlpha ? BmdFile.Type4AlphaInterpretation.ALPHA : BmdFile.Type4AlphaInterpretation.IGNORE),
                    Number=i++
                }); ;
            }
        }

        public SemiBMDFile(BmdFile bmdFile)
        {
            int i = 1;
            foreach (var a in bmdFile.rawBmdFile.frameInfo)
            {
                frames.Add(new BMDImage
                {
                    Type = a.type,
                    x = a.dx,
                    y = a.dy,
                    pixels = bmdFile.getFrame(a, BmdFile.Type4AlphaInterpretation.IGNORE),
                    Number = i++
                }); ;
            }
        }

        public SemiBMDFile(BmdFile bmdFile,bool useAlpha, Color[] pallets) : this(bmdFile, useAlpha)
        {
            foreach (BMDImage bi in frames) bi.GenerateBitmap(pallets, useAlpha);
        }

        public SemiBMDFile()
        {

        }

        public void ChangeColors(Color[] colors, bool @checked)
        {
            foreach (BMDImage bi in frames) bi.GenerateBitmap(colors, @checked);
        }

        public void CreateRawBMD(BmdFile bmd)
        {
            
            List<BmdFrameInfo> frameInfos = new List<BmdFrameInfo>();
            List<byte> pixels = new List<byte>();
            List<BmdFrameRow> rows = new List<BmdFrameRow>();
            for(int i = 0; i < frames.Count; i++)
            {
                frames[i].GeneratePixelData(pixels);
                if (frames[i].Bitmap==null)
                    frameInfos.Add(new BmdFrameInfo { dx = frames[i].x, dy = frames[i].y, len = 0, off = rows.Count, type = frames[i].Type, width = 0 });

                else
                frameInfos.Add(new BmdFrameInfo { dx = frames[i].x, dy = frames[i].y, len = frames[i].rowsData.Length, off = rows.Count, type = frames[i].Type, width = frames[i].Bitmap.Width });
                if (frames[i].rowsData!=null)
                rows.AddRange(frames[i].rowsData);
            }
            BmdHeader header = new BmdHeader
            {
                magic = 1012,
                numFrames = frames.Count,
                numPixels = pixels.Count,
                numRows = rows.Count
            };
            bmd.rawBmdFile = new RawBmdFile
            {
                header = header,
                frameInfo = frameInfos,
                pixels = pixels.ToArray(),
                rowInfo = rows
            };

        }
    }
}
