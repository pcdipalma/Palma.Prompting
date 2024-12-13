using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DocumentFormat.OpenXml.Vml;
using Microsoft.VisualBasic;
using OfficeOpenXml;
using System.Drawing;

namespace Agents.CLI.Plugins
{
    public class OrderFoodPlugin
    {
        [KernelFunction("order_food")]
        [Description("Orders food for the team")]
        [return: Description("Confirmation of the food order")]
        public Task<string> OrderFoodAsync(string foodItem, int quantity)
        {
            // Dummy implementation - replace with actual logic
            return Task.FromResult($"{quantity}x {foodItem} ordered for the team");
        }
    }

}
