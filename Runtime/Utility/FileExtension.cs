using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Utility
{
    public static class FileExtension
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFile(string lpFileName);

        public static void ForceDelete(this FileInfo @this)
        {
            try
            {
                @this.Delete();
            }
            catch (IOException e)
            {
#if UNITY_EDITOR
                Debug.LogError($"Error: {e.Message}");
#else
                _ = e;
#endif
            }

            if (DeleteFile(@this.FullName))
            {
#if UNITY_EDITOR
                Debug.LogError("File forcefully deleted successfully.");
#endif
            }
            else
            {
                var error = Marshal.GetLastWin32Error();
#if UNITY_EDITOR
                Debug.LogError($"Failed to forcefully delete the file. Error code: {error}");
#endif
            }
        }
    }
}