using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Security;

namespace ProgettoPDS
{
    class RightClickManager
    {
        public static void Register(string fileType, string shellKeyName, string menuText, string menuCommand)
        {
            try
            {
                // create full path to registry location
                string regPath = string.Format(@"{0}\shell\{1}", fileType, shellKeyName);

                // add context menu to the registry
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(regPath))
                {
                    key.SetValue(null, menuText);
                }

                // add command that is invoked to the registry
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(
                string.Format(@"{0}\command", regPath)))
                {
                    key.SetValue(null, menuCommand);
                }
            }
            catch (Exception ex) when (ex is SecurityException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
