using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;
using System.Text.Json.Serialization;

using OfficeOpenXml;
using PK = DocumentFormat.OpenXml.Packaging;
using PS = DocumentFormat.OpenXml.Presentation;
using WP = DocumentFormat.OpenXml.Wordprocessing;
using DR = DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml;

using Spire;
using Spire.Presentation.Drawing;
using Spire.Presentation;
using System.Drawing;

class Program
{
    static async Task Main(string[] args)
    {
        // Good reference for Semantic Kernel Function Calling
        // https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/?pivots=programming-language-csharp
        // Explanation of what's happening on the back end of Semantic Kernel function calling
        // https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/function-calling/?pivots=programming-language-csharp


        // Build the kernel and register plugins
        var builder = Kernel.CreateBuilder();
        builder.AddAzureOpenAIChatCompletion("gpt-4o", "https://<endpoint>.openai.azure.com/", "<key>");
        
        builder.Plugins.AddFromType<DaysAvailablePlugin>("DaysAvailablePlugin");
        builder.Plugins.AddFromType<WeatherPlugin>("WeatherPlugin");
        builder.Plugins.AddFromType<DocumentGenerationPlugin>("DocumentGenerationPlugin");
        Kernel kernel = builder.Build();

        // Set up the System Prompt
        // Vary this as needed to provide guidance to the LLM
        var systemPrompt = @"
            You are an AI assistant with access to the several useful plugins.
            When a user asks a question, determine which plugin(s) to use and provide the appropriate 
            response based on results from the plugins.";

        // Get chat completion service
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

            // Add the message from the user to the chat history
            chatHistory.AddUserMessage(input);

            // Enable auto function calling
            var executionSettings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            // Call the LLM, get the result
            var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings, kernel);
            Console.WriteLine("Assistant > " + result);

            // Add the message from the agent to the chat history
            chatHistory.AddMessage(result.Role, result?.Content!);

            // You could also first return the plugin names and parameters for a UI to display.

        }
    }
}

public class DocumentGenerationPlugin
{
    [KernelFunction("generate_word_document")]
    [Description("Generates a Word document based on provided content. Ask the user if file type not specified")]
    [return: Description("The generated document content and returns the url")]
    public string CreateWordDocument(string content)
    {
        string filePath = "C:\\Users\\paudipal\\Documents\\_Projects\\temp\\output.docx";
        using (PK.WordprocessingDocument wordDoc = PK.WordprocessingDocument.Create(filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
        {
            PK.MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
            mainPart.Document = new WP.Document();
            WP.Body body = mainPart.Document.AppendChild(new WP.Body());
            body.AppendChild(new WP.Paragraph(new WP.Run(new WP.Text(content))));
        }

        return filePath;
    }

    [KernelFunction("generate_excel_document")]
    [Description("Generates a Excel document based on provided content. Ask the user if file type not specified")]
    [return: Description("The generated document content and returns the url")]
    public string CreateExcelFile(List<List<string>> gridData)
    {
        string filePath = "C:\\Users\\paudipal\\Documents\\_Projects\\temp\\output.xlsx";
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        using (ExcelPackage package = new ExcelPackage())
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

            for (int row = 0; row < gridData.Count; row++)
            {
                for (int col = 0; col < gridData[row].Count; col++)
                {
                    worksheet.Cells[row + 1, col + 1].Value = gridData[row][col];
                }
            }

            package.SaveAs(new FileInfo(filePath));
        }
        return filePath;
    }


    [KernelFunction("generate_powerpoint_document")]
    [Description("Generates a PowerPoint document based on provided content. Ask the user if file type not specified")]
    [return: Description("The generated document content and returns the url")]
    public void CreatePowerPoint(List<string> slideContents)
    {

        string filePath = "C:\\Users\\paudipal\\Documents\\_Projects\\temp\\output.ppt";


        Spire.Presentation.Presentation presentation = new Spire.Presentation.Presentation();

        // Add slide with layout
        ISlide slide = presentation.Slides[0];

        // Create more visible textbox
        IAutoShape textbox = slide.Shapes.AppendShape(ShapeType.Rectangle, new RectangleF(200, 200, 400, 100));

        // Make textbox visible with formatting
        textbox.Fill.FillType = FillFormatType.Solid;
        textbox.Fill.SolidColor.Color = System.Drawing.Color.Black;
        textbox.Line.FillType = FillFormatType.Solid;
        textbox.Line.SolidFillColor.Color = System.Drawing.Color.Black;

        // Add text with clear formatting
        textbox.TextFrame.Text = slideContents.FirstOrDefault();
        textbox.TextFrame.Paragraphs[0].Alignment = TextAlignmentType.Center;
        textbox.TextFrame.Paragraphs[0].TextRanges[0].FontHeight = 32;
        textbox.TextFrame.Paragraphs[0].TextRanges[0].IsBold = TriState.True;

        presentation.SaveToFile(filePath, Spire.Presentation.FileFormat.PPT);

        /*
        using (PresentationDocument presentation = PresentationDocument.Create(filePath, PresentationDocumentType.Presentation))
        {
            // Add presentation part
            PresentationPart presentationPart = presentation.AddPresentationPart();
            presentationPart.Presentation = new Presentation();

            // Create slide master
            SlideMasterPart slideMasterPart = presentationPart.AddNewPart<SlideMasterPart>();
            slideMasterPart.SlideMaster = new SlideMaster(new CommonSlideData(new ShapeTree()));

            // Create slide layout
            SlideLayoutPart slideLayoutPart = slideMasterPart.AddNewPart<SlideLayoutPart>();
            slideLayoutPart.SlideLayout = new SlideLayout(new CommonSlideData(new ShapeTree()));

            // Create and add slide
            SlidePart slidePart = presentationPart.AddNewPart<SlidePart>();
            slidePart.Slide = new Slide(new CommonSlideData(new ShapeTree()));

            // Link the parts
            slidePart.AddPart(slideLayoutPart);
            slideLayoutPart.AddPart(slideMasterPart);

            // Add to slide list
            presentationPart.Presentation.SlideIdList = new SlideIdList(new SlideId() { Id = 1, RelationshipId = presentationPart.GetIdOfPart(slidePart) });
            presentationPart.Presentation.Save();
        }
        */
    }

}

// Example of calling APIs or External Sources
public class ExternalPlugin
{
    // Create as many functions as you need to give the LLM/Agent a chance to succeed at it's task
    // Maybe first validate the number:
    // [KernelFunction("validate_appnumber")]
    // THEN Call other functions
    // [KernelFunction("epo_search")]

