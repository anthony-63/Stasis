using System.ComponentModel.DataAnnotations;
using Raylib_cs;

namespace Stasis.Engine;

public static class Logger {
    public static string OutputFilePath = "";

    public static void Init(string outputPath) {
        // Raylib.SetTraceLogLevel(TraceLogLevel.None);

        if(File.Exists(outputPath)) File.Delete(outputPath);

        OutputFilePath = outputPath;

        Info("Initialized Logger");
    }

    public static void Info(params object[] args) {
        WriteToOut("INFO", args);
    }

    public static void Warn(params object[] args) {
        WriteToOut("WARN", args);
    }

    public static void Err(params object[] args) {
        WriteToOut("ERROR", args);
    }

    private static void WriteToOut(string type, params object[] args) {
        using(StreamWriter writer = File.AppendText(OutputFilePath)) {
            string head = $"[{type} {DateTime.Now.ToString("HH:mm:ss")}] ";
            writer.Write(head);
            Console.Write(head);

            foreach(var arg in args) {
                Console.Write(arg);
                writer.Write(arg);
            }
            Console.WriteLine();
            writer.WriteLine();

            Console.Out.Flush();
            writer.Flush();
        }
    }
}