using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX.StartupManagers
{
    class StartupManagerFactory
    {
        static public IStartupManager GetStartupManager()
        {
#if WindowsStore
            return new CentennialStartupManager();
#else
            return new DesktopStartupManager();
#endif
        }
    }
}
