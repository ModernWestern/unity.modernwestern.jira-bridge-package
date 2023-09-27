using System.Text;
using UnityEngine.SceneManagement;

namespace Jira.Runtime.Utility
{
    public static class MarkdownTableGenerator
    {
        private const string Header = "| Summary | Description | Project Key | Issue Type | Application Scene | Date | Screenshot |\n|------|------|------|------|------|------|------|";

        private const string TableFormat = "| {0} | {1} | {2} | {3} | {4} | {5} | ![[{6}]] |";

        public static string Create(string summary, string description, string project, string issue, string date, string imageUrl)
        {
            var tableContent = new StringBuilder().AppendLine(Header);

            tableContent.AppendFormat(TableFormat, summary, description, project, issue, SceneManager.GetActiveScene().name, date, imageUrl);

            return tableContent.ToString();
        }

        public static string AddRow(string summary, string description, string project, string issue, string date, string imageUrl)
        {
            return string.Format(TableFormat, summary, description, project, issue, SceneManager.GetActiveScene().name, date, imageUrl);
        }
    }
}