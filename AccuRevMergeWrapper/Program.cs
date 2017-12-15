using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace AccuRevMergeWrapper
{
  /// <summary>
  /// http://stackoverflow.com/questions/7377396/how-to-make-a-c-sharp-wrapper-around-a-console-application-any-language
  /// http://stackoverflow.com/questions/139593/processstartinfo-hanging-on-waitforexit-why?lq=1
  /// </summary>
  internal static class Program
  {
    [MTAThread]
    public static int Main(string[] args)
    {
      int returnCode = 0;
      using (Process process = new Process())
      {
        process.StartInfo.FileName = "accurev.exe";
        process.StartInfo.Arguments = "merge -o -K";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        StringBuilder output = new StringBuilder();
        StringBuilder error = new StringBuilder();

        using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
        using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
        {
          process.OutputDataReceived += (sender, e) =>
          {
            if (e.Data == null)
            {
              outputWaitHandle.Set();
            }
            else
            {
              Console.WriteLine(e.Data);
              output.AppendLine(e.Data);
            }
          };
          process.ErrorDataReceived += (sender, e) =>
          {
            if (e.Data == null)
            {
              errorWaitHandle.Set();
            }
            else
            {
              Console.WriteLine(e.Data);
              error.AppendLine(e.Data);
            }
          };

          process.Start();

          process.BeginOutputReadLine();
          process.BeginErrorReadLine();

          if (process.WaitForExit(5000) &&
              outputWaitHandle.WaitOne(5000) &&
              errorWaitHandle.WaitOne(5000))
          {
            // Process completed. Check process.ExitCode here.
#if DEBUG
            Console.WriteLine("Exit Code: " + process.ExitCode);
#endif


            if (process.ExitCode == 1)
            {
              // Check exit message.
#if DEBUG
              Console.WriteLine("Error Message: '" + error + "'");
#endif
              if (error.ToString().TrimEnd().Equals("No elements selected."))
              {
                returnCode = 0;
              }
              else
              {
                returnCode = 1;
              }
            }
            else
            {
              returnCode = process.ExitCode;
            }
          }
        }
      }
#if DEBUG
            Console.WriteLine("Exit Code: " + returnCode);
#endif
      return returnCode;
    }
  }
}
