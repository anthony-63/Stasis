using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using System.Text;
using Raylib_cs;

namespace Stasis.Engine;

public static class Logger {
    public static string OutputFilePath = "";
    public static Queue<string> OutputQueue = new(); 
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

    private static void StartQueueWriter() {
        new Thread(() => {
            try {
                using(StreamWriter writer = File.AppendText(OutputFilePath)) {
                    while(OutputQueue.Count > 0) {
                        string txt = OutputQueue.Dequeue();
                        Console.Out.Write(txt);
                        Console.Out.Flush();
                        writer.Write(txt);
                        writer.Flush();
                    }
                }
            } catch {}
        }).Start();
    }

    private static void WriteToOut(string type, params object[] args) {
        string txt = "";
        string head = $"[{type} {DateTime.Now.ToString("HH:mm:ss")}] ";
        txt += head;
        foreach(var arg in args) {
            txt += arg;
        }
        txt += "\n";
        OutputQueue.Enqueue(txt);
        StartQueueWriter();
    }
}