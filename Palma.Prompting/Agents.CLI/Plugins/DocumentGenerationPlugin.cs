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

namespace Agents.CLI
{


    // TODO: Clean this up, and make a cooler PPT function
    public class DocumentGenerationPlugin
    {
        [KernelFunction("generate_word_document")]
        [Description("Generates a Word document based on provided content. Ask the user if file type not specified")]
        [return: Description("The generated document content and returns the url")]
        public string CreateWordDocument(string content)
        {
            string filePath = "C:\\Users\\paudipal\\Documents\\_Projects\\temp\\output.docx";
            //using (PK.WordprocessingDocument wordDoc = PK.WordprocessingDocument.Create(filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            //{
            //    PK.MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
            //    mainPart.Document = new WP.Document();
            //    WP.Body body = mainPart.Document.AppendChild(new WP.Body());
            //    body.AppendChild(new WP.Paragraph(new WP.Run(new WP.Text(content))));
            //}

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


            //Spire.Presentation.Presentation presentation = new Spire.Presentation.Presentation();

            //// Add slide with layout
            //ISlide slide = presentation.Slides[0];

            //// Create more visible textbox
            //IAutoShape textbox = slide.Shapes.AppendShape(ShapeType.Rectangle, new RectangleF(200, 200, 400, 100));

            //// Make textbox visible with formatting
            //textbox.Fill.FillType = FillFormatType.Solid;
            //textbox.Fill.SolidColor.Color = System.Drawing.Color.Black;
            //textbox.Line.FillType = FillFormatType.Solid;
            //textbox.Line.SolidFillColor.Color = System.Drawing.Color.Black;

            //// Add text with clear formatting
            //textbox.TextFrame.Text = slideContents.FirstOrDefault();
            //textbox.TextFrame.Paragraphs[0].Alignment = TextAlignmentType.Center;
            //textbox.TextFrame.Paragraphs[0].TextRanges[0].FontHeight = 32;
            //textbox.TextFrame.Paragraphs[0].TextRanges[0].IsBold = TriState.True;

            //presentation.SaveToFile(filePath, Spire.Presentation.FileFormat.PPT);

        }

    }


}
