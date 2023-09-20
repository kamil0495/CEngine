using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEngine.Files.LIB
{
    public class LibFileInfo
    {
        LibFormat format;
        List<DirMeta> dirMetas;
        List<FileMeta> fileMetas;

        public LibFileInfo(LibFormat format, List<DirMeta> dirMetas, List<FileMeta> fileMetas)
        {
            this.format = format;
            this.dirMetas = dirMetas;
            this.fileMetas = fileMetas;
        }
    }

}
