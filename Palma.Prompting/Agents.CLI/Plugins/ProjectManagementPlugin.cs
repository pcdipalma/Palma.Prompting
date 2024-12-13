public class ProjectManagementPlugin
{
    [KernelFunction("create_task")]
    [Description("Creates a new task in a specified project")]
    [return: Description("Confirmation of the created task")]
    public Task<string> CreateTaskAsync(string projectName, string taskName)
    {
        // Dummy implementation - replace with actual logic
        return Task.FromResult($"Task '{taskName}' created in project '{projectName}'");
    }

    [KernelFunction("assign_task")]
    [Description("Assigns a task to a specified person")]
    [return: Description("Confirmation of the assigned task")]
    public Task<string> AssignTaskAsync(string taskName, string personName)
    {
        // Dummy implementation - replace with actual logic
        return Task.FromResult($"Task '{taskName}' assigned to '{personName}'");
    }
}
