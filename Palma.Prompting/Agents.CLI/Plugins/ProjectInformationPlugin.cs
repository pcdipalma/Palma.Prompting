using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Agents.CLI
{
    public class ProjectInformationPlugin
    {

        [KernelFunction("get_project_list")]
        [Description("Gets information about all the projects")]
        [return: Description("A list of project information details")]
        public Task<List<string>> GetProjectInformationAsync()
        {
            var projectInfoList = new List<string>();
            foreach (ProjectDetails projectDetails in DummyData.ProjectData.Select(p => p.Value))
            {
                var people = string.Join(", ", projectDetails.People.Select(p => $"{p.Name} ({p.Role})"));
                projectInfoList.Add($"Project Name: {projectDetails.Name}, Status: {projectDetails.Status}, People: {people}");
            }
            return Task.FromResult(projectInfoList);
        }

        [KernelFunction("get_project_information")]
        [Description("Gets information about a specified project")]
        [return: Description("Project information details")]
        public Task<string> GetProjectInformationAsync(string projectName)
        {
            if (DummyData.ProjectData.TryGetValue(projectName, out var projectDetails))
            {
                return Task.FromResult($"Project Name: {projectDetails.Name}, Status: {projectDetails.Status}");
            }
            return Task.FromResult($"Project {projectName} not found.");
        }

        [KernelFunction("get_project_status")]
        [Description("Gets the status of a specified project")]
        [return: Description("Project status details")]
        public Task<string> GetProjectStatusAsync(string projectName)
        {
            if (DummyData.ProjectData.TryGetValue(projectName, out var projectDetails))
            {
                return Task.FromResult($"Status of project {projectName}: {projectDetails.Status}");
            }
            return Task.FromResult($"Project {projectName} not found.");
        }

        [KernelFunction("get_project_people")]
        [Description("Gets the people involved in a specified project")]
        [return: Description("List of people involved in the project")]
        public Task<List<Person>> GetProjectPeopleAsync(string projectName)
        {
            if (DummyData.ProjectData.TryGetValue(projectName, out var projectDetails))
            {
                return Task.FromResult(projectDetails.People);
            }
            return Task.FromResult(new List<Person>());
        }
    }

    public class ProjectDetails
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public List<Person> People { get; set; }

        public ProjectDetails(string name, string status, List<Person> people)
        {
            Name = name;
            Status = status;
            People = people;
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public string Role { get; set; }

        public Person(string name, string role)
        {
            Name = name;
            Role = role;
        }
    }
}
