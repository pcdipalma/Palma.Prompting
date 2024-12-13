using Agents.CLI.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Threading.Tasks;

namespace Agents.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize DummyData (this is optional since it's a static class and will be initialized automatically)
            var dummyData = DummyData.CalendarData; // Accessing it to ensure it's initialized

            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion("gpt-4o", "https://<endpoint>.openai.azure.com/", "<key>");

            builder.Plugins.AddFromType<CalendarPlugin>("CalendarPlugin");
            builder.Plugins.AddFromType<ProjectInformationPlugin>("ProjectInformationPlugin");
            builder.Plugins.AddFromType<ProjectManagementPlugin>("ProjectManagementPlugin");
            builder.Plugins.AddFromType<DocumentGenerationPlugin>("DocumentGenerationPlugin");
            builder.Plugins.AddFromType<OrderFoodPlugin>("OrderFoodPlugin");

            Kernel kernel = builder.Build();

            var systemPrompt = @"
                You are an AI assistant with access to several useful plugins.
                When a user asks a question, determine which plugin(s) to use and provide the appropriate 
                response based on results from the plugins.";

            ChatHistory chatHistory = new ChatHistory();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            string? input = null;
            chatHistory.AddSystemMessage(systemPrompt);

            while (true)
            {
                Console.Write("User > ");
                input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    break;
                }

                chatHistory.AddUserMessage(input);

                var executionSettings = new OpenAIPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                };

                var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings, kernel);
                Console.WriteLine("Assistant > " + result);

                chatHistory.AddMessage(result.Role, result?.Content!);
            }
        }
    }
}
