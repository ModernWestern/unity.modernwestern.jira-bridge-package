using System.IO;
using UnityEngine;

namespace Utility
{
    public static class GitIgnore
    {
        private const string GitIgnoreCommand = "/[Jj]ira Issues Log/";

        public static void ExcludeLogs()
        {
            var gitignore = new FileInfo(Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? string.Empty, ".gitignore"));

            if (gitignore.Exists)
            {
                using var gitignoreFile = gitignore.AppendText();

                gitignoreFile.WriteLine(GitIgnoreCommand);
            }
        }
    }
}