    // Example of a potential Lexus Nexis Search
    [KernelFunction("lexusnexis_search")]
    [Description("Search Lexis for legal documents. Only call if the format is APP-O12345")]
    [return: Description("The generated document content and returns the url")]
    public Task<string> ApplicationSearchAsync(string searchTerms, string applicationNumber)
    {
        Console.WriteLine("ApplicationSearchFunction: Search and return results.");

        // 0. put in a queue for processing.

        // 1. Call the API
        string results = "Results from Lexus Nexis Search";

        // 2. Could even make a 2nd call to a smaller LLM to compile/summarize, etc.

        return Task.FromResult(results);
    }

}

public class DaysAvailablePlugin
{
    [KernelFunction("get_available_days")]
    [Description("Gets a list of available meeting days")]
    [return: Description("An array of days")]
    public Task<List<string>> GetAvailableDaysAsync()
    {
        Console.WriteLine("DaysAvailable Function: Getting available meeting days.");
        // Dummy implementation - replace with actual logic
        return Task.FromResult(new List<string>() { "Monday, Wednesday, Friday" });
    }

    // Can add other functions inside this class

}

public class WeatherPlugin
{
    [KernelFunction("get_weather_for_days")]
    [Description("Gets a list of the weather for each day")]
    [return: Description("An list of days and weather")]
    public Task<List<WeatherDays>> GetWeatherAsync()
    {
        Console.WriteLine("Weather Function: Getting weather forecasts.");
        // Dummy implementation - replace with actual API call
        List<WeatherDays> weatherForecast = new List<WeatherDays>()
        {
            new WeatherDays() { Name = "Monday", Weather = "sunny" },
            new WeatherDays() { Name = "Wednesday", Weather = "cloudy" },
            new WeatherDays() { Name = "Friday", Weather = "rainy" }
         };

        return Task.FromResult(weatherForecast);
    }

    // Can add other functions inside this class

}

public class WeatherDays
{

    [JsonPropertyName("day")]
    public string Name { get; set; }

    [JsonPropertyName("weather")]
    public string Weather { get; set; }
}
