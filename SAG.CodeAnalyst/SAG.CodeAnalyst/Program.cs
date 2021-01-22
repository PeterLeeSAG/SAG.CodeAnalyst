using System;
using System.Diagnostics;
using System.Linq;
using SAG.Common;

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
                Console.WriteLine("Please input parameters and press enter to continue...");
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
                userInput = Console.ReadLine();
            }

            try
            {
                /*
                 Checking steps:
                 1) scan binfile folder
                 2) scan data tables/fields in DB
                 3) scan source codes, find if folder/db elements exists in the codes
                 4) read all the log files after the scanning process, and sync files to cloud
                 */
                var isCheckFS = false; //Check file structure
                var isCheckDB = false; //Check database objects
                var isCheckSC = false; //Check source codes
                var isSyncFile = false; //Sync File to Cloud
                var isMapFile = false; //Map data with Cloud File 
                var isTimeStamp = false; //Set manual timestamp
                var isCovertFileID = false; //Convert the file objectid
                var isCorrectName = false; //Correct file Name
                var isArchive = false; //archive flag
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
                        case "a":
                            isArchive = true;
                            break;
                        case "f":
                            isCheckFS = true;
                            break;
                        case "d":
                            isCheckDB = true;
                            break;
                        case "s":
                            isCheckSC = true;
                            break;
                        case "sync":
                            isSyncFile = true;
                            break;
                        case "m":
                            isMapFile = true;
                            break;
                        case "cfid":
                            isCovertFileID = true;
                            break;
                        case "ts":
                            isTimeStamp = true;
                            break;
                        case "c":
                            isCorrectName = true;
                            break;
                    }
                }

                if (isTimeStamp && timeStamp != string.Empty)
                {
                    //manual set the timestamp string here
                    logger = new LogWriter("log", timeStamp);
                }

                if (isCheckDB)
                {
                    var dbService = new DBStructureService(logger);
                    msg = "Start DBStructureService process.";
                    Console.WriteLine(msg);
                    if (isLog)
                    {
                        logger.LogWrite(msg, true);
                    }

                    dbService.Process();
                }

                if (isCheckFS)
                {
                    var fileService = new FileDirectoryService(logger, true, false, null, isArchive);
                    msg = "Start FileDirectoryService process.";
                    Console.WriteLine(msg);
                    if (isLog)
                    {
                        logger.LogWrite(msg, true);
                    }

                    fileService.Process();
                }

                if (isCheckSC)
                {
                    var sourceCodeService = new SourceCodeService(logger);
                    msg = "Start SourceCodeService process.";
                    Console.WriteLine(msg);
                    if (isLog)
                    {
                        logger.LogWrite(msg, true);
                    }

                    sourceCodeService.Process();
                }

                if (isSyncFile)
                {
                    //Call the SyncFileService here
                    var syncFileService = new SyncFileService(logger, isArchive);
                    msg = "Start SyncFileService process.";
                    Console.WriteLine(msg);
                    if (isLog)
                    {
                        logger.LogWrite(msg, true);
                    }

                    syncFileService.Process();
                }

                if (isCovertFileID)
                {
                    var fileIDConversionService = new FileIDConversionService(logger);
                    msg = "Start File ID ConversionService process.";
                    Console.WriteLine(msg);
                    if (isLog)
                    {
                        logger.LogWrite(msg, true);
                    }

                    fileIDConversionService.Process();
                }

                if (isMapFile)
                {
                    var service = new KeyMappingService(logger);
                    msg = "StartKeyMappingService process.";
                    Console.WriteLine(msg);
                    if (isLog)
                    {
                        logger.LogWrite(msg, true);
                    }

                    service.Process();
                }

                if (isCorrectName)
                {
                    var fileCorrectionServices = new FileCorrectionServices(logger);
                    msg = "Start File Name Correction process.";
                    Console.WriteLine(msg);
                    if (isLog)
                    {
                        logger.LogWrite(msg, true);
                    }

                    fileCorrectionServices.Process();
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
            Console.WriteLine(string.Format("****       SAG.FileCloud.Crawler (ver.{0})       ****", version));
            Console.WriteLine("*********************************************************");
        }
    }
}