using System;
using System.IO;
using UnityEngine;
using System.Globalization;

namespace Jira.Runtime
{
    using Utility;
    using Extension = Utility.File.Extension;

    public class ScreenDocument
    {
        private readonly string _markdownPath;

        public ScreenDocument(string path)
        {
            _markdownPath = Path.Combine(path, $"{Application.productName} - Issues Log{Extension.md.Get()}");
        }

        public void SaveOnCrash(string file)
        {
            const string crash = "[project crash]";

            Save(crash, crash, crash, crash, file);
        }

        public void Save(string summary, string description, string project, string issue, string file)
        {
            try
            {
                var markdown = new FileInfo(_markdownPath);

                var date = DateTime.Now.ToString("yyyy/MM/dd/hh:mm", CultureInfo.InvariantCulture);

                if (!markdown.Exists)
                {
                    var table = MarkdownTableGenerator.Create(summary, description, project, description, date, file);

                    using var markdownFile = markdown.CreateText();

                    markdownFile.WriteLine(table);
                }
                else
                {
                    var row = MarkdownTableGenerator.AddRow(summary, description, project, issue, date, file);

                    using var markdownFile = markdown.AppendText();

                    markdownFile.WriteLine(row);
                }
            }
            catch (Exception e)
            {
#if JIRA_DEBUGGING
                Debug.LogError($"Error saving file: {e.Message}");
#else
                _ = e;
#endif
            }
        }
    }
}