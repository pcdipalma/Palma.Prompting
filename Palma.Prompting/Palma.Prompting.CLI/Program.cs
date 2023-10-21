// See https://aka.ms/new-console-template for more information
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Palma.Prompting.CLI;
using System.Text;

// Setup Open AI Connection (or whatever model you're using)
// Note: you'll need an appsettings.json file
IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

string url = config.GetSection("AZURE_OPENAI_URL").Value;
string key = config.GetSection("AZURE_OPENAI_API_KEY").Value;
var openAiClient = new OpenAIClient(new Uri(url), new AzureKeyCredential(key));

// Capture the Question and Get Response
while (true)
{
    // Step 1: Input Your Problem
    Console.WriteLine("Hello, ask me a question:");
    var input = Console.ReadLine();

    TreeOfThought.TreeOfThoughtPrompt(input, openAiClient);

}


