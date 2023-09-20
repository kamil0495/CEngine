using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cultures2_gl_port.cultures.models
{
    public class FSHeader
    {
        public UInt32 version { get; set; }
        public UInt32 num_dirs { get; set; }
        public UInt32 num_files { get; set; }
    }

    public class FSDirInfo
    {
        public string path { get; set; }
        public UInt32 depth { get; set; }
    }

    public class FSFileInfo
    {
        public string name { get; set; }
        public string path { get; set; }
        public UInt32 offset { get; set; }
        public UInt32 length { get; set; }
    }
}
