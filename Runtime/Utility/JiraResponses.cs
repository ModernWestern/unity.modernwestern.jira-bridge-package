using System;

namespace Jira.Runtime
{
    [Serializable]
    public class JiraIssueResponseData
    {
        public string id, key, self;

        public override string ToString()
        {
            return $"ISSUE DATA -> Id: {id}, Key: {key}, Self: {self}";
        }
    }
}