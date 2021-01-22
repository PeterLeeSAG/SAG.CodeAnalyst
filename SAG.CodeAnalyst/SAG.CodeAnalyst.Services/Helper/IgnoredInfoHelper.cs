using SAG.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAG.CodeAnalyst.Services
{
    public class IgnoredInfoHelper
    {
        /// <summary>
        /// load the ignored extensions from config / table
        /// </summary>
        /// <returns></returns>
        public static List<string> LoadIgnoredExtensions()
        {
            var list = new List<string>();
            var settingString = ConfigHelper.Read("IgnoredExtensions");
            var extensions = settingString.Split(';');
            if (extensions != null && extensions.Count() != 0)
            {
                for (int i = 0; i < extensions.Count(); i++)
                {
                    list.Add(extensions[i]);
                }
            }
            return list;
        }

        public static List<string> LoadTargetTempFolderNames()
        {
            var list = new List<string>();

            var settingString = ConfigHelper.Read("TargetTempFolderNames");
            var tempFolders = settingString.Split(';');
            if (tempFolders != null && tempFolders.Count() != 0)
            {
                for (int i = 0; i < tempFolders.Count(); i++)
                {
                    list.Add(tempFolders[i]);
                }
            }
            return list;
        }
    }
}
