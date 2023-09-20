//using CEngine.Files.LIB;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics.Metrics;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Channels;
//using System.Threading.Tasks;

//namespace CEngine.Files.LIB
//{
//    public class Lib //implements AutoCloseable
//    {
//        private FileStream channel;
//        private LibFileDirectory root;

//        public ReadableLibFile(Path p, LibFormat libFormat)
//        {
//            this(Files.newByteChannel(p), libFormat);
//        }

//        public ReadableLibFile(SeekableByteChannel channel, LibFormat libFormat)
//        {
//            this.channel = channel;
//            this.root = new LibFileDirectory(null);
//            LibFileInfo metas =
//                LibFileUtil.read(new NonClosableInputStream(Channels.newInputStream(channel)), libFormat);
//            initTree(metas);
//        }

//        private void initTree(LibFileInfo metas)
//        {
//            for (FileMeta fileMeta : metas.getFileMetas())
//            {
//                String[] parts = fileMeta.getName().split("\\\\");
//                LibFileDirectory parent = root;
//                for (int i = 0; i < parts.length - 1; i++)
//                {
//                    parent = parent.getDirectories().computeIfAbsent(parts[i], LibFileDirectory::new);
//                }
//                String filename = parts[parts.length - 1];
//                parent
//                .getFiles()
//                    .put(
//                filename,
//                new LibFileFile(
//                filename,
//                fileMeta.getLen(),
//                            new ChannelFileData(channel, fileMeta.getPos(), fileMeta.getLen())));
//            }
//        }

//        public LibFileDirectory getRoot()
//        {
//            return root;
//        }

//        @Override
//  public void close() throws Exception
//        {
//            channel.close();
//}


//  public class LibFileDirectory
//    {
//        string name;
//        Map<String, LibFileDirectory> directories = new HashMap<>();
//        Map<String, LibFileFile> files = new HashMap<>();

//        public LibFileDirectory(string name)
//        {
//            this.name = name;
//        }
//    }

//    @Value
//  public static class LibFileFile
//    {
//        String name;
//        int size;
//        FileData data;
//    }

//    @Value
//  private static class ChannelFileData
//    {
//        ISeekableByteChannel channel;
//    long position;
//    int size;

//        @Override
//    public InputStream getInputStream() 
//    {
//        channel.position(position);
//      return new BoundedInputStream(
//new LittleEndianDataInputStream(
//new NonClosableInputStream(Channels.newInputStream(channel))),
//          size);
//    }
//  }
//}
//}
