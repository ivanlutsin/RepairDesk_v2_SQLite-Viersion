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
                

                AddLine("Акт Приема в ремонт");
                AddLine("");
                AddLine($"Номер заказа: {orderId}");
                AddLine($"Дата и время приема: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                AddLine("");
                AddLine("Информация о клиенте");
                AddLine($"Полное имя: {fullName}");
                AddLine($"Номер телефона: {phone}");
                AddLine("");
                AddLine("Информация о устройстве");
                AddLine($"Тип: {deviceType}");
                AddLine($"Бренд: {brand}");
                AddLine($"Модель: {model}");
                AddLine($"Серийный номер: {serial}");
                AddLine("");
                AddLine("Описание неисправности:");
                AddLine(problem);
                AddLine("");
                AddLine("Примерная стоимость ремонта:");
                AddLine("________________________________");
                AddLine("");
                AddLine("Условия приема в ремонт:");
                AddLine("Диагностика и согласование");
                
                
                AddLine("1) В случае, если по результатам диагностики будет" +
                        " установлена экономическая или техническая нецелесообразность " +
                        "проведения ремонтных работ (в том числе при превышении стоимости" +
                        "ремонта 70% рыночной стоимости устройства либо при отсутствии " +
                        "необходимых для ремонта компонентов), диагностика осуществляется " +
                        "Исполнителем без взимания платы. Плата за диагностику в указанном случае не взимается.");
                
                
                AddLine("2) Стоимость диагностических работ вне зависимости от типа устройства составляет" +
                        " 1500 (одна тысяча пятьсот) рублей. В случае отказа Заказчика от выполнения ремонтных " +
                        "работ по результатам диагностики оплата диагностики производится в полном размере — 1500 рублей. " +
                        "При согласии Заказчика на проведение ремонта сумма, уплаченная за диагностику, засчитывается в " +
                        "счёт общей стоимости ремонта. ");




                AddLine("3) В случае обнаружения в процессе ремонта скрытых дефектов, " +
                        "которые не были оговорены Заказчиком при приёме и которые делают ремонт " +
                        "технически или экономически иным, чем предполагалось изначально, " +
                        "Исполнитель немедленно уведомляет об этом Заказчика любым доступным способом, " +
                        "позволяющим подтвердить факт отправки и получения сообщения. Исполнитель вправе " +
                        "пересмотреть стоимость и сроки ремонта с учётом вновь выявленных обстоятельств и сообщает об этом Заказчику.");
                
                
                AddLine("Заказчик, получивший такое уведомление, " +
                        "обязан подтвердить своё согласие на продолжение " +
                        "ремонта по новой стоимости либо заявить отказ в течение 24 часов " +
                        "с момента получения уведомления. Если Заказчик отказывается от " +
                        "продолжения ремонта в связи с обнаружением скрытых дефектов, он " +
                        "оплачивает Исполнителю фиксированную стоимость диагностики в размере 1500 рублей, " +
                        "а также фактически понесённые расходы на разборку и дефектовку устройства из " +
                        "расчёта 1000 рублей за каждый полный или неполный час работы. " +
                        "Также оплате подлежат все использованные при диагностике и разборе " +
                        "расходные материалы (припои, флюсы, химические составы, защитные плёнки и т.п.) по их фактической стоимости.");
                
                
                AddLine("При этом Исполнитель в любом случае, вне зависимости от состояния " +
                        "устройства и причин отказа от ремонта, обязуется полностью собрать " +
                        "устройство в том объёме, в котором это возможно без приобретения новых деталей. " +
                        "Устройство возвращается Заказчику в собранном, работоспособном или " +
                        "неработоспособном состоянии — в зависимости от исходных неисправностей и скрытых дефектов, " +
                        "но обязательно полностью собранным и с установленными на место всеми штатными винтами, " +
                        "кабелями и экранирующими элементами. Дополнительная плата за обратную сборку не взимается.");
                
                
                AddLine("Если Заказчик не подтвердил согласие на продолжение ремонта в течение 24 часов с " +
                        "момента получения уведомления, это расценивается как отказ от ремонта. В этом случае " +
                        "устройство собирается и возвращается Заказчику, а он обязан оплатить диагностику, " +
                        "фактическое время разборки и расходные материалы на основании выставленного счёта. " +
                        "Отказ от оплаты даёт Исполнителю право удерживать устройство до полного расчёта в " +
                        "соответствии с действующим законодательством. Претензии по качеству сборки, " +
                        "внешнему виду или невозможности использования устройства в связи с его исходным " +
                        "техническим состоянием не принимаются, так как сборка производится без замены " +
                        "неисправных компонентов и исключительно с целью приведения устройства в транспортабельный и целостный вид.");
                
                
                AddLine("");
                
                
                AddLine("Порядок оплаты:");
                AddLine("1) Полная оплата — при выдаче устройства.");
                AddLine("2) Предоплата 15% от согласованной суммы.");
                AddLine("3) При удорожании ремонта из-за скрытых дефектов — новую сумму согласовывают.");
                
                
                
                AddLine("Сроки ремонта");
                AddLine("1) Срок выполнения: до 30 рабочих дней с момента согласования сметы");
                AddLine("2) Задержка возможна при отсутствии запчастей — клиент уведомляется");
                AddLine("3) При задержке по вине сервиса — клиент вправе потребовать уменьшения цены или расторжения договора с возвратом предоплаты.");
                
                
                AddLine("");
                
                AddLine("Гарантия");
                AddLine("На выполненные работы - 1 год с момента выдачи устройства");
                AddLine("На установленные запчасти — согласно гарантии поставщика");
                AddLine("Гарантия не действует при: механических повреждениях, попадании жидкости, самостоятельном вскрытии, некорректном ПО установленном клиентом");
                
                AddLine("");
                
                AddLine("Ответственность сервиса");
                AddLine("Сервис отвечает за сохранность устройства и его компонентов во время ремонта");
                AddLine("За сохранность пользовательских данных сервис не отвечает (клиент обязан сделать бэкап).");
                AddLine("При утере или неустранимой поломке по вине сервиса — компенсация в пределах (сумма или %, например 50% от рыночной стоимости, но не более стоимости ремонта).");
                
                AddLine("");
                
                AddLine("Действия клиента");
                AddLine("Клиент обязан сообщить о: залитии, падениях, ремонте в других сервисах, известных скрытых дефектах.");
                AddLine("Устройство сдаётся с отключённой учётной записью [iCloud, FRP, Xiaomi Account, Samsung Account и т.п - в случаи невозможнности отвязать устройство знать данные для входа].");
                AddLine("Пароль/код-пароль (если нужен) предоставляется письменно.");
                
                AddLine("Хранение и выдача");
                AddLine("Срок хранения готового устройства — [30 календарных дней].");
                AddLine("Через 6 месяцев невостребованное устройство переходит в собственность сервиса с правом утилизации или продажи.");
                
                AddLine("");
                
                AddLine("Форс-мажор и отказ от ремонта");
                AddLine("Сервис вправе отказаться от ремонта, если: ремонт экономически нецелесообразен (>70% цены нового/исправного б/у), запчасти отсутствуют в разумный срок, обнаружены неустранимые дефекты платы.");
                AddLine("При отказе — возврат предоплаты за минусом диагностики и уже понесённых расходов (с подтверждением).");
                
                AddLine("");
                
                AddLine("Разрешение споров");
                AddLine("Все споры — путём переговоров, при недостижении согласия — в суде по месту нахождения исполнителя.");
                AddLine("Претензии по скрытым недостаткам принимаются в течение [5–7 дней] после получения устройства.");
                
                AddLine("");
                AddLine("С условиями приема в ремонт ознакомлен, согласен.");
                AddLine("Предупрежден о необходимости сделать резервную копию данных.");
                AddLine("Устройство сдаю без учетной записи.");
                AddLine("Пароль, Графический ключ, PIN-код - указать при наличии ___________________________________.");
                AddLine("");
                AddLine("Дата: _________________");
                AddLine("Подпись клиента: ________________/расшифровка: _________________");
                AddLine("");
                AddLine("Принял инженер (ФИО): _________________________________");
                AddLine("Принял инженер (Подпись): _________________/расшифровка: _____________________________________");

                mainPart.Document.Append(body);
                
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