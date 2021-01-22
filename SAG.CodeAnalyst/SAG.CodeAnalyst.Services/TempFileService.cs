using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SAG.Common;

namespace SAG.CodeAnalyst.Services
{
    public class TempFileService
    {
        private string _rootPath; 
        private int _rootLevel = 0;
        private List<string> _targetDirectories; // e.g. Temp, TempGraph, GraphTemp
        private List<string> _ignoreExtensions; // e.g. asp, js, css
        private LogWriter _logWriter;
        private bool _isLog;

        public TempFileService(LogWriter logWriter, bool isLog = true)
        {
            _logWriter = logWriter;
            _rootPath = LoadRootSetting();
            _targetDirectories = IgnoredInfoHelper.LoadTargetTempFolderNames();
            _ignoreExtensions = IgnoredInfoHelper.LoadIgnoredExtensions();
            _rootLevel = 0;
            _isLog = isLog;
        }

        public void Process()
        {
            Scan(_rootPath, _rootLevel);
        }

        private string LoadRootSetting()
        {
            return ConfigurationManager.AppSettings["RootPath"];
        }

        /// <summary>
        /// Delete all the files in the target directories
        /// </summary>
        private void Scan(string directory, int level = 0)
        {
            _logWriter.LogWrite(string.Format("Scanning {0} [Level:{1}]", directory, level));

            if (_targetDirectories.Any(d => directory.IndexOf(d) != -1))
            {
                //found the directory
                var files = Directory.GetFiles(directory);
                var msg = string.Format("Found Temp Path: {0}", directory);
                _logWriter.LogWrite(msg, true);
                Console.WriteLine(msg);

                foreach (var file in files)
                {
                    if (_ignoreExtensions.Any(e => file.IndexOf(e) == -1))
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                            msg = string.Format("Deleted file: {0}", file);
                            _logWriter.LogWrite(msg);
                            Console.WriteLine(msg);
                        }
                    }
                }
            }

            // Process the list of sub directory found in the directory.
            string[] directories = Directory.GetDirectories(directory);
            level++;
            foreach (var dir in directories)
            {
                Scan(dir, level); //Recursive call
            }
        }
    }
}
