using System.Text.RegularExpressions;

namespace Jira.Editor.Utility
{
    public static class StringExtension
    {
        public static string Clean(this string @this)
        {
            const string pattern = "[ \\t\\n]";

            return Regex.Replace(@this, pattern, string.Empty);
        }
    }
}