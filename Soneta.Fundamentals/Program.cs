using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading;

namespace Soneta.Fundamentals
{
    static class Program
    {
        static void Main()
        {
            Enova.Instance.InitializeAndLogIn();

            using (var catalog = new AssemblyCatalog(typeof(Program).Assembly))
            {
                using (var mef = new CompositionContainer(catalog))
                {
                    bool @continue = true;
                    (ConsoleKey, ConsoleKey) [] acceptableKeyRanges = new[]
                    {
                        (ConsoleKey.F1, ConsoleKey.F12),
                        (ConsoleKey.Q, ConsoleKey.Q)
                    };

                    do
                    {
                        mef.GetExports<IOption, IOptionData>()
                            .Select(o => o.Metadata)
                            .OrderBy(o => o.Key) //.OrderBy(o => o.Priority)
                            .ToList().ForEach(o => Console.WriteLine("({0}) - {1}", o.Key, o.Description));
                        Console.Write("Soneta.Fundamentals $ ");

                        ConsoleKey key;
                        do
                        {
                            key = Console.ReadKey(true).Key;
                        } while (!acceptableKeyRanges.Any(r => r.Contains(key)));

                        var option = mef.GetExports<IOption, IOptionData>()
                            .FirstOrDefault(o => o.Metadata.Key == key);

                        try
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("{0} [{1}]", key,
                                option == null ? "Opcja nieobsługiwana"
                                : option.Metadata.Description.Short(40));
                        }
                        finally
                        {
                            Console.ResetColor();
                        }

                        @continue = option == null || OptionRunner.RunAsync(option.Value);
                    } while (@continue);
                }
            }
        }

        private static bool Contains(this (ConsoleKey, ConsoleKey) range, ConsoleKey key)
        {
            return key >= range.Item1 && key <= range.Item2;
        }

        internal static string Short(this string value, int limit = 79)
        {
            var suffix = "...";
            if (string.IsNullOrEmpty(value) ||
                value.Length <= limit - suffix.Length)
            {
                return value;
            }
            else
            {
                return value.Substring(0, limit - 3) + suffix;
            }

        }

    }
    class OptionRunner
    {
        bool? _result;

        static internal bool Run(IOption option)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                return option.Run();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Out.WriteLine(e);
                return true;
            }
            finally
            {
                Console.ResetColor();
            }
        }

        static internal bool RunAsync(IOption option)
        {
            int delay = 80;
            var startedAt = DateTime.Now;
            var runner = new OptionRunner();
            using (var backgroundWorker = new BackgroundWorker())
            {
                backgroundWorker.DoWork += runner.Run_Work;
                backgroundWorker.RunWorkerCompleted += runner.Run_Completed;
                backgroundWorker.RunWorkerAsync(option);

                while (backgroundWorker.IsBusy)
                {
                    Thread.Sleep(delay);
                    DrawProgress((DateTime.Now - startedAt).TotalMilliseconds / delay);
                }
            }

            return runner._result ?? true;
        }

        static void DrawProgress(double progress)
        {
            int progressBarLength = 6;
            char[] dots = new[] { '-', '=' };
            int progressValue = Convert.ToInt32(progress % progressBarLength);
            Console.Write("[{0}]\r",
                string.Empty.PadLeft(1, dots[1])
                .PadLeft(progressValue, dots[0])
                .PadRight(progressBarLength, dots[0]));
        }

        void Run_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                _result = e.Result as bool?;
            }
            else
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Out.WriteLine(e.Error);
                }
                finally
                {
                    Console.ResetColor();
                }
            }
        }

        void Run_Work(object sender, DoWorkEventArgs e)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                var option = (IOption)e.Argument;
                e.Result = option.Run();
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }

    interface IOption
    {
        bool Run();
    }

    public interface IOptionData
    {
        int Priority { get; }

        ConsoleKey Key { get; }

        string Description { get; }
    }

    [Export(typeof(IOption))]
    [ExportMetadata("Key", ConsoleKey.Q)]
    [ExportMetadata("Description", "Zakończ")]
    [ExportMetadata("Priority", 1000)]
    class Exit : IOption
    {
        bool IOption.Run()
        {
            Enova.Instance.Unload();
            return false;
        }
    }
}
