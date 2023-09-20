using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Jira.Runtime
{
    using Utility;
    using Extension = Utility.File.Extension;

    public class ScreenCapturer
    {
        public string LastFileName { get; private set; }

        public string LastFile { get; private set; }

        public int AttachmentsAmount => Exists(_path) ? GetFiles(_path).Count(file => !file.EndsWith(Extension.meta.Get()) && !file.EndsWith(Extension.mp4.Get())) : 0;

        private readonly string _path;

        public ScreenCapturer(string path)
        {
            _path = path;
        }

        public void Save()
        {
            LastFileName = $"{Application.productName}_{System.DateTime.Now:yyyy_MM_dd}_{AttachmentsAmount}{Extension.png.Get()}";

            LastFile = Path.Combine(_path, LastFileName);

            ScreenCapture.CaptureScreenshot(LastFile);
        }

        #region HELPERS

        private static IEnumerable<string> GetFiles(string path) => Directory.GetFiles(path);

        private static bool Exists(string path) => Directory.Exists(path);

        #endregion
    }
}