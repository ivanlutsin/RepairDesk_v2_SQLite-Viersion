using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.Sqlite;
using YourApp.Services;

namespace RepairDesk.ViewModels.AdminFunctionViewModel.AddOrderFunction;

public class OrderCreatedViewModel
{
    private readonly DatabaseService _db = new();

    public async Task PrintReceptionActAsync(long orderId, Window parent)
    {
        try
        {
            var file = await parent.StorageProvider.SaveFilePickerAsync(
                new FilePickerSaveOptions
                {
                    Title = "Save Reception Act",
                    SuggestedFileName = $"Reception_Act_No_{orderId}.docx",
                    FileTypeChoices =
                    [
                        new FilePickerFileType("Word Document")
                        {
                            Patterns = ["*.docx"]
                        }
                    ]
                });

            if (file == null)
            {
                Console.WriteLine("User cancelled saving");
                return;
            }

            string path = file.Path.LocalPath;
            Console.WriteLine($"Attempting to save file to: {path}");

            // Create directory if it doesn't exist
            var directory = System.IO.Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                    Console.WriteLine($"Directory created: {directory}");
                }
            }

            // Check write permissions
            try
            {
                using (var testStream =
                       System.IO.File.Create(System.IO.Path.Combine(directory ?? "", "test_write.txt")))
                {
                    testStream.WriteByte(0);
                }

                System.IO.File.Delete(System.IO.Path.Combine(directory ?? "", "test_write.txt"));
                Console.WriteLine("Write permissions confirmed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No write permissions: {ex.Message}");
                await ShowMessage(parent, "Error", $"No write permissions for folder: {ex.Message}");
                return;
            }

            // Get data from database...
            string fullName = "", phone = "", deviceType = "", brand = "", model = "", serial = "", problem = "";

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    """
                    SELECT
                        ClientFullName,
                        PhoneNumber,
                        DeviceType,
                        Brand,
                        Model,
                        SerialNumber,
                        ProblemDescription
                    FROM Repairs
                    WHERE id = $id
                    """;
                cmd.Parameters.AddWithValue("$id", orderId);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    fullName = reader["ClientFullName"]?.ToString() ?? "";
                    phone = reader["PhoneNumber"]?.ToString() ?? "";
                    deviceType = reader["DeviceType"]?.ToString() ?? "";
                    brand = reader["Brand"]?.ToString() ?? "";
                    model = reader["Model"]?.ToString() ?? "";
                    serial = reader["SerialNumber"]?.ToString() ?? "";
                    problem = reader["ProblemDescription"]?.ToString() ?? "";
                }
            }

            // CREATE DOCUMENT WITH EXPLICIT SAVE
            using (var document = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document))
            {
                var mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = new Body();

                void AddLine(string text)
                {
                    var paragraph = new Paragraph();
                    var run = new Run(new Text(text));
            
                    // Remove spacing after paragraphs
                    paragraph.ParagraphProperties = new ParagraphProperties(
                        new SpacingBetweenLines
                        {
                            After = "0",      // No space after paragraph
                            Before = "0",     // No space before paragraph
                            Line = "240",     // 1.0 line spacing
                            LineRule = LineSpacingRuleValues.Auto
                        }
                    );
            
                    paragraph.Append(run);
                    body.Append(paragraph);
                }
                

                AddLine("DEVICE RECEPTION ACT");
                AddLine("");
                AddLine($"Order number: {orderId}");
                AddLine($"Reception date and time: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                AddLine("");
                AddLine("Client information");
                AddLine($"Full name: {fullName}");
                AddLine($"Phone: {phone}");
                AddLine("");
                AddLine("Device information");
                AddLine($"Device type: {deviceType}");
                AddLine($"Brand: {brand}");
                AddLine($"Model: {model}");
                AddLine($"Serial number: {serial}");
                AddLine("");
                AddLine("Problem description:");
                AddLine(problem);
                AddLine("");
                AddLine("Estimated repair cost:");
                AddLine("________________________________");
                AddLine("");
                AddLine("Repair terms:");
                AddLine("1. Device accepted for diagnostics and repair.");
                AddLine("2. Final repair cost may differ from estimate.");
                AddLine("3. Contractor is not responsible for hidden defects.");
                AddLine("4. Client confirms device transfer in described condition.");
                AddLine("5. Device must be collected after notification of readiness.");
                AddLine("6. Diagnostic fee may apply if repair is refused after diagnostics.");
                AddLine("");
                AddLine("");
                AddLine("Client signature:");
                AddLine("________________________________");
                AddLine("");
                AddLine("Technician signature:");
                AddLine("________________________________");

                mainPart.Document.Append(body);

                // EXPLICIT SAVE
                mainPart.Document.Save();
                

                Console.WriteLine("Document saved and closed");
            }

            Console.WriteLine($"File should be created at: {path}");

            // Verify file was created
            if (System.IO.File.Exists(path))
            {
                Console.WriteLine($"File successfully created! Size: {new System.IO.FileInfo(path).Length} bytes");
                await ShowMessage(parent, "Success", $"File saved to: {path}");
            }
            else
            {
                Console.WriteLine($"FILE WAS NOT CREATED!");
                await ShowMessage(parent, "Error", $"File was not created at: {path}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            await ShowMessage(parent, "Error", $"Save error: {ex.Message}");
        }
    }

// Helper method for showing messages
    private async Task ShowMessage(Window parent, string title, string message)
    {
        Console.WriteLine($"{title}: {message}");
    }
}