using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Web;

namespace WebApplication1
{
    public class WriteMMF
    {
        private static WriteMMF instance;
        private MemoryMappedFile mffile;
        private MemoryMappedViewAccessor accessor;
        private WriteMMF()
        {
            mffile = MemoryMappedFile.CreateNew("ScreenMap", 1000000);
            accessor = mffile.CreateViewAccessor();
        }
        public static WriteMMF Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WriteMMF();
                }
                return instance;
            }
        }

        public void WriteMemoryMappedFile(byte[] buffer)
        {
            accessor.Write(1, (ushort)buffer.Length);
            accessor.WriteArray(0, buffer, 0, buffer.Length);
        }
    }
}