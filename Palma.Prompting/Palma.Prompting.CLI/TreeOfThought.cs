using Azure.AI.OpenAI;
using Azure;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Palma.Prompting.CLI
{
    internal static class TreeOfThought
    {
        // System Prompts and Meta Prompts
        static string SystemPrompt = "You are a developer assistant where you only provide the code for a question. No explanation required. Write a simple json response, in this exact format:"
            + "\"{ \"Solutions\": [\"solution description...\",\r\n"
            + "\"next solution description...\", \r\n "
            + "\"next solution description...\"] }";

        static string SystemCompetePrompt = "You are a developer assistant where you only provide the code for a question. No explanation required. Write a simple json response, in this exact format:"
            + "\"{ \"RecommendedSolution\": \"recommended solution description...\",\r\n"
            + " \"AlternativeSolution\":\"alternative solution description...\" }";

        static string UserPrompt = "Find 3 solutions for the following, put the most viable option first, and keep the answers to only 25 words or less: ";
        static string CompetePrompt = "Given the 2 solutions below which one do you think is the more viable solution for the request ";

        public static string TreeOfThoughtPrompt(string input, OpenAIClient openAiClient)
        {
            SolutionResponse solutions = new SolutionResponse();
            WinningResponse winningResponse = new WinningResponse();
            string winningSolution = "";
            var chatResponseBuilder = new StringBuilder();

            // Tree of Thought Loop
            int numIterations = 6;
            for (int i = 0; i < numIterations; i++)
            {
                // Step 2: Brainstorm 3 solutions
                // Step 3: Evaluate and Rank
                //   (this could be a separate step, but currently is done in the same pass)
                //  (Future: Implement a ranking algorithm)
                Console.WriteLine("\n**** ITERATION " + i.ToString() + " ****\n");
                var treeOfThoughtPrompt = UserPrompt + input + ".";
                if (i > 0)
                    treeOfThoughtPrompt += " These solutions may not be the same as the following 3: " + solutions.Solutions.ToString();

                var chatCompletionsResponse = SetupResponse(openAiClient, SystemPrompt, treeOfThoughtPrompt);
                string solutionOutput = GetOutputOfResponse(chatCompletionsResponse);

                try
                {
                    solutions = JsonSerializer.Deserialize<SolutionResponse>(solutionOutput);
                }
                catch { }

                //Step 4: Keep the winning Idea
                if (i == 0)
                    winningSolution = solutions.Solutions == null ? string.Empty : solutions.Solutions[0];
                else
                {
                    var treeOfThoughtCompetePrompt = CompetePrompt + input + ". If these 2 solutions are the same just return the first one: " + winningSolution + " AND " + solutions.Solutions[0];

                    Console.WriteLine("\n* Winning " + i.ToString() + " *\n");
                    chatCompletionsResponse = SetupResponse(openAiClient, SystemCompetePrompt, treeOfThoughtCompetePrompt);
                    string competeOutput = GetOutputOfResponse(chatCompletionsResponse);

                    try
                    {
                        winningResponse = JsonSerializer.Deserialize<WinningResponse>(competeOutput);
                    }
                    catch { }

                    winningSolution = winningResponse.RecommendedSolution;
                }

            } // Step 5: Brainstorm 3 new Ideas in a loop

            // Step 6: Present the winning idea
            Console.WriteLine("");
            Console.WriteLine("\n\n **** The winning solution is:\n\n" + winningSolution);

            return winningSolution;
        }

        // HELPERS
        private static Response<ChatCompletions> SetupResponse(OpenAIClient openAiClient, string systemPrompt, string treeOfThoughtPrompt)
        {
            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                Messages = {
                    new ChatMessage(ChatRole.System, systemPrompt),
                    new ChatMessage(ChatRole.User, treeOfThoughtPrompt)
                    },
                FrequencyPenalty = (float?)0.1
            };
            var chatCompletionsResponse = openAiClient.GetChatCompletions(
                    "GPT-35-Turbo", chatCompletionsOptions);

            return chatCompletionsResponse;

            // Future: don't instantiate a new one, just keep the thread open
            //chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.Assistant, chatResponseBuilder.ToString()));
        }

        private static string GetOutputOfResponse(Response<ChatCompletions> chatCompletionsResponse)
        {
            string output = string.Empty;
            foreach (var chatChoice in chatCompletionsResponse.Value.Choices)//.GetChoicesStreaming())
            {
                foreach (var chatMessage in chatChoice.Message.Content)//GetMessageStreaming())
                {
                    string ou = chatMessage.ToString();
                    Console.Write(ou);
                    output += ou;
                }
            }
            Console.WriteLine("");
            return output;
        }

    }

    public class SolutionResponse
    {
        public string[] Solutions { get; set; }

        public override string ToString()
        {
            return string.Join(", ", Solutions);
        }

    }

    public class WinningResponse
    {
        public string RecommendedSolution { get; set; }
        public string AlternativeSolution { get; set; }
    }
}
