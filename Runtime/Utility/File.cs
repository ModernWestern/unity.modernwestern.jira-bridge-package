﻿namespace Jira.Runtime.Utility
{
    public static class File
    {
        public enum Extension
        {
            txt,
            md,
            mp4,
            png,
            meta,
            asmref,
            js
        }

        private const string    Text = ".txt",
                                Markdown = ".md",
                                Mp4 = ".mp4",
                                PNG = ".png",
                                Meta = ".meta",
                                Js = ".js",
                                AssemblyRef = ".asmref";

        public static string Get(this Extension extension)
        {
            return extension switch
            {
                Extension.md => Markdown,
                Extension.mp4 => Mp4,
                Extension.png => PNG,
                Extension.txt => Text,
                Extension.meta => Meta,
                Extension.js => Js,
                Extension.asmref => AssemblyRef,
                _ => ""
            };
        }
    }
}