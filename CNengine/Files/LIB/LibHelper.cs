using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CEngine.Files.LIB
{
    public static class LibHelper
    {
        public static DirMeta DirUnpack(Stream dis)
        {
            BinaryReader reader = new BinaryReader(dis);
            int nameLength = reader.ReadInt32();
            byte[] chars = new byte[nameLength];
            reader.Read(chars, 0, nameLength);
            int level = reader.ReadInt32();
            string name= Encoding.Default.GetString(chars);
            return new DirMeta { name = name, level = level };
        }

        public static FileMeta FileUnpack(Stream dis)
        {
            BinaryReader reader = new BinaryReader(dis);
            int nameLength = reader.ReadInt32();
            byte[] chars = new byte[nameLength];

            reader.Read(chars, 0, nameLength);
            string name = Encoding.Default.GetString(chars);
            int pos = reader.ReadInt32();
            int len = reader.ReadInt32();
            return new FileMeta { name = name, pos = pos, len = len };
        }

        public static Header HeaderUnpack(Stream dis, LibFormat format)
        {
            BinaryReader reader = new BinaryReader(dis);
            int unknown = reader.ReadInt32();
            int dirCount;
            if (format == LibFormat.CULTURES2)
            {
                dirCount = reader.ReadInt32();
            }
            else
            {
                dirCount = -1;
            }
            int fileCount = reader.ReadInt32();
            return new Header { unknown = unknown, dirCount = dirCount, fileCount = fileCount };
        }

        //public static void fillArray(InputStream src, byte[] dest)
        //{
        //    int filled = 0;
        //    do
        //    {
        //        int read = src.read(dest, filled, dest.length - filled);
        //        if (read == -1)
        //        {
        //            throw new EOFException();
        //        }
        //        filled += read;
        //    } while (filled != dest.length);
        //}
    }

    public class DirMeta
    {
        public string name;
        public int level;
    }

    public class FileMeta
    {
        public string name;
        public int pos;
        public int len;

        public string getFileExtension()
        {
            return Path.GetExtension(this.name);
        }
    }

    public class Header
    {
        public int unknown;
        public int dirCount;
        public int fileCount;
    }

}
