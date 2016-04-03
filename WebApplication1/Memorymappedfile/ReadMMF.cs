using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class ReadMMF
    {
        private static ReadMMF instance;
        private MemoryMappedFile mffile;
        private MemoryMappedViewStream stream;
        private int bytesToRead;

        private ReadMMF()
        {
            mffile = MemoryMappedFile.OpenExisting("ScreenMap");
            stream = mffile.CreateViewStream();

        }
        public static ReadMMF Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ReadMMF();
                }
                return instance;
            }
        }
        public void BytesToRead(int bytes)
        {
            bytesToRead = bytes;
        }
        public byte[] ReadMemoryMappedFile()
        {
            if (stream.Length > 0)
            {
                stream.Position = 0;
                byte[] bytes = new byte[bytesToRead];
                stream.Read(bytes, 0, bytesToRead);
                return bytes;
            }
            return null;
        }

    }
}