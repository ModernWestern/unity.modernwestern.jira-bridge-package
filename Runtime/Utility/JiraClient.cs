using System;

namespace Jira.Runtime
{
    [Serializable]
    public class JiraClient
    {
        public string domain, user, token, issueid, projectkey;

        public override string ToString()
        {
            return $"Domain: {domain}, User: {user}, Token: {token}, Issue Id: {issueid}, Project Key: {projectkey}";
        }
    }
}