using cultures2_gl_port.cultures.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace cultures2_gl_port.cultures
{
    public class FS
    {
        FSHeader header;
        List<FSDirInfo> dirs;
        List<FSFileInfo> files;
        FileStream fs;
        BinaryReader buffer;
        public FS(string file)
        {
            fs = new FileStream(file,FileMode.Open, FileAccess.Read);
            fs.Seek(0, SeekOrigin.Begin);
            BinaryReader buffer = new BinaryReader(fs);
            
            header = getHeader(buffer);
            getDirs(header.num_dirs, buffer);
            getFiles(header.num_files, buffer);
        }

        private FSHeader getHeader(BinaryReader fs)
        {
            return new FSHeader { version = fs.ReadUInt32(), num_dirs = fs.ReadUInt32(), num_files = fs.ReadUInt32() };
        }

        private void getDirs(UInt32 n, BinaryReader fs) 
        {
            dirs = new List<FSDirInfo>();
            for (int i = 0; i < n; i++)
            {
                dirs.Add(new FSDirInfo { path = GetString(fs), depth = fs.ReadUInt32() });
            }
        }

        public FSFileInfo GetFileInfo(string path)
        {
            FSFileInfo info= files.FirstOrDefault(f => f.path == path);
            if (info == null) throw new Exception("File not found: " + path);

            return info;
        }

        public byte[] Open(string path)
        {
            FSFileInfo fi = GetFileInfo(path);
            fs.Seek(fi.offset, SeekOrigin.Begin);

            return buffer.ReadBytes(Convert.ToInt32(fi.offset));
        }

        private string GetString(BinaryReader br)
        {
            UInt32 lenght = br.ReadUInt32();
            return System.Text.Encoding.ASCII.GetString( br.ReadBytes(Convert.ToInt32(lenght)));
        }

        private void getFiles(UInt32 n, BinaryReader fs)
        {
            files = new List<FSFileInfo>();
            for (int i = 0; i < n; i++)
            {
                files.Add(new FSFileInfo { path = GetString(fs), offset = fs.ReadUInt32(),length= fs.ReadUInt32() });
            }
        }




    }
}
