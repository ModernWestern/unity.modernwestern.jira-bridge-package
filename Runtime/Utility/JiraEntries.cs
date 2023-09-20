using System;

namespace Jira.Runtime
{
    [Serializable]
    public class JiraIssueData
    {
        public Fields fields;

        public JiraIssueData(string key, string summary, string description, string issueType)
        {
            fields = new Fields
            {
                summary = summary,

                description = description,

                project = new Project
                {
                    key = key
                },

                issuetype = new IssueType
                {
                    id = issueType
                }
            };
        }

        public override string ToString()
        {
            return $"Project: {fields.project.key}, Summary: {fields.summary}, Description: {fields.description}, Issue Type: {fields.issuetype.id}";
        }
    }

    [Serializable]
    public class Fields
    {
        public Project project;
        public string summary;
        public string description;
        public IssueType issuetype;
    }

    [Serializable]
    public class IssueType
    {
        public string id;
    }

    [Serializable]
    public class Project
    {
        public string key;
    }
}