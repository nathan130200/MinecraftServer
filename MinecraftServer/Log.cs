using System.Text.RegularExpressions;

namespace MinecraftServer;

public class Log
{
    enum LogLevel
    {
        Info,
        Warn,
        Error,
        Debug,
        Verbose
    }

    static DateTime startTime = DateTime.Now;

    record class LogEntry(string str, LogLevel lvl);

    static Queue<LogEntry> m_lines = new();

    static Log()
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                lock (m_lines)
                {
                    while (m_lines.TryDequeue(out var e))
                    {
                        var (str, lvl) = e;

                        var (c1, ch) = lvl switch
                        {
                            LogLevel.Warn => (ConsoleColor.Yellow, "W"),
                            LogLevel.Error => (ConsoleColor.Red, "E"),
                            LogLevel.Debug => (ConsoleColor.Blue, "D"),
                            LogLevel.Verbose => (ConsoleColor.Magenta, "V"),
                            LogLevel.Info or _ => (ConsoleColor.Green, "I")
                        };

                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write('[');
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write((DateTime.Now - startTime).ToString("hh':'mm':'ss'.'ffff").PadLeft(14));
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("] ");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        Console.ForegroundColor = c1;
                        Console.Write("[{0}] ", ch);

                        Console.ForegroundColor = lvl == LogLevel.Verbose ? ConsoleColor.Magenta : ConsoleColor.White;
                        Console.WriteLine(str);
                    }
                }

                await Task.Delay(1);
            }
        });

        Info("Log started at: " + startTime.ToString("dd/MM/yyyy HH:mm:ss"));
    }


    static string AddFmt(string msg)
        => $"<{DateTime.Now:HH:mm:ss.fff-zzz}> {msg}";

    public static void Info(string msg)
    {
        lock (m_lines)
            m_lines.Enqueue(new LogEntry(msg, LogLevel.Info));
    }

    public static void Warn(string msg)
    {
        lock (m_lines)
            m_lines.Enqueue(new LogEntry(msg, LogLevel.Warn));
    }

    public static void Error(string msg)
    {
        lock (m_lines)
            m_lines.Enqueue(new LogEntry(msg, LogLevel.Error));
    }

    static readonly Regex NewLinePattern = new(@"(\r|\n|\r\n)", RegexOptions.Compiled | RegexOptions.ECMAScript);

    public static void StackTrace()
    {
        lock (m_lines)
        {
            foreach (var line in NewLinePattern.Split(Environment.StackTrace)
                .Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))
                .Skip(2))

                m_lines.Enqueue(new LogEntry(line, LogLevel.Verbose));
        }
    }

    internal static void Debug(string msg)
        => m_lines.Enqueue(new LogEntry(msg, LogLevel.Debug));

    internal static void Trace(string msg)
        => m_lines.Enqueue(new LogEntry(msg, LogLevel.Verbose));
}
