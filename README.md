# Jira Bridge - Unity QA Tool

## Overview
Jira Bridge is a powerful Unity QA tool that allows you to seamlessly create Jira issues directly from your Unity project. This tool simplifies the process of reporting and managing issues by integrating with Jira, the popular issue tracking and project management software.

## Getting Started

### Configuration
To use Jira Bridge, you need to configure the following settings:

1. **Jira Organization Domain**: Enter your Jira organization's domain.
2. **Jira User**: Provide your Jira username.
3. **Jira API Token**: Generate an [API token](https://id.atlassian.com/manage-profile/security/api-tokens) in your Jira account settings and enter it here.
4. **Issue Type ID**: Specify the ID for the issue type you want to create (e.g., Bug, Task).
5. **Issue Key**: Enter the key for the Jira project where you want to create issues.

These settings are saved in a JSON file named "JiraSettings," located in the Streaming Assets folder of your Unity project.

### Usage

#### 1. Setup Configuration
Before using Jira Bridge, make sure to set up the configuration as described in the "Configuration" section.

#### 2. In-Game Interface
Jira Bridge provides an intuitive in-game interface that can be accessed while the game is in play mode. Here's how to use it:

- **Summary**: Enter a brief summary of the issue you want to report.
- **Description**: Provide a detailed description of the issue.
- **Upload**: Click the "Upload" button to create the Jira issue. The tool will capture a screenshot of the current game state for reference.

#### 3. Issue Log
As you upload issues, Jira Bridge saves them locally in a Markdown file named "[project name] - issuesLog.md." This file keeps a record of all the issues you've uploaded previously, making it easy to track and reference past issues.

## License
Jira Bridge is distributed under the [MIT License](LICENSE).

---

**Note:** Jira Bridge is not affiliated with or endorsed by Atlassian Jira. It is an independent tool developed to enhance the integration between Unity and Jira for efficient issue tracking and reporting.
