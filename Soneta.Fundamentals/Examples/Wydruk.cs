using Soneta.Business;
using Soneta.Business.App;
using Soneta.Business.UI;
using System;
using System.IO;
using System.ComponentModel.Composition;
using Microsoft.Extensions.DependencyInjection;
using Soneta.Business.Db;

namespace Soneta.Fundamentals.Examples
{
    [Export(typeof(IOption))]
    [ExportMetadata("Key", ConsoleKey.F6)]
    [ExportMetadata("Description", "Wydruk")]
    [ExportMetadata("Priority", 50)]
    class Wydruk : IOption
    {
        public bool Run()
        {
            Login login = Enova.Instance.Login;
            using (Session session = login.CreateSession(false, false))
            {
                Console.WriteLine("Symulacja długiej operacji...");
                var appExePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                var rr = new ReportResult() {
                    Format = ReportResultFormat.PDF,
                    TemplateFileSource = AspxSource.Local, // z lokalnego katalogu dyskowego
                    //TemplateFileName = Path.Combine(appExePath,"Reports\\dummy.repx") // wydruk REPX
                    TemplateFileName = Path.Combine(appExePath, "Reports\\dummy.aspx"), // wydruk ASPX
                    Context = Context.Empty.Clone(session),
                    SilentProgress = true,
                };

                System.Threading.Thread.Sleep(2000);
                var service = session.GetRequiredService<IReportService>();

                using (var stream = service.GenerateReport(rr))
                {
                    System.Threading.Thread.Sleep(2000);

                    var reportFilePath = Path.Combine(appExePath, "Wydruk testowy.pdf");
                    if (File.Exists(reportFilePath))
                        File.Delete(reportFilePath);

                    var reportFile = File.Create(reportFilePath);
                    stream.CopyTo(reportFile);
                    reportFile.Flush();
                    reportFile.Close();

                    Console.WriteLine("Wygenerowano dokument PDF: (" + stream.Length + "B) " + reportFilePath );
                }
            }
            return true;
        }
    }
}
