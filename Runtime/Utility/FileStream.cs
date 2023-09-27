﻿using System.IO;

namespace Utility
{
    public static class FFileStream
    {
        public static void ReadAndClose(this FileStream @this, byte[] array, int offset, int count)
        {
            _ = @this.Read(array, offset, count);

            @this.Close();
        }
    }
}