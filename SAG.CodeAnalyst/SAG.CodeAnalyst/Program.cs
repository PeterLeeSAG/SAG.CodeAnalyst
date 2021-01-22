using System;
using System.Diagnostics;
using System.Linq;
using SAG.Common;
using SAG.CodeAnalyst.Services;

namespace SAG.CodeAnalyst
{
    /// <summary>
    /// Crawler for getting the info about the existing file systems
    /// </summary>
    public class Program
    {
        private static void Main(string[] args)
        {
            var logger = new LogWriter();
            var isLog = true;
            var msg = "";
            var parameters = args;
            var userInput = "";
            var timeStamp = "";

            ShowProgramHeading();

            if (args.Count() == 0)
            {
                /*
                Console.WriteLine("/a: Active Archive Flag");
                Console.WriteLine("/f: Check File structure");
                Console.WriteLine("/d: Check Database objects");
                Console.WriteLine("/s: Check Source codes");
                Console.WriteLine("/sync: Sync file to SAGFileCloud");
                Console.WriteLine("/m: Map module date with the file id of SAGFileCloud");
                Console.WriteLine("/cfid: Convert file object ID");
                Console.WriteLine("/c: Correct File name");
                Console.WriteLine("/ts=[yyyyMMddhhmmss]: Manually TimeStamp setting for the prefix of the log files");
                Console.WriteLine("ps: Multiple parameters are supported");
                */
                
                Console.WriteLine("Please input parameters and press enter to continue...");
                Console.WriteLine("/rt: Remove temp files");

                userInput = Console.ReadLine();
            }

            try
            {
                var isTimeStamp = false; //Set manual timestamp
                var isRemoveTemp = false;

                parameters = userInput.Split('/');

                foreach (var parameter in parameters)
                {
                    var parameterAlt = parameter.Trim().ToLower();

                    //cut the timestamp out
                    if (parameterAlt.Count() > 2 && parameterAlt.Substring(0, 2) == "ts")
                    {
                        timeStamp = parameterAlt.Substring(3, parameterAlt.Length - 3); //e.g. ts=20200928142035
                        parameterAlt = "ts";
                    }

                    switch (parameterAlt)
                    {
                        case "rt":
                            isRemoveTemp = true;
                            break;
                    }
                }

                if (isTimeStamp && timeStamp != string.Empty)
                {
                    //manual set the timestamp string here
                    logger = new LogWriter("log", timeStamp);
                }

                if (isRemoveTemp)
                {
                    var tempFileService = new TempFileService(logger, true);
                    msg = "Start Remove temp files process.";
                    Console.WriteLine(msg);
                    if (isLog)
                    {
                        logger.LogWrite(msg, true);
                    }

                    tempFileService.Process();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error occurred: {0}", ex.ToString()));
            }

            Console.WriteLine("Press Enter to exit the program...");
            Console.ReadLine();
        }

        private static void ShowProgramHeading()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            Console.WriteLine("*********************************************************");
            Console.WriteLine(string.Format("****       SAG.CodeAnalyst (ver.{0})       ****", version));
            Console.WriteLine("*********************************************************");
        }
    }
}