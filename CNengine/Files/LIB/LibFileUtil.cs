//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CEngine.Files.LIB
//{
//    public class LibFileUtil
//    {
//        public static LibFileInfo read(InputStream in, LibFormat format)
//        {
//            try
//            {
//                LittleEndianDataInputStream dis = new LittleEndianDataInputStream(in);
//                Header header = new HeaderCodec().unpack(dis, format);

//                List<DirMeta> dirMetas;
//                if (format == LibFormat.CULTURES2)
//                {
//                    DirMetaCodec dirMetaCodec = new DirMetaCodec();

//                    dirMetas = new ArrayList<>();
//                    for (int i = 0; i < header.getDirCount(); i++)
//                    {
//                        dirMetas.add(dirMetaCodec.unpack(dis));
//                    }
//                }
//                else
//                {
//                    dirMetas = null;
//                }

//                FileMetaCodec fileMetaCodec = new FileMetaCodec();
//                List<FileMeta> fileMetas = new ArrayList<>();
//                for (int i = 0; i < header.getFileCount(); i++)
//                {
//                    fileMetas.add(fileMetaCodec.unpack(dis));
//                }
//                return new LibFileInfo(format, dirMetas, fileMetas);
//            }
//            finally
//            {
//                in.close();
//            }
//        }
//    }
//}
